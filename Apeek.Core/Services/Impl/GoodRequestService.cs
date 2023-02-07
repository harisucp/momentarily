using System;
using System.Collections.Generic;
using System.Linq;
using Apeek.Common;
using Apeek.Common.Definitions;
using Apeek.Common.Extensions;
using Apeek.Common.Logger;
using Apeek.Common.Models;
using Apeek.Common.UrlHelpers;
using Apeek.Entities.Constants;
using Apeek.Entities.Entities;
using Apeek.NH.DataAccessLayer.DataAccess;
using Apeek.NH.Repository.Repositories;
using Apeek.ViewModels.Mappers;
using Apeek.ViewModels.Mappers.Imp;
using Apeek.ViewModels.Models;
using Apeek.ViewModels.Models.Impl;
using Apeek.Common.Controllers;
using Momentarily.ViewModels.Models.Braintree;

namespace Apeek.Core.Services.Impl
{
    public class GoodRequestService<T> : IGoodRequestService where T : Good
    {
        protected readonly IRepositoryUser _repUser;
        protected readonly IRepositoryGoodRequest _repGoodRequest;
        protected readonly IRepositoryGoodBooking _repGoodBooking;
        protected readonly IRepositoryDisputes _repDisputes;
        protected readonly IRepositoryGood<T, T> _repGood;
        protected readonly IRepositoryUserGood _repUserGood;
        protected readonly IRepositoryPhoneNumber _repUserPhone;
        protected readonly IRepositoryGoodBookingRank _repRank;
        protected readonly ISendMessageService _emailService;
        protected readonly IRepositoryPaypalInfoPaymentDetail _repositoryPaypalInfoPaymentDetail;
        protected readonly IRepositoryGoodImg _repositoryGoodImg;
        protected readonly IRepositoryCancelledRequest _repositoryCancelledRequest;
        //private readonly IRepositoryUserCoupon _repositoryUserCoupon;
        public GoodRequestService(IRepositoryUser repUser, IRepositoryGoodRequest repGoodRequest,
            IRepositoryGoodBooking repGoodBooking, IRepositoryGood<T, T> repGood,
            IRepositoryUserGood repUserGood, IRepositoryPhoneNumber repUserPhone, IRepositoryGoodBookingRank repRank,
            IRepositoryDisputes repDisputes, IRepositoryPaypalInfoPaymentDetail repositoryPaypalInfoPaymentDetail, IRepositoryGoodImg repositoryGoodImg, IRepositoryCancelledRequest repositoryCancelledRequest/*, IRepositoryUserCoupon repositoryUserCoupon*/)
        {
            _repUser = repUser;
            _repGoodRequest = repGoodRequest;
            _repGoodBooking = repGoodBooking;
            _repGood = repGood;
            _repUserGood = repUserGood;
            _repUserPhone = repUserPhone;
            _repRank = repRank;
            _emailService = Ioc.Get<ISendMessageService>();
            _repDisputes = repDisputes;
            _repositoryPaypalInfoPaymentDetail = repositoryPaypalInfoPaymentDetail;
            _repositoryGoodImg = repositoryGoodImg;
            _repositoryCancelledRequest = repositoryCancelledRequest;
        //_repositoryUserCoupon = repositoryUserCoupon;
    }
        public Result<GoodRequestViewModel> GetGoodRequest(int userId, int requestId)
        {
            var result = new Result<GoodRequestViewModel>(CreateResult.Error, new GoodRequestViewModel());
            try
            {
                Uow.Wrap(u =>
                {
                    GoodRequest userRequest = _repGoodRequest.GetGoodRequest(userId, requestId);
                    if (userRequest != null)
                    {
                        result.Obj = EntityMapper<IGoodRequestMapper>.Mapper().Map(userRequest, new GoodRequestViewModel());
                        result.Obj.UserPhone = _repUserPhone.GetUserPhone(userRequest.User.Id);
                        result.Obj.ApplyForDelivery = userRequest.GoodBooking.ApplyForDelivery;
                        result.Obj.CouponCode = userRequest.CouponCode;
                        result.Obj.CouponDiscount = userRequest.DiscountAmount;
                        result.Obj.StartTime = userRequest.GoodBooking.StartTime;                        result.Obj.EndTime = userRequest.GoodBooking.EndTime;
                        result.CreateResult = CreateResult.Success;
                    }
                },
                null,
                LogSource.GoodRequestService);

            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.GoodRequestService, string.Format("Get good booking fail. Ex: {0}.", ex));
            }
            return result;
        }
        public Result<GoodRequestViewModel> GetGoodRequest(int requestId)
        {
            var result = new Result<GoodRequestViewModel>(CreateResult.Error, new GoodRequestViewModel());
            try
            {
                Uow.Wrap(u =>
                {
                    var userRequest = _repGoodRequest.GetUserRequest(requestId);
                    if (userRequest != null)
                    {
                        result.Obj = EntityMapper<IGoodRequestMapper>.Mapper().Map(userRequest, new GoodRequestViewModel());
                        result.Obj.UserPhone = _repUserPhone.GetUserPhone(userRequest.User.Id);
                        result.Obj.StartTime = userRequest.GoodBooking.StartTime;                        result.Obj.EndTime = userRequest.GoodBooking.EndTime;
                        result.CreateResult = CreateResult.Success;

                    }
                },
                null,
                LogSource.GoodRequestService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.GoodRequestService, string.Format("Get good booking fail. Ex: {0}.", ex));
            }
            return result;
        }
        public Result<GoodRequest> GetRequest(int requestId)
        {
            var result = new Result<GoodRequest>(CreateResult.Error, new GoodRequest());
            try
            {
                Uow.Wrap(u =>
                {
                    var userRequest = _repGoodRequest.Table.Where(x => x.Id == requestId).FirstOrDefault();
                    result.CreateResult = CreateResult.Success;
                },
                null,
                LogSource.GoodRequestService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.GoodRequestService, string.Format("Get good request fail. Ex: {0}.", ex));
            }
            return result;
        }

        public List<GoodRequest> GetAllGoodRequest()
        {
            var result = new List<GoodRequest>();
            try
            {
                Uow.Wrap(u =>
                {
                    result = _repGoodRequest.Table.Where(x => x.StatusId == (int)UserRequestStatus.ReturnConfirmed
                    || x.StatusId == (int)UserRequestStatus.Late
                    || x.StatusId == (int)UserRequestStatus.Damaged
                    || x.StatusId == (int)UserRequestStatus.LateAndDamaged
                    ).ToList();

                },
                null,
                LogSource.GoodRequestService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.GoodRequestService, string.Format("Get good booking fail. Ex: {0}.", ex));
            }
            return result;
        }



