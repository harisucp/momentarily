using System;
using System.Linq;
using Apeek.Common.Logger;
using Apeek.NH.DataAccessLayer.DataAccess;
using Apeek.NH.Repository.Repositories;
using Apeek.Common;
using Apeek.Common.Models;
using Apeek.ViewModels.Mappers;
using Apeek.ViewModels.Mappers.Imp;
using Apeek.ViewModels.Models.Impl;
using Apeek.Common.Extensions;
using Apeek.Entities.Entities;
using Apeek.ViewModels.Models;
using System.Collections.Generic;
using Momentarily.ViewModels.Models;
using Apeek.Common.Definitions;

namespace Apeek.Core.Services.Impl
{
    public class UserDataService<T> : LangDataService, IUserDataService<T> where T : Good
    {
        private readonly IRepositoryUser _repUser;
        private readonly IRepositoryGoodRequest _repositoryGoodRequest;
        private readonly IRepositoryUserGood _repositoryUserGood;
        private readonly IRepositoryGoodBookingRank _repBookingRank;
        private readonly IRepositoryCancelledRequest _repositoryCancelledRequest;
        private readonly IRepositoryGood<T, T> _repGood;
        private readonly IRepositorySubscibes _repSubscriber;
        private readonly int ReviewsPerPage = 5;
        private readonly int GoodsPerPage = 6;
        public UserDataService(IRepositoryUser repUser, IRepositoryGoodBookingRank repBookingRank, IRepositoryGood<T, T> repGood,
            IRepositorySubscibes repSubscriber, IRepositoryGoodRequest repositoryGoodRequest,
            IRepositoryUserGood repositoryUserGood, IRepositoryCancelledRequest repositoryCancelledRequest)
        {
            _repUser = repUser;
            _repBookingRank = repBookingRank;
            _repGood = repGood;

            _repBookingRank = repBookingRank;
            _repGood = repGood;
            _repositoryGoodRequest = repositoryGoodRequest;
            _repositoryUserGood = repositoryUserGood;
            _repositoryCancelledRequest = repositoryCancelledRequest;
            _repSubscriber = repSubscriber;
        }
        public Result<UserPublicProfile> GetPublicUserProfile(int? userId)
        {
            var result = new Result<UserPublicProfile>(CreateResult.Error, new UserPublicProfile());
            try
            {
                Uow.Wrap(u =>
                {
                    var user = _repUser.Get(userId);
                    if (user != null)
                    {
                        var userViewModel = new UserViewModel();
                        result.Obj.User = EntityMapper<IUserMapper>.Mapper().Map(user, userViewModel);
                        result.Obj.User.UserProfileImageUrl = user.UserImages.MainImageUrlNormal(true);
                        int currentPage = 1;
                        result.Obj.SeekersReviews = new ListViewModel<ReviewViewModel>()
                        {
                            Pagination = new ViewModels.BaseViewModel.Pagination()
                            {
                                CurrentPage = currentPage,
                                ItemsPerPage = ReviewsPerPage,
                            }
                        };
                        var fromSeekers = _repBookingRank.GetRankFromSeekers(userId);
                        var fromSharers = _repBookingRank.GetRankFromSharers(userId);
                        var seekersReview = fromSeekers.GroupBy(g => g.Id).Select(c => new { Id = c.Key, Rank = c.Average(r => r.Rank) }).ToList();
                        var sharersReview = fromSharers.GroupBy(g => g.Id).Select(c => new { Id = c.Key, Rank = c.Average(r => r.Rank) }).ToList();
                        result.Obj.SeekersCountReview = seekersReview.Count();
                        result.Obj.SharersCountReview = sharersReview.Count();
                        //var count = fromSeekers.Count() + fromSharers.Count();
                        result.Obj.RankSeekers = (decimal)seekersReview.Select(c => c.Rank).DefaultIfEmpty(0).Average();
                        result.Obj.RankSharers = (decimal)sharersReview.Select(c => c.Rank).DefaultIfEmpty(0).Average();
                        result.Obj.SeekersReviews = GetListReviews(fromSeekers);
                        result.Obj.SharersReviews = GetListReviews(fromSharers);
                        result.Obj.TotalSharedRentals = 0;
                        result.Obj.TotalCompletedRentals = 0;
                        result.Obj.CompletedPercentage = 0;
                        result.Obj.TotalCancelledRentals = 0;


                        try
                        {

                            var allSharedTransactions = (from request in _repositoryGoodRequest.Table
                                                         join usergood in _repositoryUserGood.Table
                                                         on request.GoodId equals usergood.GoodId
                                                         where usergood.UserId == userId
                                                         select request).ToList();

                            var allBorrowedTransactions = (from request in _repositoryGoodRequest.Table
                                                           where request.UserId == userId
                                                           select request).ToList();


                            result.Obj.TotalSharedRentals = allSharedTransactions.Count();
                            result.Obj.TotalBorrowedRentals = allBorrowedTransactions.Count();
                            result.Obj.TotalRentals = allSharedTransactions.Count() + allBorrowedTransactions.Count();

                            var CompletedSharedRentals = allSharedTransactions.Where(x =>
                             x.StatusId == (int)UserRequestStatus.Closed
                             || x.StatusId == (int)UserRequestStatus.ClosedWithDamaged
                             || x.StatusId == (int)UserRequestStatus.ClosedWithDispute
                             || x.StatusId == (int)UserRequestStatus.ClosedWithLate
                             || x.StatusId == (int)UserRequestStatus.ClosedWithLateAndDamaged
                            ).Count();


                            var CompletedBorrowedRentals = allBorrowedTransactions.Where(x =>
                            x.StatusId == (int)UserRequestStatus.Closed
                            || x.StatusId == (int)UserRequestStatus.ClosedWithDamaged
                            || x.StatusId == (int)UserRequestStatus.ClosedWithDispute
                            || x.StatusId == (int)UserRequestStatus.ClosedWithLate
                            || x.StatusId == (int)UserRequestStatus.ClosedWithLateAndDamaged
                            ).Count();

                            result.Obj.TotalCompletedRentals = CompletedSharedRentals + CompletedBorrowedRentals;

                            result.Obj.TotalCancelledRentals = (from p in _repositoryCancelledRequest.Table
                                                                where p.UserId == userId
                                                                select p).Count();

                            if (result.Obj.TotalRentals > 0 && result.Obj.TotalCompletedRentals > 0)
                            {
                                var completedPercentage = Convert.ToDecimal(result.Obj.TotalCompletedRentals * 100) / result.Obj.TotalRentals;
                                result.Obj.CompletedPercentage = Math.Round(completedPercentage, 2);
                            }
                            else
                            {
                                result.Obj.CompletedPercentage = 0;
                            }


                            if (result.Obj.TotalRentals > 0 && result.Obj.TotalCancelledRentals > 0)
                            {
                                var cancelledPercentage = Convert.ToDecimal(result.Obj.TotalCancelledRentals * 100) / result.Obj.TotalRentals;
                                result.Obj.CancelledPercentage = Math.Round(cancelledPercentage, 2);
                            }
                            else
                            {
                                result.Obj.CancelledPercentage = 0;
                            }

                        }
                        catch (Exception ex)
                        {

                        }
                        var goods = GetUserItems(userId);
                        result.Obj.Listings = new ListViewModel<UserProfileGoodViewModel>()
                        {
                            Pagination = new ViewModels.BaseViewModel.Pagination()
                            {
                                CurrentPage = currentPage,
                                ItemsPerPage = GoodsPerPage
                            }
                        };
                        var goodsViewModel = goods.Skip(currentPage - 1).Take(GoodsPerPage).ToList();
                        var goodEntityMapper = EntityMapper<IGoodMapper>.Mapper();
                        //ListViewModel<UserProfileGoodViewModel> lst = new ListViewModel<UserProfileGoodViewModel>();
                        //foreach (var item in goodsViewModel)
                        //{
                        //    result.Obj.Listings.Items.Add(new
                        //        UserProfileGoodViewModel
                        //    {
                        //        GoodId = item.Id,
                        //        Name = item.Name,
                        //        Description = item.Description,
                        //        Price = item.Price,
                        //        PricePerWeek = item.PricePerWeek,
                        //        PricePerMonth = item.PricePerMonth,
                        //        RentPeriodDay = item.RentPeriodDay,
                        //        RentPeriodWeek = item.RentPeriodWeek,
                        //        RentPeriodMonth = item.RentPeriodMonth,
                        //        AgreeToDeliver = item.AgreeToDeliver,
                        //        AgreeToShareImmediately = item.AgreeToShareImmediately,
                        //        Latitude = item.GoodLocation.Latitude,
                        //        Longitude = item.GoodLocation.Longitude,
                        //        //Location = item.
                        //        //Type = item.GoodLocation.GetType
                        //        //Image = item.GoodImages.
                        //        //Deposit = item.UserGood.User.
                        //    });
                        //}
                        goodsViewModel.ForEach(g =>
                        {
                            result.Obj.Listings.Items.Add((UserProfileGoodViewModel)goodEntityMapper.Map(g, ((UserProfileGoodViewModel)new UserProfileGoodViewModel())));
                        });
                        result.Obj.Listings.Pagination.TotalItems = goods.Count();
                        //result.Obj.SeekersReviews.Reviews = seekersReviews;
                        result.CreateResult = CreateResult.Success;
                    }
                }, null, LogSource.UserService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogError(LogSource.UserService, string.Format("Get user profile failed: {0}", ex));
            }
            return result;
        }
        public ListViewModel<ReviewViewModel> GetListReviews(IQueryable<GoodBookingRank> items, int currentPage = 1)
        {
            var result = new ListViewModel<ReviewViewModel>()
            {
                Pagination = new ViewModels.BaseViewModel.Pagination()
                {
                    CurrentPage = currentPage,
                    ItemsPerPage = ReviewsPerPage
                }
            };
            result.Pagination.TotalItems = items.Count();
            var pageOfList = items.OrderByDescending(c => c.CreateDate).Distinct().Skip(result.Pagination.CurrentPage - 1).Take(result.Pagination.ItemsPerPage).ToList();
            var reviewsEntitytMapper = EntityMapper<IGoodBookingRankMapper>.Mapper();
            pageOfList.ForEach(r =>
            {
                result.Items.Add(reviewsEntitytMapper.Map(r, new ReviewViewModel()));
            });
            return result;
        }
        private IQueryable<T> GetUserItems(int? userId)
        {
            var items = (from g in _repGood.Table
                         join ug in _repGood.TableFor<UserGood>() on g.Id equals ug.GoodId
                         where ug.UserId == userId && g.IsArchive == false
                         select g);
            return items;
        }
        public Result<ListViewModel<ReviewViewModel>> GetSeekersReview(int userId, int page = 1)
        {
            var result = new Result<ListViewModel<ReviewViewModel>>(CreateResult.Error, null);
            try
            {
                Uow.Wrap(u =>
                {
                    var r = _repBookingRank.GetRankFromSeekers(userId);
                    result.Obj = GetListReviews(r, page);
                    result.CreateResult = CreateResult.Success;
                }, null, LogSource.UserService);
            }
            catch (Exception e)
            {
                Ioc.Get<IDbLogger>().LogError(LogSource.UserService, string.Format("Get seekers review failed: {0}", e));
            }
            return result;
        }
        public Result<ListViewModel<ReviewViewModel>> GetSharersReview(int userId, int page = 1)
        {
            var result = new Result<ListViewModel<ReviewViewModel>>(CreateResult.Error, null);
            try
            {
                Uow.Wrap(u =>
                {
                    var r = _repBookingRank.GetRankFromSharers(userId);
                    result.Obj = GetListReviews(r, page);
                    result.CreateResult = CreateResult.Success;
                }, null, LogSource.UserService);
            }
            catch (Exception e)
            {
                Ioc.Get<IDbLogger>().LogError(LogSource.UserService, string.Format("Get seekers review failed: {0}", e));
            }
            return result;
        }
        public int GetUserCount()
        {
            int result =0;
            try
            {
                Uow.Wrap(u =>
                {
                    result =  _repUser.GetUserCount();
                }, null, LogSource.UserService);
            }
            catch (Exception e)
            {
                Ioc.Get<IDbLogger>().LogError(LogSource.UserService, string.Format("Get seekers review failed: {0}", e));
            }
            return result;

        }

        public int GetAvailableItemsCount()
        {
            //var items = (from g in _repGood.Table
            //             join ug in _repGood.TableFor<UserGood>() on g.Id equals ug.GoodId
            //             where ug.UserId == userId && g.IsArchive == false
            //             select g);

           // var data = _repUser.GetAvailableItemsCount();
            int result = 0;
            try
            {
                Uow.Wrap(u =>
                {
                    //result = _repUser.GetAvailableItemsCount();
                    result = _repGood.getgoodcount();
                }, null, LogSource.UserService);
            }
            catch (Exception e)
            {
                Ioc.Get<IDbLogger>().LogError(LogSource.UserService, string.Format("Get seekers review failed: {0}", e));
            }
            return result;
           
        }
       public int GetNewLendersCount()
        {
            int result = 0;
            try
            {
                Uow.Wrap(u =>
                {
                    //result = _repUser.GetAvailableItemsCount();
                    result = _repGood.getnewlenderscount();
                }, null, LogSource.UserService);
            }
            catch (Exception e)
            {
                Ioc.Get<IDbLogger>().LogError(LogSource.UserService, string.Format("Get seekers review failed: {0}", e));
            }
            return result;
            
        }

        public int GetNewBorrowersCount()
        {
            int result = 0;
            try
            {
                Uow.Wrap(u =>
                {
                    //result = _repUser.GetAvailableItemsCount();
                    result = _repGood.getnewborrowerscount();
                }, null, LogSource.UserService);
            }
            catch (Exception e)
            {
                Ioc.Get<IDbLogger>().LogError(LogSource.UserService, string.Format("Get seekers review failed: {0}", e));
            }
            return result;
        }
        public int GetTotalNewItemsCount()
        {
            int result = 0;
            try
            {
                Uow.Wrap(u =>
                {
                    //result = _repUser.GetAvailableItemsCount();
                    result = _repGood.gettotalnewitemscount();
                }, null, LogSource.UserService);
            }
            catch (Exception e)
            {
                Ioc.Get<IDbLogger>().LogError(LogSource.UserService, string.Format("Get seekers review failed: {0}", e));
            }
            return result;
        }

        public int GetTotalLendersCount()
        {
            int result = 0;
            try
            {
                Uow.Wrap(u =>
                {
                    //result = _repUser.GetAvailableItemsCount();
                    result = _repGood.gettotallenderscount();
                }, null, LogSource.UserService);
            }
            catch (Exception e)
            {
                Ioc.Get<IDbLogger>().LogError(LogSource.UserService, string.Format("Get seekers review failed: {0}", e));
            }
            return result;
        }
        public int GetTotalBorrowersCount()
        {
            int result = 0;
            try
            {
                Uow.Wrap(u =>
                {
                    //result = _repUser.GetAvailableItemsCount();
                    result = _repGood.gettotalborrowerscount();
                }, null, LogSource.UserService);
            }
            catch (Exception e)
            {
                Ioc.Get<IDbLogger>().LogError(LogSource.UserService, string.Format("Get seekers review failed: {0}", e));
            }
            return result;
        }

        public List<User> GetAllUser()
        {
            List<User> result = null;
            try
            {
                Uow.Wrap(u =>
                {
                    //result = _repUser.GetAvailableItemsCount();
                    result = _repUser.GetAllUser();
                }, null, LogSource.UserService);
            }
            catch (Exception e)
            {
                Ioc.Get<IDbLogger>().LogError(LogSource.UserService, string.Format("Get seekers review failed: {0}", e));
            }
            return result;
        }

       public List<Subscribes> GetAllSubscriber()
        {
            List<Subscribes> result = null;
            try
            {
                Uow.Wrap(u =>
                {
                    //result = _repUser.GetAvailableItemsCount();
                    result = _repSubscriber.GetAllSubscriber();
                }, null, LogSource.UserService);
            }
            catch (Exception e)
            {
                Ioc.Get<IDbLogger>().LogError(LogSource.UserService, string.Format("Get subscriber failed: {0}", e));
            }
            return result;
        }


        public int UserBlocked(int userId, bool checkedValue)
        {
            int result = 0;
            try
            {
                Uow.Wrap(u =>
                {
                    //result = _repUser.GetAvailableItemsCount();
                    result = _repUser.UserBlockedStatusChanged(userId,checkedValue);
                }, null, LogSource.UserService);
            }
            catch (Exception e)
            {
                Ioc.Get<IDbLogger>().LogError(LogSource.UserService, string.Format("Get seekers review failed: {0}", e));
            }
            return result;
        }
       
        //public List<Good> GetAvailableItems()
        //{
        //    //List<Good> result = null;
        //    var result = (dynamic)null;
        //    try
        //    {
        //        Uow.Wrap(u =>
        //        {
        //            result = _repGood.getAvailableItems();

        //        }, null, LogSource.UserService);
        //    }
        //    catch (Exception e)
        //    {
        //        Ioc.Get<IDbLogger>().LogError(LogSource.UserService, string.Format("Get seekers review failed: {0}", e));
        //    }
        //    return result.ToList();
        //}

        public List<User> Getnewborrowerslist()        {            List<User> result = null;            try            {                Uow.Wrap(u =>                {
                    //result = _repUser.GetAvailableItemsCount();
                    result = _repGood.getnewborrowerslist();                }, null, LogSource.UserService);            }            catch (Exception e)            {                Ioc.Get<IDbLogger>().LogError(LogSource.UserService, string.Format("Get seekers review failed: {0}", e));            }            return result;        }        public List<User> GetTotalborrowerslist()        {            List<User> result = null;            try            {                Uow.Wrap(u =>                {
                    //result = _repUser.GetAvailableItemsCount();
                    result = _repGood.gettotalborrowerslist();                }, null, LogSource.UserService);            }            catch (Exception e)            {                Ioc.Get<IDbLogger>().LogError(LogSource.UserService, string.Format("Get seekers review failed: {0}", e));            }            return result;        }        public List<User> GetnewLenderslist()        {            List<User> result = null;            try            {                Uow.Wrap(u =>                {                    result = _repGood.getnewLenderslist();                }, null, LogSource.UserService);            }            catch (Exception e)            {                Ioc.Get<IDbLogger>().LogError(LogSource.UserService, string.Format("Get seekers review failed: {0}", e));            }            return result;        }        public List<User> GetTotalLenderslist()        {            List<User> result = null;            try            {                Uow.Wrap(u =>                {                    result = _repGood.gettotalLenderslist();                }, null, LogSource.UserService);            }            catch (Exception e)            {                Ioc.Get<IDbLogger>().LogError(LogSource.UserService, string.Format("Get seekers review failed: {0}", e));            }            return result;        }


        public double GetTotalEarning()        {            double result = 0;            try            {                Uow.Wrap(u =>                {
                    //result = _repUser.GetAvailableItemsCount();
                    result = _repGood.gettotalearning();                }, null, LogSource.UserService);            }            catch (Exception e)            {                Ioc.Get<IDbLogger>().LogError(LogSource.UserService, string.Format("Get seekers review failed: {0}", e));            }            return result;        }        public List<SharedUsers> GetDLLLenderslist()        {            List<SharedUsers> result = null;            try            {                Uow.Wrap(u =>                {                    result = _repGood.getdllLenderslist();                }, null, LogSource.UserService);            }            catch (Exception e)            {                Ioc.Get<IDbLogger>().LogError(LogSource.UserService, string.Format("Get seekers review failed: {0}", e));            }            return result;        }        public List<SharedUsers> GetDLLBorrowersList()        {            List<SharedUsers> result = null;            try            {                Uow.Wrap(u =>                {
                    //result = _repUser.GetAvailableItemsCount();
                    result = _repGood.getdllborrowerslist();                }, null, LogSource.UserService);            }            catch (Exception e)            {                Ioc.Get<IDbLogger>().LogError(LogSource.UserService, string.Format("Get seekers review failed: {0}", e));            }            return result;        }
        public string GetCountryCodeByPhoneNumber(string number)        {            string code = "";            Uow.Wrap(u =>            {                code = _repUser.GetCountryCodeByPhoneNumber(number);            }, null, LogSource.PersonService);            return code;        }

        public BookingRankViewModal GetUserReviews(int userid)        {            BookingRankViewModal bookingRankViewModal = new BookingRankViewModal();            try            {                Uow.Wrap(u =>                {

                    var SeekersRank = (from r in _repBookingRank.Table
                                       join r1 in _repBookingRank.Table
                                             on r.GoodRequestId equals r1.GoodRequestId
                                       where r.SharerId == userid && r1.Id != r.Id && r1.SeekerId == r.ReviewerId && (r.is_deleted == null || r.is_deleted == false)
                                       select new BookingRank
                                       {
                                           Id = r.Id,
                                           GoodRequestId = r.GoodRequestId,
                                           GoodId = r.GoodId,
                                           SharerId = r.SharerId,
                                           SeekerId = r.SeekerId,
                                           ReviewerId = r.ReviewerId,
                                           GoodName = (from good in _repGood.Table where good.Id == r.GoodId select good.Name).FirstOrDefault(),
                                           SharerName = (from user in _repUser.Table where user.Id == r.SharerId select user.FullName).FirstOrDefault(),
                                           SeekerName = (from user in _repUser.Table where user.Id == r.SeekerId select user.FullName).FirstOrDefault(),
                                           ReviewerName = (from user in _repUser.Table where user.Id == r.ReviewerId select user.FullName).FirstOrDefault(),
                                           Rank = r.Rank,
                                           Message = r.Message,
                                           Reviewer = r.Reviewer,
                                           UserId = userid
                                       }).ToList();


                    var SharerRank = (from r in _repBookingRank.Table
                                      join r1 in _repBookingRank.Table on r.GoodRequestId equals r1.GoodRequestId
                                      where r.SeekerId == userid && r1.Id != r.Id && r1.SharerId == r.ReviewerId && (r.is_deleted == null || r.is_deleted == false)
                                      select new BookingRank
                                      {
                                          Id = r.Id,
                                          GoodRequestId = r.GoodRequestId,
                                          GoodId = r.GoodId,
                                          SharerId = r.SharerId,
                                          SeekerId = r.SeekerId,
                                          ReviewerId = r.ReviewerId,
                                          GoodName = (from good in _repGood.Table where good.Id == r.GoodId select good.Name).FirstOrDefault(),
                                          SharerName = (from user in _repUser.Table where user.Id == r.SharerId select user.FullName).FirstOrDefault(),
                                          SeekerName = (from user in _repUser.Table where user.Id == r.SeekerId select user.FullName).FirstOrDefault(),
                                          ReviewerName = (from user in _repUser.Table where user.Id == r.ReviewerId select user.FullName).FirstOrDefault(),
                                          Rank = r.Rank,
                                          Message = r.Message,
                                          Reviewer = r.Reviewer,
                                          UserId = userid
                                      }).ToList();                    bookingRankViewModal.RankFromSeekers = SeekersRank;                    bookingRankViewModal.RankFromSharers = SharerRank;                }, null, LogSource.UserService);            }            catch (Exception e)            {                Ioc.Get<IDbLogger>().LogError(LogSource.UserService, string.Format("Get user reviews failed: {0}", e));            }            return bookingRankViewModal;        }

        public bool GetupdateGoodBookingRank(int id)
        {
            bool result = false;            Uow.Wrap(u =>            {                GoodBookingRank goodBookingRank = _repBookingRank.Table.Where(x => x.Id == id).FirstOrDefault();                goodBookingRank.is_deleted = true;
                _repBookingRank.Update(goodBookingRank);                result = true;            }, null, LogSource.PersonService);            return result;
        }

        public bool UpdateIsViewedNotification(int id)
        {
            bool result = false;            Uow.Wrap(u =>            {                var good = _repGood.Table.Where(x => x.Id == id).FirstOrDefault();                good.IsViewed = true;
                _repGood.SaveOrUpdate(good);                result = true;            }, null, LogSource.PersonService);            return result;
        }
    }
}