        public Result<BookingListViewModel> GetGoodRequests(int userId, int goodId)
        {
            var result = new Result<BookingListViewModel>(CreateResult.Error, new BookingListViewModel());
            try
            {
                Uow.Wrap(u =>
                {
                    var goodRequestMapper = EntityMapper<IGoodRequestMapper>.Mapper();
                    result.Obj.GoodRequests = _repGoodRequest
                        .GetGoodRequests(userId, goodId)
                        .Select(p => goodRequestMapper.Map(p, new GoodRequestViewModel())).ToList();
                    result.Obj.GoodName = _repGood.Get(goodId).Name;
                    if (result.Obj.GoodRequests.Any())
                        result.CreateResult = CreateResult.Success;
                },
                null,
                LogSource.GoodRequestService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.GoodRequestService, string.Format("Get good requests fail. Ex: {0}.", ex));
            }
            return result;
        }
        public Result<GoodRequestViewModel> GetUserRequest(int userId, int requestId)
        {
            var result = new Result<GoodRequestViewModel>(CreateResult.Error, new GoodRequestViewModel());
            try
            {
                Uow.Wrap(u =>
                {
                    var userRequest = _repGoodRequest.GetUserRequest(requestId);
                    if (userRequest != null)
                    {
                        result.Obj = EntityMapper<IGoodRequestMapper>.Mapper().Map(userRequest, new GoodRequestViewModel());
                        result.Obj.StartTime = userRequest.GoodBooking.StartTime;                        result.Obj.EndTime = userRequest.GoodBooking.EndTime;
                        var userGood = _repUserGood.Table.FirstOrDefault(r => r.GoodId == userRequest.GoodId);
                        if (userGood != null)
                        {
                            result.Obj.OwnerId = userGood.UserId;
                            result.Obj.OwnerEmail = userGood.User.Email;
                            result.Obj.OwnerName = userGood.User.FirstName;
                            result.Obj.OwnerPhone = _repUserPhone.GetUserPhone(userGood.UserId);
                            result.Obj.ApplyForDelivery = userRequest.GoodBooking.ApplyForDelivery;
                            result.Obj.CouponCode = userRequest.CouponCode;
                            result.Obj.CouponDiscount = userRequest.DiscountAmount;
                            result.Obj.StartTime = userRequest.GoodBooking.StartTime;                            result.Obj.EndTime = userRequest.GoodBooking.EndTime;

                        }
                        result.Obj.GoodImageUrl = userRequest.Good.GoodImages.ImageUrlNormal(0, ImageFolder.Good.ToString(), false);
                        result.CreateResult = CreateResult.Success;
                    }
                },
                null,
                LogSource.GoodRequestService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.GoodRequestService, string.Format("Get user booking fail. Ex: {0}.", ex));
            }
            return result;
        }
        //public Result<IEnumerable<GoodRequestViewModel>> GetUserRequests(int userId)
        //{
        //    var result = new Result<IEnumerable<GoodRequestViewModel>>(CreateResult.Error, new List<GoodRequestViewModel>());
        //    try
        //    {
        //        Uow.Wrap(u =>
        //        {
        //            var goodRequestMapper = EntityMapper<IGoodRequestMapper>.Mapper();
        //            var userRequests = _repGoodRequest.Table.Where(p => p.UserId == userId).ToList();
        //            if (userRequests.Any())
        //                result.Obj = userRequests.Select(p => goodRequestMapper.Map(p, new GoodRequestViewModel())).ToList();
        //            result.CreateResult = CreateResult.Success;
        //        },
        //        null,
        //        LogSource.GoodRequestService);
        //    }
        //    catch (Exception ex)
        //    {
        //        Ioc.Get<IDbLogger>().LogWarning(LogSource.GoodRequestService, string.Format("Get user bookings fail. Ex: {0}.", ex));
        //    }
        //    return result;
        //}
        public Result<IEnumerable<GoodRequestViewModel>> GetUserRequests(int userId)
        {
            var result = new Result<IEnumerable<GoodRequestViewModel>>(CreateResult.Error, new List<GoodRequestViewModel>());
            try
            {
                Uow.Wrap(u =>
                {
                    // var goodRequestMapper = EntityMapper<IGoodRequestMapper>.Mapper();
                    var userRequests = (from req in _repGoodRequest.Table
                                        where req.UserId == userId
                                        join booking in _repGoodBooking.Table on req.Id equals booking.GoodRequestId
                                        join good in _repGood.Table
                                        on req.GoodId equals good.Id
                                        select new GoodRequestViewModel
                                        {
                                            Id = req.Id,
                                            GoodName = good.Name,
                                            StartDate = booking.StartDate,
                                            EndDate = booking.EndDate,
                                            CustomerCost = req.CustomerCost,
                                            StatusId = req.StatusId,
                                            GoodImageUrl = (from img in _repositoryGoodImg.Table where img.GoodId == req.GoodId && img.Type == (int)ImageType.Original select img.FileName).FirstOrDefault(),
                                            StatusName = Enum.GetName(typeof(UserRequestStatus), req.StatusId),
                                            CreateDate = req.CreateDate

                                        }).ToList();
                    //var userRequests = _repGoodRequest.Table.Where(p => p.UserId == userId).ToList();
                    if (userRequests.Any())
                        result.Obj = userRequests;
                    result.CreateResult = CreateResult.Success;
                },
                null,
                LogSource.GoodRequestService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.GoodRequestService, string.Format("Get user bookings fail. Ex: {0}.", ex));
            }
            return result;
        }
        public bool ApproveGoodRequest(int userId, int requestId)
        {
            try
            {
                Ioc.Get<IDbLogger>().LogMessage(LogSource.GoodRequestService, string.Format("Trying to approve user request."));
                return GoodRequestChangeStatus(userId, requestId, UserRequestStatus.Approved);

            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.GoodRequestService, string.Format("Approve user request fail. Ex: {0}.", ex));
            }
            return false;
        }
        public bool DeclineGoodRequest(int userId, int requestId)
        {
            try
            {
                Ioc.Get<IDbLogger>().LogMessage(LogSource.GoodRequestService, string.Format("Trying to decline user request."));
                return GoodRequestChangeStatus(userId, requestId, UserRequestStatus.Declined);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.GoodRequestService, string.Format("Decline user request fail. Ex: {0}.", ex));
            }
            return false;
        }
        //san
        public bool NotResponedGoodRequest()
        {
            try
            {
                Ioc.Get<IDbLogger>().LogMessage(LogSource.GoodRequestService, string.Format("Trying to not Responded user request."));
                return GoodRequestChangeStatusNotRespondedJob(UserRequestStatus.NotResponded);

            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.GoodRequestService, string.Format("Not Responded user request fail. Ex: {0}.", ex));
            }
            return false;
        }
        public bool CloseGoodRequest(int userId, int requestId)
        {
            try
            {
                Ioc.Get<IDbLogger>().LogMessage(LogSource.GoodRequestService, string.Format("Trying to close the request."));

                return GoodRequestChangeStatus(userId, requestId, UserRequestStatus.Closed);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.GoodRequestService, string.Format("Close user request fail. Ex: {0}.", ex));
            }
            return false;
        }
        public bool CancelGoodRequest(int userId, int requestId)        {            try            {                Ioc.Get<IDbLogger>().LogMessage(LogSource.GoodRequestService, string.Format("Trying to cancel the request."));                return GoodRequestChangeStatus(userId, requestId, UserRequestStatus.CanceledBySharer);            }            catch (Exception ex)            {                Ioc.Get<IDbLogger>().LogWarning(LogSource.GoodRequestService, string.Format("Cancel user request fail. Ex: {0}.", ex));            }            return false;        }
        public bool CancelGoodRequestBeforePayment(int userId, int requestId)        {            try            {                Ioc.Get<IDbLogger>().LogMessage(LogSource.GoodRequestService, string.Format("Trying to cancel the request."));                return GoodRequestChangeStatus(userId, requestId, UserRequestStatus.CanceledBySharerBeforePayment);            }            catch (Exception ex)            {                Ioc.Get<IDbLogger>().LogWarning(LogSource.GoodRequestService, string.Format("Cancel user request before payment fail. Ex: {0}.", ex));            }            return false;        }
        public bool ReleaseGoodRequest(int userId, int requestId)
        {
            var result = false;
            try
            {
                Uow.Wrap(u =>
                {
                    Ioc.Get<IDbLogger>().LogMessage(LogSource.GoodRequestService, string.Format("Trying to change good request status to released."));

                    var goodRequest = _repGoodRequest.Table.Where(x => x.Id == requestId).FirstOrDefault();
                    if (goodRequest != null)
                    {
                        goodRequest.StatusId = (int)UserRequestStatus.Released;
                        _repGoodRequest.SaveOrUpdateAudit(goodRequest, userId);
                        result = true;
                    }
                },
                null,
                LogSource.GoodRequestService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.GoodRequestService, string.Format("Change good request status to released is failed. Ex: {0}.", ex));
            }
            return result;
        }

        public bool ReceiveGoodRequest(int userId, int requestId)
        {
            var result = false;
            try
            {
                Uow.Wrap(u =>
                {
                    Ioc.Get<IDbLogger>().LogMessage(LogSource.GoodRequestService, string.Format("Trying to change good request status to receive."));

                    var goodRequest = _repGoodRequest.Table.Where(x => x.Id == requestId).FirstOrDefault();
                    if (goodRequest != null)
                    {
                        goodRequest.StatusId = (int)UserRequestStatus.Received;
                        _repGoodRequest.SaveOrUpdateAudit(goodRequest, userId);
                        result = true;
                    }
                },
                null,
                LogSource.GoodRequestService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.GoodRequestService, string.Format("Change good request status to receive is failed. Ex: {0}.", ex));
            }
            return result;
        }

        public bool ReturnGoodRequest(int userId, int requestId)
        {
            var result = false;
            try
            {
                Uow.Wrap(u =>
                {
                    Ioc.Get<IDbLogger>().LogMessage(LogSource.GoodRequestService, string.Format("Trying to change good request status to return."));

                    var goodRequest = _repGoodRequest.Table.Where(x => x.Id == requestId).FirstOrDefault();
                    if (goodRequest != null)
                    {
                        goodRequest.StatusId = (int)UserRequestStatus.Returned;
                        _repGoodRequest.SaveOrUpdateAudit(goodRequest, userId);
                        result = true;
                    }
                },
                null,
                LogSource.GoodRequestService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.GoodRequestService, string.Format("Change good request status to return is failed. Ex: {0}.", ex));
            }
            return result;
        }

        public bool ReturnConfirmGoodRequest(int userId, int requestId)
        {
            var result = false;
            try
            {
                Uow.Wrap(u =>
                {
                    Ioc.Get<IDbLogger>().LogMessage(LogSource.GoodRequestService, string.Format("Trying to change good request status to return confirm."));

                    var goodRequest = _repGoodRequest.Table.Where(x => x.Id == requestId).FirstOrDefault();
                    if (goodRequest != null)
                    {
                        goodRequest.StatusId = (int)UserRequestStatus.ReturnConfirmed;
                        _repGoodRequest.SaveOrUpdateAudit(goodRequest, userId);
                        result = true;
                    }
                },
                null,
                LogSource.GoodRequestService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.GoodRequestService, string.Format("Change good request status to return confirm is failed. Ex: {0}.", ex));
            }
            return result;
        }
        public bool CancelUserRequest(int userId, int requestId)        {            try            {                Ioc.Get<IDbLogger>().LogMessage(LogSource.GoodRequestService, string.Format("Seeker trying to cancel the request"));                return UserRequestChangeStatus(userId, requestId, UserRequestStatus.CanceledByBorrower);            }            catch (Exception ex)            {                Ioc.Get<IDbLogger>().LogWarning(LogSource.GoodRequestService, string.Format("Cancel user request fail. Ex: {0}.", ex));            }            return false;        }
        public bool CancelUserRequestBeforePayment(int userId, int requestId)        {            try            {                Ioc.Get<IDbLogger>().LogMessage(LogSource.GoodRequestService, string.Format("Seeker trying to cancel the request"));                return UserRequestChangeStatus(userId, requestId, UserRequestStatus.CanceledByBorrowerBeforePayment);            }            catch (Exception ex)            {                Ioc.Get<IDbLogger>().LogWarning(LogSource.GoodRequestService, string.Format("Cancel user request before payment fail. Ex: {0}.", ex));            }            return false;        }
        public bool SeekerPaidUserRequest(int userId, int requestId)
        {
            try
            {
                //Ioc.Get<IDbLogger>().LogMessage(LogSource.GoodRequestService, string.Format("Trying to paid user request."));

                return UserRequestChangeStatus(userId, requestId, UserRequestStatus.Paid);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.GoodRequestService, string.Format("Paid user request fail. Ex: {0}.", ex));
            }
            return false;
        }
        public bool ReviewingUserRequest(int userId, int requestId)
        {
            try
            {
                Ioc.Get<IDbLogger>().LogMessage(LogSource.GoodRequestService, string.Format("Trying to reviewing user request."));

                return UserRequestChangeStatus(userId, requestId, UserRequestStatus.Reviewing);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.GoodRequestService, string.Format("Reviewing user request fail. Ex: {0}.", ex));
            }
            return false;
        }

        //public bool AmountChargedUserRequest(int userId, int requestId)
        //{
        //    try
        //    {
        //        return UserRequestChangeStatus(userId, requestId, UserRequestStatus.AmountCharged);
        //    }
        //    catch (Exception ex)
        //    {
        //        Ioc.Get<IDbLogger>().LogWarning(LogSource.GoodRequestService, string.Format("Paid user request fail. Ex: {0}.", ex));
        //    }
        //    return false;
        //}
        //public bool DepositChargedUserRequest(int userId, int requestId)
        //{
        //    try
        //    {
        //        return UserRequestChangeStatus(userId, requestId, UserRequestStatus.DepositCharged);
        //    }
        //    catch (Exception ex)
        //    {
        //        Ioc.Get<IDbLogger>().LogWarning(LogSource.GoodRequestService, string.Format("Paid user request fail. Ex: {0}.", ex));
        //    }
        //    return false;
        //}
        public bool SharerTakeUserRequest(int userId, int requestId, IUnitOfWork unitOfWork = null)
        {
            try
            {
                Ioc.Get<IDbLogger>().LogMessage(LogSource.GoodRequestService, string.Format("Trying to sharer released user request."));

                return UserRequestChangeStatus(userId, requestId, UserRequestStatus.Released, unitOfWork);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.GoodRequestService, string.Format("Sharer released user request fail. Ex: {0}.", ex));
            }
            return false;
        }
        private bool GoodRequestChangeStatus(int userId, int requestId, UserRequestStatus requestStatus)
        {
            var result = false;
            try
            {
                Uow.Wrap(u =>
                {
                    Ioc.Get<IDbLogger>().LogMessage(LogSource.GoodRequestService, string.Format("Trying to change good request status."));

                    var goodRequest = _repGoodRequest.GetGoodRequest(userId, requestId);
                    if (goodRequest != null)
                    {
                        if (CanChangeStatus((UserRequestStatus)goodRequest.StatusId, requestStatus))
                        {
                            goodRequest.StatusId = (int)requestStatus;
                            _repGoodRequest.SaveOrUpdateAudit(goodRequest, userId);
                            result = true;
                        }
                    }
                },
                null,
                LogSource.GoodRequestService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.GoodRequestService, string.Format("Change good request status fail. Ex: {0}.", ex));
            }
            return result;
        }
        private bool GoodRequestChangeStatusNotRespondedJob(UserRequestStatus requestStatus)
        {
            var result = false;
            try
            {
                Uow.Wrap(u =>
                {
                    Ioc.Get<IDbLogger>().LogMessage(LogSource.GoodRequestService, string.Format("Trying to change status not responded."));

                    var goodRequest = _repGoodRequest.GetGoodRequestNotRespondedJob();
                    if (goodRequest != null)
                    {
                        //if (CanChangeStatus((UserRequestStatus)goodRequest.StatusId, requestStatus))
                        //{
                        //    goodRequest.StatusId = (int)requestStatus;
                        //    _repGoodRequest.SaveOrUpdateAudit(goodRequest, userId);
                        //    result = true;
                        //}
                    }
                },
                null /*LogSource.GoodRequestService*/);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.GoodRequestService, string.Format("Change status not responded fail. Ex: {0}.", ex));
            }
            return result;
        }


        private bool CanChangeStatus(UserRequestStatus oldStatus, UserRequestStatus newStatus)        {            if (oldStatus == UserRequestStatus.Unknown && newStatus == UserRequestStatus.Pending) return true;            if (oldStatus == UserRequestStatus.Pending && (newStatus == UserRequestStatus.Approved || newStatus == UserRequestStatus.CanceledBySharer || newStatus == UserRequestStatus.CanceledByBorrower || newStatus == UserRequestStatus.CanceledBySharerBeforePayment || newStatus == UserRequestStatus.CanceledByBorrowerBeforePayment || newStatus == UserRequestStatus.Declined)) return true;            if (oldStatus == UserRequestStatus.Approved && newStatus == UserRequestStatus.Paid) return true;            if (oldStatus == UserRequestStatus.Approved && (newStatus == UserRequestStatus.CanceledBySharer || newStatus == UserRequestStatus.CanceledByBorrower || newStatus == UserRequestStatus.CanceledBySharerBeforePayment || newStatus == UserRequestStatus.CanceledByBorrowerBeforePayment)) return true;            if ((oldStatus == UserRequestStatus.Released || oldStatus == UserRequestStatus.Returned) && (newStatus == UserRequestStatus.Closed || newStatus == UserRequestStatus.Dispute)) return true;            if ((oldStatus == UserRequestStatus.Paid || oldStatus == UserRequestStatus.Released || oldStatus == UserRequestStatus.Received) && newStatus == UserRequestStatus.Dispute) return true;            if (oldStatus == UserRequestStatus.Paid && (newStatus == UserRequestStatus.CanceledBySharer || newStatus == UserRequestStatus.CanceledByBorrower || newStatus == UserRequestStatus.CanceledBySharerBeforePayment || newStatus == UserRequestStatus.CanceledByBorrowerBeforePayment)) return true;            if (oldStatus == UserRequestStatus.Paid && newStatus == UserRequestStatus.Released) return true;
            //if ((oldStatus == UserRequestStatus.Released || oldStatus == UserRequestStatus.Closed) && newStatus == UserRequestStatus.Reviewing) return true;
            if ((oldStatus == UserRequestStatus.Late || oldStatus == UserRequestStatus.Damaged || oldStatus == UserRequestStatus.LateAndDamaged) && newStatus == UserRequestStatus.Dispute) return true;            if ((oldStatus == UserRequestStatus.Returned) && newStatus == UserRequestStatus.Dispute) return true;            return false;        }

        private bool UserRequestChangeStatus(int userId, int requestId, UserRequestStatus requestStatus, IUnitOfWork unitOfWork = null)
        {
            var result = false;
            try
            {
                Uow.Wrap(u =>
                {
                    var userRequest = _repGoodRequest.Get(requestId);
                    if (userRequest.UserId == userId)
                    {
                        if (CanChangeStatus((UserRequestStatus)userRequest.StatusId, requestStatus))
                        {
                            userRequest.StatusId = (int)requestStatus;
                            _repGoodRequest.SaveOrUpdateAudit(userRequest, userId);
                            result = true;
                        }
                    }
                },
                unitOfWork,
                LogSource.GoodRequestService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.GoodRequestService, string.Format("Change user request status fail. Ex: {0}.", ex));
            }
            return result;
        }




        public Result<PriceViewModel> CalculatePrice(int GoodId, DateTime StartDate, DateTime EndDate, double ShippingDistance, bool ApplyForDelivery, IUnitOfWork unitOfWork = null)
        {
            PriceViewModel model = new PriceViewModel();
            Result<PriceViewModel> result = new Result<PriceViewModel>(CreateResult.Error, model);
            try
            {
                Uow.Wrap(u =>
                {
                    var good = _repGood.Get(GoodId);
                    if (good != null)
                    {
                        model.GoodId = GoodId;
                        model.StartDate = StartDate;
                        model.EndDate = EndDate;
                        model.Days = (model.EndDate - model.StartDate).Days;
                        //model.Days = (model.EndDate - model.StartDate).Days + 1;
                        model.ShippingDistance = ShippingDistance * 2;
                        model.Price = good.Price;
                        model.PricePerWeek = good.PricePerWeek;
                        model.PricePerMonth = good.PricePerMonth;
                        if ((good.Price == 0 && model.Days < 7) || (good.Price == 0 && good.PricePerWeek == 0 && model.Days < 30))
                        {
                            result.Message = "Rent period is too small";
                        }
                        else
                        {
                            var settingsDataService = Ioc.Get<ISettingsDataService>();
                            model.CustomerServiceFee = settingsDataService.GetBorrowerPaymentTransactionCommision();
                            model.CustomerCharity = settingsDataService.GetCharity();
                            model.SharerServiceFee = settingsDataService.GetSharerPaymentTransactionCommision();
                            model.SharerCharity = settingsDataService.GetCharity();
                            model.DiliveryPrice = settingsDataService.GetDiliveryPrice();
                            model.PerDayCost = model.Price;
                            if (model.Days >= 7)
                            {
                                model.PerDayCost = model.PricePerWeek > 0 ? model.PricePerWeek / 7 :
                                                        model.Price;
                            }
                            //if (model.Days >= 30)
                            //{
                            //    model.PerDayCost = model.PricePerMonth > 0 ? model.PricePerMonth / 30 :
                            //                        model.PricePerWeek > 0 ? model.PricePerWeek / 7 :
                            //                        model.Price;
                            //}
                            model.PerDayCost = Math.Round(model.PerDayCost, 2);

                            if (model.PricePerWeek <= 0)
                            {
                                model.PricePerWeek = model.PerDayCost * 7;
                            }
                            model.DaysCost = model.Days == 7 ? model.PricePerWeek :
                                             Math.Round(model.Days * model.PerDayCost, 2);



                            model.CustomerCharityCost = Math.Round(model.CustomerCharity * model.DaysCost, 2);
                            model.CustomerServiceFeeCost = Math.Round(model.CustomerServiceFee * model.DaysCost, 2);
                            model.SharerCharityCost = Math.Round(model.SharerCharity * model.DaysCost, 2);
                            model.SharerServiceFeeCost = Math.Round(model.SharerServiceFee * model.DaysCost, 2);
                            if (model.CustomerServiceFeeCost < 2)
                            {
                                model.CustomerServiceFeeCost = 2;
                            }
                            //if (model.SharerServiceFeeCost < 2)                            //{                            //    model.SharerServiceFeeCost = 2;                            //}
                            if (model.ShippingDistance <= 3.33 && ApplyForDelivery)
                            {
                                model.DiliveryCost = 5;
                            }
                            else
                            {
                                model.DiliveryCost = model.ShippingDistance > 0 ? Math.Round(model.ShippingDistance * model.DiliveryPrice, 2) : 0;
                            }
                            model.CustomerCost += model.DaysCost;
                            model.CustomerCost += model.CustomerCharityCost;
                            model.CustomerCost += model.CustomerServiceFeeCost;
                            model.CustomerCost += ApplyForDelivery == true ? model.DiliveryCost : 0;
                            model.SharerCost += model.DaysCost;
                            model.SharerCost -= model.SharerCharityCost;
                            model.SharerCost -= model.SharerServiceFeeCost;
                            model.SharerCost += ApplyForDelivery == true ? model.DiliveryCost : 0;
                            model.CustomerCost = Math.Round(model.CustomerCost, 2);
                            model.SharerCost = Math.Round(model.SharerCost, 2);
                            result.CreateResult = CreateResult.Success;
                        }
                    }
                },
                unitOfWork,
                LogSource.GoodRequestService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogException(LogSource.GoodRequestService, string.Format("Fail to price calculation. Ex: {0}.", ex), ex);
            }
            return result;
        }

        public Result<PriceViewModel> CalculatePriceOnDiscount(int GoodId, DateTime StartDate, DateTime EndDate, double ShippingDistance, bool ApplyForDelivery)
        {
            PriceViewModel model = new PriceViewModel();
            Result<PriceViewModel> result = new Result<PriceViewModel>(CreateResult.Error, model);
            try
            {

                var good = _repGood.Get(GoodId);
                if (good != null)
                {
                    model.GoodId = GoodId;
                    model.StartDate = StartDate;
                    model.EndDate = EndDate;
                    model.Days = (model.EndDate - model.StartDate).Days;
                    model.ShippingDistance = ShippingDistance * 2;
                    model.Price = good.Price;
                    model.PricePerWeek = good.PricePerWeek;
                    model.PricePerMonth = good.PricePerMonth;
                    if ((good.Price == 0 && model.Days < 7) || (good.Price == 0 && good.PricePerWeek == 0 && model.Days < 30))
                    {
                        result.Message = "Rent period is too small";
                    }
                    else
                    {
                        var settingsDataService = Ioc.Get<ISettingsDataService>();
                        model.CustomerServiceFee = settingsDataService.GetBorrowerPaymentTransactionCommision();
                        model.CustomerCharity = settingsDataService.GetCharity();
                        model.SharerServiceFee = settingsDataService.GetSharerPaymentTransactionCommision();
                        model.SharerCharity = settingsDataService.GetCharity();
                        model.DiliveryPrice = settingsDataService.GetDiliveryPrice();
                        model.PerDayCost = model.Price;
                        if (model.Days >= 7)
                        {
                            model.PerDayCost = model.PricePerWeek > 0 ? model.PricePerWeek / 7 :
                                                    model.Price;
                        }
                        //if (model.Days >= 30)
                        //{
                        //    model.PerDayCost = model.PricePerMonth > 0 ? model.PricePerMonth / 30 :
                        //                        model.PricePerWeek > 0 ? model.PricePerWeek / 7 :
                        //                        model.Price;
                        //}
                        model.PerDayCost = Math.Round(model.PerDayCost, 2);

                        if (model.PricePerWeek <= 0)
                        {
                            model.PricePerWeek = model.PerDayCost * 7;
                        }
                        model.DaysCost = model.Days == 7 ? model.PricePerWeek :
                                         Math.Round(model.Days * model.PerDayCost, 2);



                        model.CustomerCharityCost = Math.Round(model.CustomerCharity * model.DaysCost, 2);
                        model.CustomerServiceFeeCost = Math.Round(model.CustomerServiceFee * model.DaysCost, 2);
                        model.SharerCharityCost = Math.Round(model.SharerCharity * model.DaysCost, 2);
                        model.SharerServiceFeeCost = Math.Round(model.SharerServiceFee * model.DaysCost, 2);
                        if (model.CustomerServiceFeeCost < 2)                        {                            model.CustomerServiceFeeCost = 2;                        }
                        if (model.ShippingDistance <= 3.33 && ApplyForDelivery)
                        {
                            model.DiliveryCost = 5;
                        }
                        else
                        {
                            model.DiliveryCost = model.ShippingDistance > 0 ? Math.Round(model.ShippingDistance * model.DiliveryPrice, 2) : 0;
                        }
                        model.CustomerCost += model.DaysCost;
                        model.CustomerCost += model.CustomerCharityCost;
                        model.CustomerCost += model.CustomerServiceFeeCost;
                        model.CustomerCost += ApplyForDelivery == true ? model.DiliveryCost : 0;
                        model.SharerCost += model.DaysCost;
                        model.SharerCost -= model.SharerCharityCost;
                        model.SharerCost -= model.SharerServiceFeeCost;
                        model.SharerCost += ApplyForDelivery == true ? model.DiliveryCost : 0;
                        model.CustomerCost = Math.Round(model.CustomerCost, 2);
                        model.SharerCost = Math.Round(model.SharerCost, 2);
                        result.CreateResult = CreateResult.Success;
                    }
                }

            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogException(LogSource.GoodRequestService, string.Format("Fail to price calculation. Ex: {0}.", ex), ex);
            }
            return result;
        }

        public Result<RequestViewModel> BaseGetGoodRequest(int userId, RequestModel requestModel, IUnitOfWork unitOfWork = null)
        {
            var result = new Result<RequestViewModel>(CreateResult.Error, new RequestViewModel());
            try
            {
                Uow.Wrap(u =>
                {
                    var userGood = _repUserGood.Table.FirstOrDefault(p => p.GoodId == requestModel.GoodId);
                    if (userGood != null)
                    {
                        var good = _repGood.Get(requestModel.GoodId);
                        if (good != null)
                        {
                            var settingsDataService = Ioc.Get<ISettingsDataService>();
                            result.Obj.UserId = userGood.User.Id;
                            result.Obj.GoodId = good.Id;
                            result.Obj.FirstName = userGood.User.FirstName;
                            result.Obj.UserName = userGood.User.FullName;
                            result.Obj.GoodName = good.Name;
                            result.Obj.GoodDescription = good.Description;
                            result.Obj.ShippingAddress = requestModel.ShippingAddress;
                            result.Obj.ApplyForDelivery = requestModel.ApplyForDelivery;
                            var calculatePriceResult = CalculatePrice(
                                                    GoodId: requestModel.GoodId,
                                                    StartDate: requestModel.StartDate,
                                                    EndDate: requestModel.EndDate,
                                                    ShippingDistance: requestModel.ShippingDistance,
                                                    ApplyForDelivery: requestModel.ApplyForDelivery,
                                                    unitOfWork: u
                                                 );


                            //Sanjeev
                            if (calculatePriceResult.CreateResult == CreateResult.Success)
                            {
                                result.Obj.StartDate = calculatePriceResult.Obj.StartDate;
                                result.Obj.EndDate = calculatePriceResult.Obj.EndDate;
                                result.Obj.Price = calculatePriceResult.Obj.Price;
                                result.Obj.PricePerWeek = calculatePriceResult.Obj.PricePerWeek;
                                result.Obj.PricePerMonth = calculatePriceResult.Obj.PricePerMonth;
                                result.Obj.Days = calculatePriceResult.Obj.Days;
                                result.Obj.PerDayCost = calculatePriceResult.Obj.PerDayCost;
                                result.Obj.DaysCost = calculatePriceResult.Obj.DaysCost;
                                result.Obj.CharityCost = calculatePriceResult.Obj.CustomerCharityCost;
                                result.Obj.ServiceFeeCost = calculatePriceResult.Obj.CustomerServiceFeeCost;
                                result.Obj.DiliveryCost = calculatePriceResult.Obj.DiliveryCost;
                                if (requestModel.CouponCode != null && requestModel.CouponDiscount != 0)
                                {
                                    result.Obj.CouponDiscount = requestModel.CouponDiscount;
                                    result.Obj.Cost = calculatePriceResult.Obj.CustomerCost - requestModel.CouponDiscount;
                                }
                                else
                                {
                                    result.Obj.Cost = calculatePriceResult.Obj.CustomerCost;
                                }

                                result.Obj.Charity = calculatePriceResult.Obj.CustomerCharity;
                                result.Obj.ServiceFee = calculatePriceResult.Obj.CustomerServiceFee;
                                result.Obj.ShippingDistance = calculatePriceResult.Obj.ShippingDistance;
                                result.Obj.DiliveryPrice = calculatePriceResult.Obj.DiliveryPrice;
                                result.Obj.GoodImageUrl = good.GoodImages.ImageUrlNormal(0, ImageFolder.Good.ToString(), false);
                                result.Obj.UserImageUrl = userGood.User.UserImages.MainImageUrlThumb(false);
                                result.Obj.StartTime = requestModel.StartTime;                                result.Obj.EndTime = requestModel.EndTime;
                                result.CreateResult = CreateResult.Success;
                            }
                        }
                    }
                },
                unitOfWork,
                LogSource.GoodRequestService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.GoodRequestService, string.Format("Get good request fail. Ex: {0}.", ex));
            }
            return result;
        }
        public Result<GoodRequest> SaveGoodRequest(int userId, RequestViewModel requestModel)
        {
            var result = new Result<GoodRequest>(CreateResult.Error, new GoodRequest());
            try
            {
                Uow.Wrap(u =>
                {
                    var good = _repGood.Get(requestModel.GoodId);
                    if (good != null)
                    {
                        var userGood = _repUserGood.Table.FirstOrDefault(p => p.GoodId == good.Id && p.User.Id != userId);
                        if (userGood != null)
                        {
                            var calculatePriceResult = CalculatePrice(
                                                   GoodId: requestModel.GoodId,
                                                   StartDate: requestModel.StartDate,
                                                   EndDate: requestModel.EndDate,
                                                   ShippingDistance: requestModel.ShippingDistance,
                                                   ApplyForDelivery: requestModel.ApplyForDelivery,
                                                   unitOfWork: u
                                                );
                            if (calculatePriceResult.CreateResult == CreateResult.Success)
                            {
                                var goodRequest = new GoodRequest
                                {
                                    UserId = userId,
                                    GoodId = requestModel.GoodId,
                                    StatusId = (int)UserRequestStatus.Pending,
                                    Price = calculatePriceResult.Obj.PerDayCost,
                                    Days = calculatePriceResult.Obj.Days,
                                    DaysCost = calculatePriceResult.Obj.DaysCost,
                                    CustomerCost = calculatePriceResult.Obj.CustomerCost,
                                    CustomerServiceFee = calculatePriceResult.Obj.CustomerServiceFee,
                                    CustomerServiceFeeCost = calculatePriceResult.Obj.CustomerServiceFeeCost,
                                    CustomerCharity = calculatePriceResult.Obj.CustomerCharity,
                                    CustomerCharityCost = calculatePriceResult.Obj.CustomerCharityCost,
                                    SharerCost = calculatePriceResult.Obj.SharerCost,
                                    SharerServiceFee = calculatePriceResult.Obj.SharerServiceFee,
                                    SharerServiceFeeCost = calculatePriceResult.Obj.SharerServiceFeeCost,
                                    SharerCharity = calculatePriceResult.Obj.SharerCharity,
                                    SharerCharityCost = calculatePriceResult.Obj.SharerCharityCost,
                                    DiliveryCost = calculatePriceResult.Obj.DiliveryCost,
                                    ShippingDistance = calculatePriceResult.Obj.ShippingDistance,
                                    DiliveryPrice = calculatePriceResult.Obj.DiliveryPrice,
                                    SecurityDeposit = requestModel.Deposit,
                                    DiscountAmount = requestModel.CouponDiscount,
                                    CouponCode = requestModel.CouponCode,
                                    IsUsedCoupon = requestModel.IsUsedCoupon,
                                };
                                var savedGoodRequest = _repGoodRequest.SaveOrUpdateAudit(goodRequest, userId);
                                var goodBooking = new GoodBooking
                                {
                                    GoodRequestId = savedGoodRequest.Id,
                                    StartDate = requestModel.StartDate,
                                    EndDate = requestModel.EndDate,
                                    ShippingAddress = requestModel.ShippingAddress,
                                    ApplyForDelivery = requestModel.ApplyForDelivery,
                                    StartTime = requestModel.StartTime,                                    EndTime = requestModel.EndTime
                                };
                                var savedGoodBooking = _repGoodBooking.SaveOrUpdateAudit(goodBooking, userId);
                                savedGoodRequest.Good = good;
                                savedGoodRequest.GoodBooking = savedGoodBooking;
                                savedGoodRequest.Good.UserGood = userGood;
                                result.Obj = savedGoodRequest;
                                result.CreateResult = CreateResult.Success;
                            }
                            else
                            {
                                result.Message = calculatePriceResult.Message;
                            }
                        }
                    }
                    Ioc.Get<IDbLogger>().LogMessage(LogSource.GoodRequestService, string.Format("Trying to save good request"));
                },
                null,
                LogSource.GoodRequestService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.GoodRequestService, string.Format("Save good request fail. Ex: {0}.", ex));
            }
            return result;
        }
        public bool CanSeekerCancel(int statusId)
        {
            try
            {

                UserRequestStatus status = (UserRequestStatus)statusId;
                if (status == UserRequestStatus.Pending || status == UserRequestStatus.Approved || status == UserRequestStatus.Paid) return true;
                return false;
                //if (statusId == (int) UserRequestStatus.Declined) return false;
                //if (statusId == (int) UserRequestStatus.Approved ||)
                //if ((statusId != (int)UserRequestStatus.Canceled &&
                //    startDate > DateTime.Now.Date) && statusId != (int)UserRequestStatus.Declined)
                //    return true;
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.GoodRequestService, string.Format("Seeker cancel the request status fail. Ex: {0}.", ex));
            }
            return false;
        }
        public bool CanSharerCancel(int statusId, DateTime createDate)
        {
            try
            {

                if (statusId != (int)UserRequestStatus.CanceledByBorrower &&
                    createDate.AddHours(24) >= DateTime.Now)
                    return true;
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.GoodRequestService, string.Format("Sharer cancel the request status fail. Ex: {0}.", ex));
            }
            return false;
        }
        public bool CanSharerStartDispute(int statusId, DateTime endDate)
        {
            try
            {

                DateTime disputeStartPeriod = endDate;
                DateTime disputeEndPeriod = disputeStartPeriod.AddHours(48);
                DateTime now = DateTime.Now;
                if ((statusId == (int)UserRequestStatus.Released || statusId == (int)UserRequestStatus.Returned) && now >= disputeStartPeriod && now <= disputeEndPeriod) return true;
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.GoodRequestService, string.Format(" Sharer start dispute fail. Ex: {0}.", ex));
            }
            return false;
        }

        public bool CanSeekerStartDispute(int statusId, DateTime startDate, DateTime endDate)
        {
            try
            {

                DateTime disputeStartPeriod = startDate;
                DateTime disputeEndPeriod = disputeStartPeriod.AddHours(24);
                DateTime now = DateTime.Now;
                if ((statusId == (int)UserRequestStatus.Released || statusId == (int)UserRequestStatus.Received || statusId == (int)UserRequestStatus.Paid)
                    && now >= disputeStartPeriod && now <= disputeEndPeriod)
                { return true; }


                if (statusId == (int)UserRequestStatus.Late || statusId == (int)UserRequestStatus.Damaged || statusId == (int)UserRequestStatus.LateAndDamaged)
                { return true; }
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.GoodRequestService, string.Format("Seeker start dispute status fail. Ex: {0}.", ex));
            }
            return false;
        }

        public bool CanSeekerReview(int userId, int goodRequestId, int statusId, DateTime endDate)
        {
            var result = false;
            try
            {
                Uow.Wrap(u =>
                {

                    //   DateTime reviewDate = DateTime.Now.AddHours(-24 - 72);
                    DateTime reviewDate = DateTime.Now;
                    if ((statusId == (int)UserRequestStatus.Reviewing || statusId == (int)UserRequestStatus.ReturnConfirmed || statusId == (int)UserRequestStatus.Closed) && (endDate <= reviewDate) && (reviewDate <= endDate.AddDays(7)))
                    {
                        var review = _repGood.TableFor<GoodBookingRank>().Where(r => r.ReviewerId == userId && r.SeekerId == userId && r.GoodRequestId == goodRequestId);
                        if (!review.Any()) result = true;
                    }
                },
               null,
               LogSource.GoodRequestService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.GoodRequestService, string.Format("Can seeker review get status fail. Ex: {0}.", ex));
            }
            return result;
        }
        public bool CanSharerReview(int userId, int goodRequestId, int statusId, DateTime endDate)
        {
            var result = false;
            try
            {
                Uow.Wrap(u =>
                {

                    //   DateTime reviewDate = DateTime.Now.AddHours(-24 - 72);
                    DateTime reviewDate = DateTime.Now;
                    if ((statusId == (int)UserRequestStatus.Reviewing || statusId == (int)UserRequestStatus.ReturnConfirmed || statusId == (int)UserRequestStatus.Closed) && (endDate <= reviewDate) && (reviewDate <= endDate.AddDays(7)))
                    {
                        var review = _repGood.TableFor<GoodBookingRank>().Where(r => r.ReviewerId == userId && r.SharerId == userId && r.GoodRequestId == goodRequestId);
                        if (!review.Any()) result = true;
                    }
                },
               null,
               LogSource.GoodRequestService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.GoodRequestService, string.Format("Can sharer review get status fail. Ex: {0}.", ex));
            }
            return result;
        }


        public bool NeedRefundAmount(int userId, int requestId)
        {
            var result = false;
            try
            {
                Uow.Wrap(u =>
                {
                    Ioc.Get<IDbLogger>().LogMessage(LogSource.GoodRequestService, string.Format("Trying to refund amount."));

                    var userRequest = _repGoodRequest.GetUserRequest(userId, requestId);
                    //need refund if now <= StartDate-24h
                    var startPeriodRefund = userRequest.GoodBooking.StartDate.AddHours(-24);
                    var now = DateTime.Now;
                    if (now <= startPeriodRefund && userRequest.StatusId == (int)UserRequestStatus.Paid)
                    {
                        result = true;
                    }
                },
                null,
                LogSource.GoodRequestService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>()
                    .LogWarning(LogSource.PayPalPaymentService, string.Format("Get refund amount fail. Ex: {0}.", ex));
            }
            return result;
        }
        public bool SharerStartDispute(int userId, int requestId)
        {
            try
            {
                Ioc.Get<IDbLogger>().LogMessage(LogSource.GoodRequestService, string.Format("Trying to sharer start dispute."));

                return UserRequestChangeStatus(userId, requestId, UserRequestStatus.Dispute);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.GoodRequestService, string.Format("Sharer start dispute fail. Ex: {0}.", ex));
            }
            return false;
        }
        public bool SeekerStartDispute(int userId, int requestId)
        {
            try
            {
                Ioc.Get<IDbLogger>().LogMessage(LogSource.GoodRequestService, string.Format("Trying to seeker start dispute."));

                return UserRequestChangeStatus(userId, requestId, UserRequestStatus.Dispute);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.GoodRequestService, string.Format("Seeker start dispute fail. Ex: {0}.", ex));
            }
            return false;
        }
        public Result<bool> SeekerLeavesReview(GoodRequestRankInsertModel reviewModel, int userId, QuickUrl quickUrl)
        {
            var result = new Result<bool>(Common.CreateResult.Error, false);
            GoodRequest userRequest = null;
            UserGood userGood = null;
            try
            {
                Uow.WrapWithResult(u =>
                {
                    Ioc.Get<IDbLogger>().LogMessage(LogSource.GoodRequestService, string.Format("Trying to seeker leave review."));

                    userRequest = _repGoodRequest.GetUserRequest(userId, reviewModel.GoodRequestId);
                    if (userRequest != null)
                    {
                        userGood = _repUserGood.Table.FirstOrDefault(p => p.GoodId == userRequest.GoodId);
                        if (userGood != null)
                        {
                            var rank = new GoodBookingRank()
                            {
                                GoodId = userRequest.Good.Id,
                                GoodRequestId = reviewModel.GoodRequestId,
                                Message = reviewModel.Message,
                                Rank = reviewModel.Rank,
                                ReviewerId = userId,
                                SeekerId = userId,
                                SharerId = userGood.UserId,
                                is_deleted = false
                            };
                            _repRank.SaveOrUpdateAudit(rank, userId);
                            return true;
                            //send email to sharer that seeker write review
                        }
                    }
                    return false;
                }, null, LogSource.GoodRequestService).OnSuccess(() =>
                {
                    _emailService.SendEmailReviewFromBorrower(userGood.User.Email,
                        userRequest.User.FirstName + " " +
                        userRequest.User.LastName,
                        userRequest.Good.Name, quickUrl.ReviewSharerAbsoluteUrl(reviewModel.GoodRequestId), userGood.UserId);
                    //if two review in DB send message to both
                    //var reviews = GetReviewsOfRequest(reviewModel.GoodRequestId, userId, userGood.UserId);
                    //if (reviews.SharersReview != null && reviews.SeekersReview != null)
                    //{
                    //    _emailService.SendEmailSharerReadReview(reviews.SharersReview.Reviewer.Email, reviews.SeekersReview.Reviewer.FirstName + " " + reviews.SeekersReview.Reviewer.LastName, reviews.SeekersReview.Message, userRequest.Good.Name, null);
                    //    _emailService.SendEmailSeekerReadReview(reviews.SeekersReview.Reviewer.Email, reviews.SharersReview.Reviewer.FirstName + " " + reviews.SharersReview.Reviewer.LastName, reviews.SharersReview.Message, userRequest.Good.Name, null);
                    //    //send emails
                    //}
                }).Run();
                // Uow.Wrap(u =>
                // {
                //     var userRequest = _repGoodRequest.GetUserRequest(userId, reviewModel.GoodRequestId);
                //     if (userRequest != null)
                //     {
                //         var userGood = _repUserGood.Table.FirstOrDefault(p => p.GoodId == userRequest.GoodId);
                //         if (userGood != null)
                //         {
                //             var rank = new GoodBookingRank()
                //             {
                //                 GoodId = userRequest.Good.Id,
                //                 GoodRequestId = reviewModel.GoodRequestId,
                //                 Message = reviewModel.Message,
                //                 Rank = reviewModel.Rank,
                //                 ReviewerId = userId,
                //                 SeekerId = userId,
                //                 SharerId = userGood.UserId,
                //             };
                //             _repRank.SaveOrUpdateAudit(rank, userId);
                //             //send email to sharer that seeker write review
                //             _emailService.SendEmailSeekerWroteReview(userGood.User.Email, userRequest.User.FirstName + " " + userRequest.User.LastName, userRequest.Good.Name, quickUrl.ReviewSharerAbsoluteUrl(reviewModel.GoodRequestId));
                //             //if two review in DB send message to both
                //             var reviews = GetReviewsOfRequest(reviewModel.GoodRequestId, userId, userGood.UserId);
                //             if (reviews.SharersReview != null && reviews.SeekersReview != null)
                //             {
                //                 _emailService.SendEmailSharerReadReview(reviews.SharersReview.Reviewer.Email, reviews.SeekersReview.Reviewer.FirstName + " " + reviews.SeekersReview.Reviewer.LastName, reviews.SeekersReview.Message, userRequest.Good.Name, null);
                //                 _emailService.SendEmailSeekerReadReview(reviews.SeekersReview.Reviewer.Email, reviews.SharersReview.Reviewer.FirstName + " " + reviews.SharersReview.Reviewer.LastName, reviews.SharersReview.Message, userRequest.Good.Name, null);
                //                 //send emails
                //             }
                //         }
                //     }
                // },
                //null,
                //LogSource.GoodRequestService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogError(LogSource.ImageProcessor, string.Format("Seeker leave review fail. Ex: {0}.", ex));
            }
            return result;
        }
        public Result<bool> SharerLeavesReview(GoodRequestRankInsertModel reviewModel, int userId, QuickUrl quickUrl)
        {
            var result = new Result<bool>(Common.CreateResult.Error, false);
            GoodRequest userRequest = null;
            UserGood userGood = null;
            try
            {
                Ioc.Get<IDbLogger>().LogMessage(LogSource.GoodRequestService, string.Format("Trying to sharer leave review."));

                Uow.WrapWithResult(u =>
                {
                    userRequest = _repGoodRequest.GetUserRequest(reviewModel.GoodRequestId);
                    if (userRequest != null)
                    {
                        userGood = _repUserGood.Table.FirstOrDefault(p => p.GoodId == userRequest.GoodId);
                        if (userGood != null)
                        {
                            var rank = new GoodBookingRank()
                            {
                                GoodId = userRequest.Good.Id,
                                GoodRequestId = reviewModel.GoodRequestId,
                                Message = reviewModel.Message,
                                Rank = reviewModel.Rank,
                                ReviewerId = userId,
                                SeekerId = userRequest.UserId,
                                SharerId = userId,
                                is_deleted = false
                            };
                            _repRank.SaveOrUpdateAudit(rank, userId);
                            return true;
                        }
                    }
                    return false;
                }, null, LogSource.GoodRequestService).OnSuccess(() =>
                {
                    _emailService.SendEmailReviewFromSharer(userRequest.User.Email,
                        userGood.User.FirstName + " " + userGood.User.LastName,
                        userRequest.Good.Name, quickUrl.ReviewSeekerAbsoluteUrl(reviewModel.GoodRequestId), userRequest.UserId);
                    //if two review in DB send message to both
                    //var reviews = GetReviewsOfRequest(reviewModel.GoodRequestId, userId, userGood.UserId);
                    //if (reviews.SharersReview != null && reviews.SeekersReview != null)
                    //{
                    //    _emailService.SendEmailSharerReadReview(reviews.SharersReview.Reviewer.Email,
                    //        reviews.SeekersReview.Reviewer.FirstName + " " + reviews.SeekersReview.Reviewer.LastName,
                    //        reviews.SeekersReview.Message, userRequest.Good.Name, null);

                    //    _emailService.SendEmailSeekerReadReview(reviews.SeekersReview.Reviewer.Email,
                    //        reviews.SharersReview.Reviewer.FirstName + " " + reviews.SharersReview.Reviewer.LastName,
                    //        reviews.SharersReview.Message, userRequest.Good.Name, null);
                    //    //send emails
                    //}
                }).Run();

                // Uow.Wrap(u =>
                // {
                //     var userRequest = _repGoodRequest.GetUserRequest(reviewModel.GoodRequestId);
                //     if (userRequest != null)
                //     {
                //         var userGood = _repUserGood.Table.FirstOrDefault(p => p.GoodId == userRequest.GoodId);
                //         if (userGood != null)
                //         {
                //             var rank = new GoodBookingRank()
                //             {
                //                 GoodId = userRequest.Good.Id,
                //                 GoodRequestId = reviewModel.GoodRequestId,
                //                 Message = reviewModel.Message,
                //                 Rank = reviewModel.Rank,
                //                 ReviewerId = userId,
                //                 SeekerId = userRequest.UserId,
                //                 SharerId = userId,
                //             };
                //          var res=   _repRank.SaveOrUpdateAudit(rank, userId);
                //             //send email to seekers that sharer write review
                //             _emailService.SendEmailSharerWroteReview(userRequest.User.Email, userGood.User.FirstName + " " + userGood.User.LastName, userRequest.Good.Name, quickUrl.ReviewSeekerAbsoluteUrl(reviewModel.GoodRequestId));
                //             //if two review in DB send message to both
                //             var reviews = GetReviewsOfRequest(reviewModel.GoodRequestId, userId, userGood.UserId);
                //             if (reviews.SharersReview != null && reviews.SeekersReview != null)
                //             {
                //                 _emailService.SendEmailSharerReadReview(reviews.SharersReview.Reviewer.Email, reviews.SeekersReview.Reviewer.FirstName + " " + reviews.SeekersReview.Reviewer.LastName, reviews.SeekersReview.Message, userRequest.Good.Name, null);
                //                 _emailService.SendEmailSeekerReadReview(reviews.SeekersReview.Reviewer.Email, reviews.SharersReview.Reviewer.FirstName + " " + reviews.SharersReview.Reviewer.LastName, reviews.SharersReview.Message, userRequest.Good.Name, null);
                //                 //send emails
                //             }
                //             //if two review in DB send message to both
                //         }
                //     }
                // },
                //null,
                //LogSource.GoodRequestService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogError(LogSource.ImageProcessor, string.Format("Leave review fail. Ex: {0}.", ex));
            }
            return result;
        }
        public virtual Result<IList<ViewModels.Models.Impl.ApeekPayout>> GetGoodRequestForPayout()
        {
            return null;
        }
        public virtual Result<List<AppekPaymentVoid>> GetTransactionForVoid()
        {
            return null;
        }
        public virtual Result<List<ReviewEmailViewModel>> GetBookingForReview()
        {
            return null;
        }
        private GoodRequestReviews GetReviewsOfRequest(int goodRequestId, int seekerId, int sharerId)
        {
            var result = new GoodRequestReviews() { GoodRequestId = goodRequestId };
            try
            {
                Uow.Wrap(u =>
                {
                    result.SeekersReview =
                        _repRank.Table.SingleOrDefault(
                            r => r.GoodRequestId == goodRequestId && r.SeekerId == seekerId && r.ReviewerId == seekerId);
                    result.SharersReview =
                        _repRank.Table.SingleOrDefault(
                            r => r.GoodRequestId == goodRequestId && r.SharerId == sharerId && r.ReviewerId == sharerId);
                }, null, LogSource.GoodRequestService);
            }
            catch (Exception ex)
            {
            }
            return result;
        }

        public bool UpdateGoodRequest(GoodRequest request)
        {
            var result = false;
            try
            {
                Uow.Wrap(u =>
                {
                    _repGoodRequest.Update(request);
                    result = true;

                }, null, LogSource.GoodRequestService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.GoodRequestService, string.Format("Change user request status fail. Ex: {0}.", ex));
            }
            return result;
        }
        public bool SaveDisputeDetail(RequestChangeStatusViewModel model, int UserId, IUnitOfWork unitOfWork = null)
        {
            var result = false;
            try
            {
                Uow.Wrap(u =>
                {
                    Ioc.Get<IDbLogger>().LogMessage(LogSource.GoodRequestService, string.Format("Trying to dispute save."));

                    Disputes dispute = new Disputes();
                    dispute.RequestId = model.Id;
                    dispute.DisputeCreatedBy = UserId;
                    dispute.LastStatus = model.StatusId;
                    dispute.Reason = model.ReasonId;
                    dispute.Description = model.Message;
                    dispute.CreateDate = DateTime.Now;
                    dispute.ModDate = DateTime.Now;
                    dispute.CreateBy = UserId;
                    dispute.ModBy = UserId;
                    _repDisputes.Save(dispute);
                    result = true;
                },
                unitOfWork,
                LogSource.GoodRequestService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.GoodRequestService, string.Format("Dispute save failed. Ex: {0}.", ex));
            }
            return result;
        }
        public bool CheckCouponForcurrentUserToSendThankYouTemplate(string couponCode, int currentUserId, int requestId)
        {
            var result = false;
            try
            {
                Uow.Wrap(u =>
                {

                        int usedcoupan = _repGoodRequest.Table.Where(x =>  x.UserId == currentUserId && x.IsUsedCoupon).Count();
                        if (usedcoupan > 0)
                        {
                            result = true;
                        }
                        else
                        {
                            result = false;
                        }
                }, null, LogSource.GoodRequestService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.GoodRequestService, string.Format("Check coupon code to user request for once status fail. Ex: {0}.", ex));
            }
            return result;
        }

        public bool UpdateGoodRequestForCoupon(int currentUserId, int requestId)
        {
            var result = false;
            try
            {
                Uow.Wrap(u =>
                {
                    var goodRequest = _repGoodRequest.GetUserRequest(currentUserId, requestId);
                 
                    if (goodRequest != null)
                    {
                        goodRequest.IsUsedCoupon = true;
                        _repGoodRequest.Update(goodRequest);
                    }

                }, null, LogSource.GoodRequestService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.GoodRequestService, string.Format("Update good user request for once status fail. Ex: {0}.", ex));
            }
            return result;
        }

        public bool AddCancelledRequest(int userId, int requestId)
        {
            var result = false;
            try
            {
                Uow.Wrap(u =>
                {
                    Ioc.Get<IDbLogger>().LogMessage(LogSource.GoodRequestService, string.Format("Trying to add cancelled request"));
                    CancelledRequest cancelledRequest = new CancelledRequest();
                    cancelledRequest.UserId = userId;
                    cancelledRequest.RequestId = requestId;
                    cancelledRequest.CreateBy = userId;
                    cancelledRequest.CreateDate = DateTime.Now;
                    cancelledRequest.ModBy = userId;
                    cancelledRequest.ModDate = DateTime.Now;

                    _repositoryCancelledRequest.Save(cancelledRequest);
                    result = true;


                },
                null,
                LogSource.GoodRequestService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.GoodRequestService, string.Format("Failed to add cancelled request:- .", ex));
            }
            return result;
        }
    }
}
