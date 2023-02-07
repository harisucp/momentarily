using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Apeek.Common;
using Apeek.Common.Configuration;
using Apeek.Common.Definitions;
using Apeek.Common.Extensions;
using Apeek.Common.HttpContextImpl;
using Apeek.Common.Logger;
using Apeek.Common.Models;
using Apeek.Core.Services;
using Apeek.Core.Services.Impl;
using Apeek.Entities.Constants;
using Apeek.Entities.Entities;
using Apeek.NH.DataAccessLayer.DataAccess;
using Apeek.NH.Repository.Common;
using Apeek.NH.Repository.Repositories;
using Apeek.ViewModels.Mappers;
using Apeek.ViewModels.Mappers.Imp;
using Apeek.ViewModels.Models;
using Apeek.ViewModels.Models.Impl;
using Momentarily.Common.Definitions;
using Momentarily.Entities.Entities;
using Momentarily.ViewModels.Models;


namespace Momentarily.UI.Service.Services.Impl
{
    public class MomentarilyItemDataService : GoodService<MomentarilyItem, MomentarilyItemMapViewModel>, IMomentarilyItemDataService
    {
        private readonly IRepositoryUser _repUser;
        private readonly IRepositoryGoodPropertyValueDefinition _repGoodPropertyValueDefinition;
        private readonly IRepositoryGoodRequest _repGoodRequest;
        private readonly IRepositoryGoodBookingRank _repRank;
        private readonly IUserDataService<MomentarilyItem> _userService;
        private readonly IRepositoryGoodBooking _repositoryGoodBooking;
        private readonly IRepositoryFAQ _repositoryFAQ;
        private readonly IRepositoryGlobalCode _repositoryGlobalCode;
        private readonly IRepositoryUserCoupon _repositoryUserCoupon;
        private readonly IMomentarilyGoodRequestService _goodRequestService;
        private readonly IRepositoryGoodImg _repositoryGoodImg;
        private readonly ISendMessageService _sendMessageService;
        private readonly IRepositoryGoodImg _repGoodImg;
        private readonly IRepositoryDisputes _repositoryDisputes;        private readonly IRepositoryFinalFeedback _repositoryFinalFeedback;        private readonly IRepositoryResolvedDisputeDetail _repositoryResolvedDisputeDetail;
        private readonly IRepositoryChatTopics _repositoryChatTopics;
        private readonly IRepositoryChatQuestions _repositoryChatQuestions;
        private readonly IRepositoryChatAnswers _repositoryChatAnswers;
        private readonly IRepositoryCancelledRequest _repositoryCancelledRequest;

        public MomentarilyItemDataService(IRepositoryUserGood repUserGood, IRepositoryUser repUser,
            IRepositoryGoodCategory repGoodCategory, IRepositoryCategory repCategory, IRepositoryGood<MomentarilyItem, MomentarilyItemMapViewModel> repGood,
            IRepositoryGoodPropertyValue repGoodPropertyValue,
            IRepositoryGoodPropertyValueDefinition repGoodPropertyValueDefinition,
            IMomentarilyImageDataService imageDataService, IRepositoryGoodLocation repGoodLocation,
            IRepositoryAudit<GoodStartEndDate> repGoodStartEndDate, IRepositoryGoodRequest repGoodRequest, IRepositoryGoodBookingRank repRank,
            IGoodShareDateRepository goodShareDateRepository,
            IRepositoryGoodStartDateEndDate repositoryGoodStartDateEndDate, IRepositoryGoodBooking repositoryGoodBooking,
            IRepositoryGlobalCode repositoryGlobalCode, IRepositoryUserCoupon repositoryUserCoupon, IMomentarilyGoodRequestService goodRequestService,
            IRepositoryFAQ repositoryFAQ, IRepositoryGoodImg repositoryGoodImg, ISendMessageService sendMessageService, IRepositoryGoodImg repGoodImg, IRepositoryDisputes repositoryDisputes, 
            IRepositoryFinalFeedback repositoryFinalFeedback, IRepositoryResolvedDisputeDetail repositoryResolvedDisputeDetail, IRepositoryChatTopics repositoryChatTopics,
            IRepositoryChatQuestions repositoryChatQuestions, IRepositoryChatAnswers repositoryChatAnswers, IRepositoryCancelledRequest repositoryCancelledRequest)
            : base(repGood, repGoodPropertyValue, repUserGood, repGoodCategory, repCategory, imageDataService, repGoodLocation, repGoodStartEndDate, goodShareDateRepository, repositoryGoodStartDateEndDate, repGoodImg)
        {
            _repUser = repUser;
            _repGoodPropertyValueDefinition = repGoodPropertyValueDefinition;
            _repGoodRequest = repGoodRequest;
            _repRank = repRank;
            _userService = Ioc.Get<IUserDataService<MomentarilyItem>>();
            _repositoryGoodBooking = repositoryGoodBooking;
            _repositoryFAQ = repositoryFAQ;
            _repositoryGlobalCode = repositoryGlobalCode;
            _repositoryUserCoupon = repositoryUserCoupon;
            _goodRequestService = goodRequestService;
            _repositoryGoodImg = repositoryGoodImg;
            _sendMessageService = sendMessageService;
            _repGoodImg = repGoodImg;
            _repositoryDisputes = repositoryDisputes;            _repositoryFinalFeedback = repositoryFinalFeedback;            _repositoryResolvedDisputeDetail = repositoryResolvedDisputeDetail;
            _repositoryChatTopics = repositoryChatTopics;
            _repositoryChatQuestions = repositoryChatQuestions;
            _repositoryChatAnswers = repositoryChatAnswers;
            _repositoryCancelledRequest = repositoryCancelledRequest;
        }

        public Result<FilteredGoodsModel> GetFilteredItems(MomentarilyItemSearchModel searchModel)
        {
            var result = new Result<FilteredGoodsModel>(CreateResult.Error, new FilteredGoodsModel());
            try
            {
                Uow.Wrap(u =>
                {
                    if (!searchModel.SearchByMap)
                        searchModel = SetRectangleCoordinates(searchModel);
                    var filterQuery = new StringBuilder();
                    filterQuery.AppendLine(
                        string.Format(@"DECLARE @propId int = (SELECT TOP 1 dbo.d_good_property.[type_id]  
						               FROM dbo.d_good_property
						               WHERE dbo.d_good_property.name = N'{0}' )", MomentarilyItemProperties.MomentarilyItemType));
                    filterQuery.AppendLine(
                        string.Format(@"DECLARE @propLocationId int = (SELECT TOP 1 dbo.d_good_property.[type_id]  
						               FROM dbo.d_good_property
						               WHERE dbo.d_good_property.name = N'{0}' )", MomentarilyItemProperties.MomentarilyItemLocation));
                    if (searchModel.RentPeriod == RentPeriod.Week)
                    {
                        filterQuery.AppendLine(
                           string.Format(@"SELECT DISTINCT 
                                            uG.user_id              AS UserId, 
                                            g.id                    AS GoodId, 
                                            g.name                  AS Name, 
                                            Round((case when g.rent_period_day ='0' then   g.price_per_week/7 else g.price  end),2)       AS Price,  
                                            g.price_per_week        AS PricePerWeek,
				                            g.price_per_month	    AS PricePerMonth,
                                            g.rent_period_day	    AS RentPeriodDay,                                            g.rent_period_week	    AS RentPeriodWeek,
                                            gL.latitude             AS Latitude, 
                                            gL.longitude            AS Longitude, 
                                            uI.file_name            AS UserImageFileName, 
                                            gI.file_name            AS GoodImageFileName,
                                            ISNULL(r.ReviewCount,0) AS ReviewCount, 
                                            ISNULL(r.Rank,0)        AS Rank, 
                                            gPvLocation.value       AS Location,
											CASE WHEN  gr.good_id IS NULL THEN 0 ELSE COUNT (*) over (PARTITION by gr.good_id) END AS RentsCount
                                       FROM dbo.c_good AS g 
                                       JOIN dbo.c_user_good AS uG ON g.id = uG.good_id 
                                       JOIN dbo.c_good_category AS c ON g.id = c.good_id 
                                       JOIN dbo.c_good_location AS gL ON g.id = gL.good_id 
                                       JOIN dbo.c_good_start_end_date AS gSEd ON g.id = gSEd.good_id 
                                       JOIN dbo.c_good_property_value AS gPv ON g.id = gPv.good_id 
                                       JOIN dbo.c_good_share_date as gsd on g.id=gsd.good_id
                                       JOIN dbo.c_good_property_value AS gPvLocation ON g.id = gPvLocation.good_id  and gPvLocation.good_property_id = @propLocationId
                                       LEFT JOIN (select R.UserId as UserId, COUNT(R.UserId) as ReviewCount, AVG(CAST(r.rank AS DECIMAL(10,2))) as Rank from (select b.id, b.rank, b.sharer_id as UserId from c_good_booking_rank as b
                                                            join c_good_booking_rank as b1 on b.good_request_id = b1.good_request_id
                                                            where b1.id<>b.id and b1.seeker_id=b.reviewer_id
                                                            union
                                                            select b.id, b.rank, b.seeker_id as UserId from c_good_booking_rank as b
                                                            join c_good_booking_rank as b1 on b.good_request_id = b1.good_request_id
                                                            where  b1.id<>b.id and b1.sharer_id=b.reviewer_id) as R
                                                            GROUP By R.UserId) as r ON ug.user_id = r.UserId
                                       LEFT JOIN dbo.c_user_img AS uI ON uG.user_id = uI.user_id AND uI.type = {0} 
                                       LEFT JOIN dbo.c_good_img AS gI ON g.id = gI.good_id AND gI.type = {1} AND gI.sequence = 0 
                                       LEFT JOIN dbo.c_good_request AS gr ON g.id = gr.good_id AND gr.status_id = {2}
                                        LEFT JOIN dbo.c_report_abuse AS abuse ON g.id = abuse.good_id",
                                       (int)ImageType.Thumb, (int)ImageType.Original, (int)UserRequestStatus.Paid));
                    }
                    else
                    {
                        filterQuery.AppendLine(
                            string.Format(@"SELECT DISTINCT 
                                            uG.user_id              AS UserId, 
                                            g.id                    AS GoodId, 
                                            g.name                  AS Name, 
                                            g.price                 AS Price, 
                                            g.price_per_week        AS PricePerWeek,
				                            g.price_per_month	    AS PricePerMonth,
                                            g.rent_period_day	    AS RentPeriodDay,                                            g.rent_period_week	    AS RentPeriodWeek,
                                            gL.latitude             AS Latitude, 
                                            gL.longitude            AS Longitude, 
                                            uI.file_name            AS UserImageFileName, 
                                            gI.file_name            AS GoodImageFileName,
                                            ISNULL(r.ReviewCount,0) AS ReviewCount, 
                                            ISNULL(r.Rank,0)        AS Rank, 
                                            gPvLocation.value       AS Location,
											CASE WHEN  gr.good_id IS NULL THEN 0 ELSE COUNT (*) over (PARTITION by gr.good_id) END AS RentsCount
                                       FROM dbo.c_good AS g 
                                       JOIN dbo.c_user_good AS uG ON g.id = uG.good_id 
                                       JOIN dbo.c_good_category AS c ON g.id = c.good_id 
                                       JOIN dbo.c_good_location AS gL ON g.id = gL.good_id 
                                       JOIN dbo.c_good_start_end_date AS gSEd ON g.id = gSEd.good_id 
                                       JOIN dbo.c_good_property_value AS gPv ON g.id = gPv.good_id 
                                       JOIN dbo.c_good_share_date as gsd on g.id=gsd.good_id
                                       JOIN dbo.c_good_property_value AS gPvLocation ON g.id = gPvLocation.good_id  and gPvLocation.good_property_id = @propLocationId
                                       LEFT JOIN (select R.UserId as UserId, COUNT(R.UserId) as ReviewCount, AVG(CAST(r.rank AS DECIMAL(10,2))) as Rank from (select b.id, b.rank, b.sharer_id as UserId from c_good_booking_rank as b
                                                            join c_good_booking_rank as b1 on b.good_request_id = b1.good_request_id
                                                            where b1.id<>b.id and b1.seeker_id=b.reviewer_id
                                                            union
                                                            select b.id, b.rank, b.seeker_id as UserId from c_good_booking_rank as b
                                                            join c_good_booking_rank as b1 on b.good_request_id = b1.good_request_id
                                                            where  b1.id<>b.id and b1.sharer_id=b.reviewer_id) as R
                                                            GROUP By R.UserId) as r ON ug.user_id = r.UserId
                                       LEFT JOIN dbo.c_user_img AS uI ON uG.user_id = uI.user_id AND uI.type = {0} 
                                       LEFT JOIN dbo.c_good_img AS gI ON g.id = gI.good_id AND gI.type = {1} AND gI.sequence = 0 
                                       LEFT JOIN dbo.c_good_request AS gr ON g.id = gr.good_id AND gr.status_id = {2}
                                       LEFT JOIN dbo.c_report_abuse AS abuse ON g.id = abuse.good_id",
                                (int)ImageType.Thumb, (int)ImageType.Original, (int)UserRequestStatus.Paid));
                    }


                    if (searchModel.SearchByMap)
                    {
                        filterQuery.AppendLine(
                            string.Format(@"WHERE {0} >= {3} AND {0} <= {2} AND {1} >= {5} AND {1} <= {4} ",
                                "gL.latitude", "gL.longitude",
                                searchModel.NeLatitude.ToString(CultureInfo.InvariantCulture),
                                searchModel.SwLatitude.ToString(CultureInfo.InvariantCulture),
                                searchModel.NeLongitude.ToString(CultureInfo.InvariantCulture),
                                searchModel.SwLongitude.ToString(CultureInfo.InvariantCulture)));
                    }
                    else
                    {
                        filterQuery.AppendLine(
                            string.Format(@"WHERE ({0} like '%'+ '{1}' AND {2}={3}",
                                "gPvLocation.Value", searchModel.Location,
                                "gPvLocation.good_property_id", "@propLocationId"));
                        //include results 10km around selected location
                        filterQuery.AppendLine(
                         string.Format(@"OR ((6371 * acos( cos( radians({0}) ) * cos( radians( {2} ) ) * cos( radians( {3} ) 
                            - radians({1}) ) + sin( radians({0}) ) * sin( radians( {2} ) )) < {4})) )",
                            "gL.latitude", "gL.longitude",
                            searchModel.Latitude.ToString(CultureInfo.InvariantCulture),
                            searchModel.Longitude.ToString(CultureInfo.InvariantCulture), searchModel.Radius));
                    }
                    if (searchModel.TypeId.HasValue)
                        filterQuery.Append(string.Format(@"AND gPv.property_value_definition_id = {0} ",
                            searchModel.TypeId));
                    if (searchModel.CategoryId.HasValue && searchModel.CategoryId != 12)
                        filterQuery.Append(string.Format(@"AND c.category_id = {0} ", searchModel.CategoryId));
                    //if (searchModel.DateStart.HasValue)
                    //    filterQuery.Append(string.Format(@"AND gSEd.date_start >= '{0}' ", searchModel.DateStart));
                    //if (searchModel.DateEnd.HasValue)
                    //    filterQuery.Append(string.Format(@"AND gSEd.date_end <= '{0}' ", searchModel.DateEnd));
                    if (searchModel.PriceFrom.HasValue)
                        filterQuery.Append(string.Format(@"AND g.price >= {0} ", searchModel.PriceFrom));
                    if (searchModel.PriceTo.HasValue)
                        filterQuery.Append(string.Format(@"AND g.price <= {0} ", searchModel.PriceTo));
                    filterQuery.Append("AND g.is_archive = 0 ");
                    filterQuery.Append("AND gPv.good_property_id = @propId ");
                    filterQuery.Append(string.Format(@" AND(g.NAME like '%{0}%'OR g.description like '%{0}%') ",
                        string.IsNullOrWhiteSpace(searchModel.Keyword) ? "" : searchModel.Keyword.Replace(" ", "%")));
                    if (searchModel.RentPeriod == RentPeriod.Any) filterQuery.Append(@" AND (g.rent_period_day = 1 OR g.rent_period_week = 1) ");
                    if (searchModel.RentPeriod == RentPeriod.Day) filterQuery.Append(@" AND g.rent_period_day = 1 ");
                    if (searchModel.RentPeriod == RentPeriod.Week) filterQuery.Append(@" AND g.rent_period_week = 1 ");
                    if (searchModel.RentPeriod == RentPeriod.Month) filterQuery.Append(@" AND g.rent_period_month = 1 ");
                    filterQuery.Append(@"AND convert(date,gsd.share_date)>=convert(date,getdate())");

                    filterQuery.Append(@"AND(abuse.global_code_id != 4 or abuse.global_code_id is null) ");

                    if (searchModel.SortBy == SortBy.PriceHighToLow || searchModel.SortBy == SortBy.PriceLowToHigh)
                    {
                        filterQuery.Append(string.Format(@" ORDER BY {0} {1}",
                            //searchModel.RentPeriod == RentPeriod.Week ? "g.price_per_week" : 
                            searchModel.RentPeriod == RentPeriod.Week ? "Price" :
                            searchModel.RentPeriod == RentPeriod.Month ? "g.price_per_month" : "g.price",
                            searchModel.SortBy == SortBy.PriceLowToHigh ? "asc" : "desc"));
                    }
                    else if (searchModel.SortBy == SortBy.LeastRented || searchModel.SortBy == SortBy.MostRented)
                    {
                        filterQuery.Append(string.Format(@" ORDER BY RentsCount {0}",
                            searchModel.SortBy == SortBy.LeastRented ? "asc" : "desc"));
                    }
                    else if (searchModel.SortBy == SortBy.LeastRated || searchModel.SortBy == SortBy.MostRated)
                    {
                        filterQuery.Append(string.Format(@" ORDER BY Rank {0}, ReviewCount {0}",
                            searchModel.SortBy == SortBy.LeastRated ? "asc" : "desc"));
                    }
                    //                    filterQuery.Append(string.Format(@"ORDER BY g.id
                    //                                       OFFSET (({0} - 1) * {1}) ROWS  
                    //                                       FETCH NEXT {1} ROWS ONLY;", searchModel.Page, searchModel.PageSize));
                    var users = _repUser.Table.Where(x => x.IsAdmin != true && (x.IsBlocked == true || x.IsRemoved == true)).Select(y => y.Id).ToList();
                    var goods = _repGood.GetItems(filterQuery.ToString());

                    foreach (var item in goods)
                    {
                        if (users.Contains(item.UserId))
                        {
                            goods = goods.Where(x => x.UserId != item.UserId).ToList();
                        }
                    }


                    foreach (var path in goods)
                    {
                        var filePath = HttpContextFactory.Current.Server.MapPath(
                            AppSettings.GetInstance().GoodImageLocalStoragePath + path.GoodImageFileName);
                        if (!File.Exists(filePath))
                        {
                            string dummyImage = "error-bg.png";
                            path.GoodImageUrl = dummyImage;
                        }

                    }
                    result.Obj.Count = goods.Count();
                    result.Obj.Goods = goods.Skip((searchModel.Page - 1) * searchModel.PageSize).Take(searchModel.PageSize).ToList();
                    result.CreateResult = CreateResult.Success;
                },
                    null,
                    LogSource.GoodService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogError(LogSource.GoodService, string.Format("Cannot load Momentarily Items: {0}", ex));
                //result = new Result<FilteredGoodsModel>(CreateResult.Error, new FilteredGoodsModel());
            }

            return result;
        }
        private MomentarilyItemSearchModel SetRectangleCoordinates(MomentarilyItemSearchModel searchModel)
        {
            try
            {
                var radiusToDegrees = searchModel.Radius * 0.009;
                searchModel.NeLatitude = searchModel.Latitude + radiusToDegrees;
                searchModel.SwLatitude = searchModel.Latitude - radiusToDegrees;
                searchModel.NeLongitude = searchModel.Longitude + (radiusToDegrees / Math.Cos(searchModel.Latitude * (Math.PI / 180)));
                searchModel.SwLongitude = searchModel.Longitude - (radiusToDegrees / Math.Cos(searchModel.Latitude * (Math.PI / 180)));
                return searchModel;
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogError(LogSource.GoodService, string.Format("Cannot set rectangle coordinates: {0}", ex));
            }
            return null;
        }
        //public List<ListMomentarilyItemViewModel> GetUsersItems(int userId)
        //{
        //    try
        //    {
        //        List<ListMomentarilyItemViewModel> items = null;
        //        Uow.Wrap(u =>
        //        {
        //            items = (from g in _repGood.Table
        //                     join ug in _repUserGood.Table on g.Id equals ug.GoodId
        //                     where ug.UserId == userId && g.IsArchive == false
        //                     select new ListMomentarilyItemViewModel
        //                     {
        //                         Id = g.Id,
        //                         Name = g.Name,
        //                         DailyPrice = g.Price,
        //                         WeeklyPrice = g.PricePerWeek,
        //                         MounthlyPrice = g.PricePerMonth,
        //                         BookingCount = _repGoodRequest.Table.Count(gr => gr.GoodId == g.Id),
        //                         CreateDate = g.CreateDate
        //                     })
        //                     .ToList();
        //            items.ForEach(i => i.GoodPropertyValues = (from gpv in _repGoodPropertyValue.Table
        //                                                       join gp in _repGoodPropertyValue.TableFor<GoodProperty>() on gpv.GoodPropertyId equals gp.Id
        //                                                       where gpv.GoodId == i.Id
        //                                                       select new { Key = gp.Name, Values = gpv }).ToDictionary(k => k.Key, k => k.Values));
        //        }, null, LogSource.GoodService);
        //        return items;
        //    }
        //    catch (Exception ex)
        //    {
        //        Ioc.Get<IDbLogger>().LogError(LogSource.GoodService, string.Format("Cannot load Momentarily Items: {0}", ex));
        //    }
        //    return new List<ListMomentarilyItemViewModel>();
        //}
        public List<ListMomentarilyItemViewModel> GetUsersItems(int userId)
        {
            try
            {
                List<ListMomentarilyItemViewModel> items = null;
                Uow.Wrap(u =>
                {
                    items = (from g in _repGood.Table
                             join ug in _repUserGood.Table on g.Id equals ug.GoodId
                             where ug.UserId == userId && g.IsArchive == false
                             select new ListMomentarilyItemViewModel
                             {
                                 Id = g.Id,
                                 Name = g.Name,
                                 DailyPrice = g.Price,
                                 WeeklyPrice = g.PricePerWeek,
                                 MounthlyPrice = g.PricePerMonth,
                                 Image = (from img in _repGoodImg.Table where img.GoodId == g.Id && img.Type == (int)ImageType.Original select img.FileName).FirstOrDefault(),
                                 BookingCount = _repGoodRequest.Table.Count(gr => gr.GoodId == g.Id),
                                 CreateDate = g.CreateDate
                             })
                    .ToList();
                    items.ForEach(i => i.GoodPropertyValues = (from gpv in _repGoodPropertyValue.Table
                                                               join gp in _repGoodPropertyValue.TableFor<GoodProperty>() on gpv.GoodPropertyId equals gp.Id
                                                               where gpv.GoodId == i.Id
                                                               select new { Key = gp.Name, Values = gpv }).ToDictionary(k => k.Key, k => k.Values));
                }, null, LogSource.GoodService);
                return items;
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogError(LogSource.GoodService, string.Format("Cannot load Momentarily Items: {0}", ex));
            }
            return new List<ListMomentarilyItemViewModel>();
        }
        public Result<MomentarilyItem> GetMyItem(int userId, int itemId)
        {
            try
            {
                var good = GetMyGood(userId, itemId);
                good.Obj.AddEmptyProperties();
                return good;
                //return GetMyGood(userId, itemId);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogError(LogSource.GoodService, string.Format("Cannot load Momentarily Item: {0}", ex));
            }
            return new Result<MomentarilyItem>(CreateResult.Error, new MomentarilyItem());
        }
        public Result<MomentarilyItem> SaveUserItem(MomentarilyItem item, int userId)
        {
            try
            {
                return SaveGood(item, userId);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogError(LogSource.GoodService, string.Format("Cannot save Momentarily Item: {0}", ex));
            }
            return new Result<MomentarilyItem>(CreateResult.Error, new MomentarilyItem());
        }
        public Result<MomentarilyItemMapViewModel> GetItem(int itemId)
        {
            var ownerid = 0;
            const int imageType = (int)ImageType.Thumb;
            const int imageSequence = 0;

            var result = new Result<MomentarilyItemMapViewModel>(CreateResult.Error, new MomentarilyItemMapViewModel());
            try
            {
                Uow.Wrap(uow =>
                {
                    var user = GetItemUser(itemId);
                    if (user != null)
                    {
                        result.Obj.User = EntityMapper<IUserMapper>.Mapper().Map(user, new UserViewModel());

                        var item = _repGood.Get(itemId);
                        if (item != null)
                        {
                            item.GoodPropertyValues = _repGoodPropertyValue.GetGoodProperties(itemId);
                            result.Obj = (MomentarilyItemMapViewModel)EntityMapper<IGoodMapper>.Mapper().Map(item, result.Obj);
                            result.Obj.MinimumRentPeriod = item.MinimumRentPeriod;
                            var seekersReview = _repRank.GetRankFromSeekers(user.Id).GroupBy(g => g.Id).Select(c => new { Id = c.Key, Rank = c.Average(r => r.Rank) }).ToList();
                            var sharersReview = _repRank.GetRankFromSharers(user.Id).GroupBy(g => g.Id).Select(c => new { Id = c.Key, Rank = c.Average(r => r.Rank) }).ToList();
                            var un = seekersReview.Union(sharersReview).ToList();
                            result.Obj.SeekersReviews = _userService.GetListReviews(_repRank.GetRankFromSeekers(user.Id));
                            result.Obj.ReviewCount = un.Count();
                            result.Obj.Rank = (decimal)un.Select(c => c.Rank).DefaultIfEmpty(0).Average();
                            //if (un != null)
                            //{
                            //    result.Obj.ReviewCount = reviews.ReviewCount;
                            //    result.Obj.Rank = (decimal)reviews.Rank;
                            //}
                            result.Obj.Location = item.Location;
                            result.Obj.PickUpLoaction = item.PickUpLocation;
                            result.Obj.Deposit = item.Deposit;
                            var type = _repGoodPropertyValueDefinition.Get(item.TypeId);
                            if (type != null)
                            {
                                result.Obj.Type = type.Value;
                            }
                            else
                            {
                                result.Obj.Type = GetGoodCategory(item.Id);
                            }

                            var userImage = user.UserImages.FirstOrDefault(i => i.Type == imageType);
                            if (userImage != null)
                            {
                                result.Obj.UserImageFileName = userImage.FileName;
                            }

                            var listingImages = item.GoodImages.ToList();
                            result.Obj.ListingImages = GetListingImages(listingImages);

                            var goodImage = listingImages.FirstOrDefault(i => i.Type == (int)ImageType.Original && i.Sequence == imageSequence);
                            if (goodImage != null)
                            {
                                result.Obj.GoodImageFileName = goodImage.FileName;
                            }

                            var date = DateTime.Now.ToString("yyyy-MM-dd");
                            DateTime dateNew = Convert.ToDateTime(date);

                            result.Obj.GoodShareDates = _goodShareDateRepository.Table.Where(x => x.GoodId == item.Id && x.ShareDate >= dateNew).OrderBy(x => x.ShareDate)
                                            .Select(x => x.ShareDate.ToString("MM.dd.yyyy"))
                                            .ToList();

                            result.Obj.GoodBookedDates = new List<string>();
                            result.Obj.GoodBookedDatesUntil = new List<string>();
                            var start_time = _goodShareDateRepository.Table.Where(x => x.GoodId == item.Id && x.ShareDate >= dateNew).Select(x => x.StartTime).FirstOrDefault();
                            var end_time = _goodShareDateRepository.Table.Where(x => x.GoodId == item.Id && x.ShareDate >= dateNew).Select(x => x.EndTime).FirstOrDefault();
                            if (start_time != null)
                                result.Obj.StartTime = start_time;                            else
                                result.Obj.StartTime = "09:00 AM";
                            if (end_time != null)
                                result.Obj.EndTime = end_time;                            else
                                result.Obj.EndTime = "06:00 PM";
                            // var enddateList = (dynamic)null;
                            var startDates = new List<string>();
                            var endDates = new List<string>();
                            var goodRequests = _repGoodRequest.Table.Where(x => x.GoodId == item.Id && x.GoodBooking.EndDate >= DateTime.Now).ToList();
                            foreach (GoodRequest request in goodRequests)
                            {
                                if (request.StatusId != (int)UserRequestStatus.Declined
                                && request.StatusId != (int)UserRequestStatus.CanceledByBorrower
                                && request.StatusId != (int)UserRequestStatus.CanceledBySharer
                                && request.StatusId != (int)UserRequestStatus.CanceledByBorrowerBeforePayment
                                && request.StatusId != (int)UserRequestStatus.CanceledBySharerBeforePayment)
                                {
                                    var dates = new List<DateTime>();
                                    var start = request.GoodBooking.StartDate > dateNew ? request.GoodBooking.StartDate : dateNew;
                                    var end = request.GoodBooking.EndDate; 

                                    for (var dt = start; dt <= end; dt = dt.AddDays(1))
                                    {
                                        dates.Add(dt);
                                    }

                                    result.Obj.GoodBookedDates.AddRange(dates.Select(x => x.ToString("MM.dd.yyyy")));
                                    result.Obj.GoodBookedDatesUntil.AddRange(dates.Select(x => x.ToString("MM.dd.yyyy")));
                                    startDates.Add(request.GoodBooking.StartDate.ToString("MM.dd.yyyy"));
                                    endDates.Add(request.GoodBooking.EndDate.ToString("MM.dd.yyyy"));
                                }
                            }

                            result.Obj.AllStartDates = startDates;
                            result.Obj.AllEndDates = endDates;
                            //foreach(var end in endDates)
                            //{
                            //    result.Obj.GoodBookedDates.Remove(end.ToString("MM.dd.yyyy"));
                            //}
                            //foreach (var start in startDates)
                            //{
                            //    result.Obj.GoodBookedDatesUntil.Remove(start.ToString("MM.dd.yyyy"));
                            //}

                            result.CreateResult = CreateResult.Success;
                            Ioc.Get<IDbLogger>().LogMessage(LogSource.GoodService, string.Format("Trying to get Momentarily Item"));
                        }
                        ownerid = (from usergood in _repUserGood.Table
                                   where usergood.GoodId == itemId
                                   select usergood.UserId).FirstOrDefault();

                    }
                }, null, LogSource.GoodService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogError(LogSource.GoodService, string.Format("Cannot get Momentarily Item: {0}", ex));
            }
            try
            {
                result.Obj.CancelledPercentage = 0;

                if (ownerid != null && ownerid > 0)
                {
                    var userprofile = _userService.GetPublicUserProfile(ownerid);
                    if (userprofile.CreateResult == CreateResult.Success && userprofile.Obj != null)
                    {
                        result.Obj.CancelledPercentage = userprofile.Obj.CancelledPercentage;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return result;
        }
        public string GetGoodCategory(int itemId)
        {
            var result = "";
            var fullResult = "";
            try
            {
                var currentGoodCategory = _repGoodCategory.Table.ToList().Where(x => x.GoodId == itemId);
                foreach (var item in currentGoodCategory)
                {
                    //int categoryid = _repGoodCategory.GetGoodCategoryId(item.CategoryId);
                    result = _repositoryCategory.GetCategory(item.CategoryId).Name;
                    fullResult += result + ",";
                }
                fullResult = fullResult.TrimEnd(',');

            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogError(LogSource.CategoryService, string.Format("Cannot Find Item's Category: {0}", ex));
            }
            return fullResult;
        }
        public bool ArchiveGood(int itemId, int userId)
        {
            var result = false;
            try
            {
                Uow.Wrap(uow =>
                {
                    var item = (from g in _repGood.Table
                                join ug in _repUserGood.Table on g.Id equals ug.GoodId
                                where ug.UserId == userId && g.Id == itemId
                                select g).FirstOrDefault();
                    if (item != null && item.IsArchive != true)
                    {
                        item.IsArchive = true;
                        _repGood.Update(item);
                        result = true;
                    }
                }, null, LogSource.GoodService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogError(LogSource.GoodService, string.Format("Cannot archive Momentarily item: {0}", ex));
            }
            return result;
        }
        public bool DeleteArchiveGood(int itemId)
        {
            var result = false;
            try
            {
                Uow.Wrap(uow =>
                {

                    var itemGood = (from g in _repGood.Table
                                    join ug in _repUserGood.Table on g.Id equals ug.GoodId
                                    join u in _repUser.Table on ug.UserId equals u.Id
                                    where g.Id == itemId && g.IsArchive != true
                                    select new { g, u }).FirstOrDefault();
                    if (itemGood != null)
                    {
                        itemGood.g.IsArchive = true;
                        _repGood.SaveOrUpdate(itemGood.g);
                        bool sendemail_deleteItem = _sendMessageService.SendTemporaryDeletedUserItemsMessage(itemGood.u.Email, itemGood.u.FirstName, itemGood.g.Name);
                        result = true;
                    }

                    #region Old Code
                    //var goodsharedateItem = (from g in _goodShareDateRepository.Table
                    //                         where g.GoodId == itemId
                    //                         select g).ToList();

                    //foreach (var item in goodsharedateItem)
                    //{
                    //    _goodShareDateRepository.Delete(item);
                    //}

                    //var goodstartenddate = (from g in _repositoryGoodStartDateEndDate.Table
                    //                        where g.GoodId == itemId
                    //                        select g).FirstOrDefault();
                    //_repositoryGoodStartDateEndDate.Delete(goodstartenddate);

                    //var goodpropertyValue = (from g in _repGoodPropertyValue.Table
                    //                         where g.GoodId == itemId
                    //                         select g).ToList();
                    //foreach (var itemValue in goodpropertyValue)
                    //{
                    //    _repGoodPropertyValue.Delete(itemValue);
                    //}

                    //var goodLocation = (from g in _repGoodLocation.Table
                    //                    where g.GoodId == itemId
                    //                    select g).FirstOrDefault();
                    //_repGoodLocation.Delete(goodLocation);

                    //var goodCategory = (from g in _repGoodCategory.Table
                    //                    where g.GoodId == itemId
                    //                    select g).ToList();
                    //foreach (var itemValue in goodCategory)
                    //{
                    //    _repGoodCategory.Delete(itemValue);
                    //}

                    //var itemGood = (from g in _repGood.Table
                    //                join ug in _repUserGood.Table on g.Id equals ug.GoodId
                    //                where g.Id == itemId
                    //                select g).FirstOrDefault();
                    //if (itemGood != null && itemGood.IsArchive != true)
                    //{
                    //    _repGood.Delete(itemGood);
                    //    result = true;
                    //}
                    #endregion


                }, null, LogSource.GoodService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogError(LogSource.GoodService, string.Format("Cannot archive Momentarily item: {0}", ex));
            }
            return result;
        }


        private User GetItemUser(int itemId)
        {
            return (from ug in _repUserGood.Table
                    join u in _repUser.Table on ug.UserId equals u.Id
                    where ug.GoodId == itemId
                    select u).FirstOrDefault();
        }
        private List<GoodImageViewModel> GetListingImages(IList<GoodImg> listingImages)
        {
            return (from normal in listingImages.Where(x => x.Type == (int)ImageType.Original)
                    join large in listingImages.Where(x => x.Type == (int)ImageType.Original) on normal.Sequence equals large.Sequence
                    select new GoodImageViewModel
                    {
                        LargeImageUrl = large.GoodImageUrl(),
                        NormalImageUrl = normal.GoodImageUrl()
                    }).ToList();
        }

        public List<MomentarilyItem> GetAvalableItem()
        {
            List<MomentarilyItem> result = null;
            List<MomentarilyItem> resultnew = new List<MomentarilyItem>();
            Uow.Wrap(uow =>
            {
                result = (from pv in _repGood.Table
                          join p in _repGood.TableFor<GoodShareDate>() on pv.Id equals p.GoodId
                          where p.ShareDate >= DateTime.Now
                          select pv).ToList();
                foreach (var item in result)
                {
                    if (item.Price <= 0)
                    {
                        if (item.PricePerWeek > 0)
                        {
                            item.Price = item.PricePerWeek / 7;
                        }
                    }

                    if (item.PricePerWeek <= 0)
                    {
                        if (item.Price > 0)
                        {
                            item.PricePerWeek = item.Price * 7;
                        }
                    }
                    int count = resultnew.Where(x => x.Id == item.Id).Count();
                    if (count > 0)
                    {
                        continue;
                    }
                    resultnew.Add(item);
                }
            }, null, LogSource.GoodService);
            return resultnew;
        }

        public List<MostRentedItems> GetTotalLoanedItemList()
        {
            List<MostRentedItems> result = null;
            Uow.Wrap(uow =>
            {
                result = (from pv in _repGood.Table
                          join p in _repGood.TableFor<GoodShareDate>() on pv.Id equals p.GoodId
                          where p.ShareDate >= DateTime.Now
                          select new MostRentedItems
                          {
                              Id = pv.Id,
                              ItemName = pv.Name,
                              Count = (from v in _repGoodRequest.Table where v.GoodId == pv.Id select v.Id).Count()
                          }
                         ).Distinct().ToList();
                result = result.Where(x => x.Count > 0).ToList();
            }, null, LogSource.GoodService);
            return result;
        }

        public List<MostRentedItems> GetMostRentedItem()
        {
            List<MostRentedItems> mostRentedItems = null;
            Uow.Wrap(uow =>
            {
                List<MostRentedItems> resultv = (from pv in _repGood.Table
                                                 join p in _repGood.TableFor<GoodShareDate>() on pv.Id equals p.GoodId
                                                 where p.ShareDate >= DateTime.Now
                                                 select new MostRentedItems
                                                 {
                                                     Id = pv.Id,
                                                     ItemName = pv.Name,
                                                     Count = (from v in _repGoodRequest.Table where v.GoodId == pv.Id select v.Id).Count()
                                                 }
                              ).Distinct().ToList();

                mostRentedItems = resultv.OrderByDescending(y => y.Count).Take(5).ToList();

            }, null, LogSource.GoodService);
            return mostRentedItems;
        }

        public List<MostRentedItems> GetTotalLoanedItemListForAdmin()
        {
            //List<MostRentedItems> mostRentedItems = null;
            List<MostRentedItems> totalloaneditemlist = null;
            Uow.Wrap(uow =>
            {
                List<MostRentedItems> resultv = (from pv in _repGood.Table
                                                 join req in _repGoodRequest.Table on pv.Id equals req.GoodId
                                                 join dates in _repositoryGoodBooking.Table on req.Id equals dates.GoodRequestId
                                                 where (req.StatusId == 2 || req.StatusId == 5 || req.StatusId == 7) && dates.StartDate <= DateTime.Now
                                                 select new MostRentedItems
                                                 {
                                                     Id = pv.Id,
                                                     ItemName = pv.Name,
                                                     StartDate = dates.StartDate,
                                                     Enddate = dates.EndDate,
                                                     Status = req.StatusId,
                                                     Count = (from v in _repGoodRequest.Table where v.GoodId == pv.Id && (v.StatusId == 2 || v.StatusId == 5 || v.StatusId == 7) select v.Id).Count()
                                                 }
                              ).Distinct().ToList();

                totalloaneditemlist = new List<MostRentedItems>();
                totalloaneditemlist = resultv.GroupBy(x => x.Id).Select(y => y.First()).ToList();
                totalloaneditemlist = totalloaneditemlist.Where(x => x.StartDate <= DateTime.Now && x.Count > 0).ToList();

                totalloaneditemlist = totalloaneditemlist.OrderByDescending(y => y.Count).ToList();

            }, null, LogSource.GoodService);
            return totalloaneditemlist;
        }
        public List<MomentarilyItem> GetNewItemsList()
        {
            List<MomentarilyItem> result = null;
            List<MomentarilyItem> resultnew = new List<MomentarilyItem>();

            Uow.Wrap(uow =>
            {
                result = (from pv in _repGood.Table
                          join p in _repGood.TableFor<GoodShareDate>() on pv.Id equals p.GoodId
                          //where p.ShareDate >= DateTime.Now.AddDays(-7)
                          select pv).ToList();
                foreach (var item in result)
                {
                    int count = resultnew.Where(x => x.Id == item.Id).Count();
                    if (count > 0)
                    {
                        continue;
                    }
                    if (item.Price <= 0)
                    {
                        if (item.PricePerWeek > 0)
                        {
                            item.Price = item.PricePerWeek / 7;
                        }
                    }

                    if (item.PricePerWeek <= 0)
                    {
                        if (item.Price > 0)
                        {
                            item.PricePerWeek = item.Price * 7;
                        }
                    }
                    item.Price = Math.Round(item.Price, 2);
                    item.PricePerWeek = Math.Round(item.PricePerWeek, 2);

                    item.GoodOwner = (from ug in _repUserGood.Table
                                      join usr in _repUser.Table
                                      on ug.UserId equals usr.Id
                                      where ug.GoodId == item.Id
                                      select usr).FirstOrDefault();
                    item.GoodImageslist = (from img in _repositoryGoodImg.Table
                                           where img.GoodId == item.Id
                                           select new GoodImg
                                           {
                                               Id = img.Id,
                                               GoodId = img.GoodId,
                                               UserId = img.UserId,
                                               Type = img.Type,
                                               Sequence = img.Sequence,
                                               FileName = img.FileName,
                                               Folder = img.Folder

                                           }).ToList();
                    resultnew.Add(item);
                }
            }, null, LogSource.GoodService);
            return resultnew;
        }

        public int GetTotalLoanedItem()
        {
            int TotalLanedItemCount = 0;
            Uow.Wrap(uow =>
            {

                List<MostRentedItems> resultv = (from pv in _repGood.Table
                                                 join p in _repGood.TableFor<GoodShareDate>() on pv.Id equals p.GoodId
                                                 where p.ShareDate >= DateTime.Now
                                                 select new MostRentedItems
                                                 {
                                                     Id = pv.Id,
                                                     ItemName = pv.Name,
                                                     Count = (from v in _repGoodRequest.Table where v.GoodId == pv.Id select v.Id).Count()
                                                 }
                              ).Distinct().ToList();

                TotalLanedItemCount = resultv.Sum(x => x.Count);

            }, null, LogSource.GoodService);
            return TotalLanedItemCount;
        }

        public UserDashboardViewModel GetTotalLoanedItemList(int? _userID)
        {
            UserDashboardViewModel obj = new UserDashboardViewModel();
            List<MostRentedItems> resultTotalLoanedItemList = null;

            Uow.Wrap(uow =>
            {

                resultTotalLoanedItemList = (from good in _repGood.Table
                                             join goodrequest in _repGoodRequest.Table on good.Id equals goodrequest.GoodId
                                             where good.CreateBy == _userID && goodrequest.ModBy == _userID
                                             && goodrequest.StatusId == 2
                                             select new MostRentedItems
                                             {
                                                 Id = good.Id,
                                                 ItemName = good.Name,
                                                 Count = (from goodrequest in _repGoodRequest.Table where goodrequest.GoodId == good.Id && goodrequest.StatusId == 2 select goodrequest.Id).Count()
                                             }).Distinct().ToList();
                obj.mostloanedItemsCount = resultTotalLoanedItemList.Sum(x => x.Count);
                obj.mostloanedItems = resultTotalLoanedItemList;
                /////////
                ///
                //list = (from pv in TableFor<GoodRequest>()
                //        join u in TableFor<User>() on pv.UserId equals u.Id
                //        where (pv.StatusId == 1 || pv.StatusId == 2 || pv.StatusId == 5)
                //        select u).ToList();
                //foreach (var item in list)
                //{
                //    int count = listNew.Where(x => x.Id == item.Id).Count();
                //    if (count > 0)
                //    {
                //        continue;
                //    }
                //    listNew.Add(item);
                //}





            }, null, LogSource.GoodService);
            return obj;
        }

        public List<MostRankingCategory> GetTopRankingofCategory()
        {
            List<MostRankingCategory> mostToprankingCategory = new List<MostRankingCategory>();
            Uow.Wrap(uow =>
            {
                var categories = (from category in _repositoryCategory.Table where category.Name != "Momentarily" && category.Name != "All Categories" select category).ToList();
                foreach (var category in categories)
                {
                    int itemCount = (from good in _repGood.Table
                                     join goodcategory in _repGoodCategory.Table
                                     on good.Id equals goodcategory.GoodId
                                     where goodcategory.CategoryId == category.Id
                                     select good.Id).Count();
                    mostToprankingCategory.Add(new MostRankingCategory
                    {
                        Id = category.Id,
                        Count = itemCount,
                        CategoryName = category.Name,

                    });
                }

                mostToprankingCategory = mostToprankingCategory.OrderByDescending(y => y.Count).ToList();

            }, null, LogSource.GoodService);
            return mostToprankingCategory;
        }

        public List<Categories> categoriesList()
        {
            List<Categories> CategoryList = new List<Categories>();
            Uow.Wrap(uow =>
            {
                var categories = (from category in _repositoryCategory.Table where category.Name != "Momentarily" && category.Name != "All Categories" select category).ToList();
                foreach (var item in categories)
                {
                    CategoryList.Add(new Categories { CategoryId = item.Id, CategoryName = item.Name });
                }
            }, null, LogSource.GoodService);
            return CategoryList;
        }

        public List<string> categoriesListbyGoodId(int goodid)
        {
            List<string> CategoryList = new List<string>();
            Uow.Wrap(uow =>
            {
                CategoryList = (from category in _repositoryCategory.Table
                                join goodcat in _repGoodCategory.Table on category.Id equals goodcat.CategoryId
                                where goodcat.GoodId == goodid
                                select category.Name).ToList();
            }, null, LogSource.GoodService);
            return CategoryList;
        }

        public List<AllTransactionReports> GetReportByCategory(List<AllTransactionReports> allTransactionReports, int CategoryId)
        {

            Uow.Wrap(uow =>
            {
                allTransactionReports = (from data in allTransactionReports
                                         join goodcat in _repGoodCategory.Table on data.GoodId equals goodcat.GoodId
                                         where goodcat.CategoryId == CategoryId
                                         select data).ToList();
            }, null, LogSource.GoodService);
            return allTransactionReports;
        }

        public int RatingToOwnerPerRequest(int requestid, int sharerid)
        {
            int rates = 0;
            Uow.Wrap(uow =>
            {
                rates = (from rate in _repRank.Table
                         where rate.GoodRequestId == requestid && rate.SharerId == sharerid && rate.ReviewerId != sharerid
                         select rate.Rank).FirstOrDefault();
            }, null, LogSource.GoodService);
            return rates;
        }

        public int RatingToBorrowerPerRequest(int requestid, int seekerId)
        {
            int rates = 0;
            Uow.Wrap(uow =>
            {
                rates = (from rate in _repRank.Table
                         where rate.GoodRequestId == requestid && rate.SeekerId == seekerId && rate.ReviewerId != seekerId
                         select rate.Rank).FirstOrDefault();
            }, null, LogSource.GoodService);
            return rates;
        }



        public List<MostRentedItems> gettotalborrowerslistbyUser(int? userId)
        {
            List<MostRentedItems> result = null;
            List<MostRentedItems> resultnew = new List<MostRentedItems>();

            Uow.Wrap(uow =>
            {
                result = (from pv in _repGoodRequest.Table
                          join gd in _repGood.Table on pv.GoodId equals gd.Id
                          join u in _repUser.Table on pv.UserId equals u.Id
                          where (pv.StatusId == 2 || pv.StatusId == 5) && pv.UserId == userId
                          select new MostRentedItems
                          {
                              Id = gd.Id,
                              ItemName = gd.Name,
                              UserId = pv.UserId,
                              Count = (from v in _repGoodRequest.Table where v.GoodId == gd.Id && v.UserId == userId select v.GoodId).Count()
                          }).ToList();
                resultnew = new List<MostRentedItems>();
                resultnew = result.GroupBy(x => x.Id).Select(y => y.First()).ToList();
                // resultnew = resultnew.Where(x => x.StartDate <= DateTime.Now && x.Count > 0).ToList();
                resultnew = resultnew.OrderByDescending(y => y.Count).ToList();

            }, null, LogSource.GoodService);
            return resultnew;
        }

        public List<MostRentedItems> TotalUserEarning(int? userId)
        {
            List<MostRentedItems> resultnew = new List<MostRentedItems>();

            Uow.Wrap(uow =>
            {
                var UsergoodIds = (from good in _repGood.Table where good.CreateBy == userId select good.Id).ToList();
                resultnew = (from good in _repGoodRequest.Table
                             where UsergoodIds.Contains(good.GoodId)
                             where (good.StatusId == 2 || good.StatusId == 5)
                             select new MostRentedItems
                             {
                                 UserId = good.UserId,
                                 Id = good.Good.Id,
                                 ItemName = good.Good.Name,
                                 Total = good.SharerCost
                             }).ToList();

            }, null, LogSource.GoodService);
            return resultnew;
        }

        public List<MostRentedItems> TotalUserEarningByMonth(int? userId)
        {
            List<MostRentedItems> resultnew = new List<MostRentedItems>();
            List<MostRentedItems> Data = new List<MostRentedItems>();

            Uow.Wrap(uow =>
            {
                var UsergoodIds = (from good in _repGood.Table where good.CreateBy == userId select good.Id).ToList();
                resultnew = (from goodReq in _repGoodRequest.Table
                             where UsergoodIds.Contains(goodReq.GoodId)
                             where (goodReq.StatusId == 2 || goodReq.StatusId == 5) && goodReq.CreateDate >= DateTime.Now.AddYears(-1)
                             select new MostRentedItems
                             {
                                 UserId = goodReq.UserId,
                                 ItemName = goodReq.Good.Name,
                                 Total = goodReq.SharerCost,
                                 Date = goodReq.CreateDate
                             }).ToList();

                if (resultnew != null && resultnew.Count > 0)
                {
                    for (int i = 1; i <= 12; i++)
                    {
                        string monthName = GetMonth_Name(i);

                        Data.Add(new MostRentedItems
                        {
                            Month = monthName,
                            Total = resultnew.Where(x => x.Date.Month == i).Sum(s => s.Total),
                        });
                    }
                }


            }, null, LogSource.GoodService);


            return Data;
        }

        public List<MostRentedItems> TotalUserSpentByMonth(int? userId)
        {
            List<MostRentedItems> resultnew = new List<MostRentedItems>();
            List<MostRentedItems> Data = new List<MostRentedItems>();


            Uow.Wrap(uow =>
            {
                resultnew = (from goodreq in _repGoodRequest.Table
                             join good in _repGood.Table
                             on goodreq.GoodId equals good.Id
                             where goodreq.CreateBy == userId && (goodreq.StatusId == 5 || goodreq.StatusId == 2) && goodreq.CreateDate >= DateTime.Now.AddYears(-1)
                             select new MostRentedItems
                             {

                                 Id = good.Id,
                                 ItemName = good.Name,
                                 Total = goodreq.CustomerCost,
                                 Date = goodreq.CreateDate
                             }).ToList();

                if (resultnew != null && resultnew.Count > 0)
                {
                    for (int i = 1; i <= 12; i++)
                    {
                        string monthName = GetMonth_Name(i);

                        Data.Add(new MostRentedItems
                        {
                            Month = monthName,
                            Total = resultnew.Where(x => x.Date.Month == i).Sum(s => s.Total),
                        });
                    }
                }


            }, null, LogSource.GoodService);
            return Data;
        }

        public List<MostRentedItems> TotalUserSpent(int? userId)
        {
            List<MostRentedItems> resultnew = new List<MostRentedItems>();

            Uow.Wrap(uow =>
            {
                resultnew = (from goodreq in _repGoodRequest.Table
                             join good in _repGood.Table
                             on goodreq.GoodId equals good.Id
                             where goodreq.CreateBy == userId && (goodreq.StatusId == 5 || goodreq.StatusId == 2)
                             select new MostRentedItems
                             {

                                 Id = good.Id,
                                 ItemName = good.Name,
                                 Total = goodreq.CustomerCost

                             }).ToList();

            }, null, LogSource.GoodService);
            return resultnew;
        }

        public List<ProductsList> GetProductList(DateTime startRentalDate, DateTime endRentalDate, string searchShareName, string searchBorrowerName, int categoryId, string ItemName, int amountRangeId)
        {
            List<ProductsList> Products = new List<ProductsList>();
            List<MomentarilyItem> momentarilyItems = new List<MomentarilyItem>();

            Uow.Wrap(uow =>
            {
                Products = GetProductFilter(startRentalDate, endRentalDate, searchShareName, searchBorrowerName, categoryId, ItemName, amountRangeId);
            }, null, LogSource.GoodService);
            return Products;
        }

        public List<sharers> GetUserList(DateTime startRentalDate, DateTime endRentalDate, string searchShareName, string searchBorrowerName, int categoryId, string ItemName, int amountRangeId)
        {
            List<MomentarilyItem> momentarilyItems = new List<MomentarilyItem>();
            List<sharers> Sharers = new List<sharers>();
            Uow.Wrap(uow =>
            {
                Sharers = GetSharerFilter(startRentalDate, endRentalDate, searchShareName, searchBorrowerName, categoryId, ItemName, amountRangeId);
            }, null, LogSource.GoodService);
            return Sharers;
        }

        public List<CategoriesList> GetCategoriesList(DateTime startRentalDate, DateTime endRentalDate, string searchShareName, string searchBorrowerName, int categoryId, string ItemName, int amountRangeId)
        {
            List<CategoriesList> Categories = new List<CategoriesList>();
            List<MomentarilyItem> momentarilyItems = new List<MomentarilyItem>();

            Uow.Wrap(uow =>
            {
                Categories = GetCategoryFilter(startRentalDate, endRentalDate, searchShareName, searchBorrowerName, categoryId, ItemName, amountRangeId);
            }, null, LogSource.GoodService);
            return Categories;
        }

        public List<Borrowers> GetBorrowerList(DateTime startRentalDate, DateTime endRentalDate, string searchShareName, string searchBorrowerName, int categoryId, string ItemName, int amountRangeId)
        {
            List<Borrowers> Categories = new List<Borrowers>();
            List<MomentarilyItem> momentarilyItems = new List<MomentarilyItem>();

            Uow.Wrap(uow =>
            {
                Categories = GetBorrowerFilter(startRentalDate, endRentalDate, searchShareName, searchBorrowerName, categoryId, ItemName, amountRangeId);
            }, null, LogSource.GoodService);
            return Categories;
        }

        public List<ProductsList> GetProductFilter(DateTime startRentalDate, DateTime endRentalDate, string searchShareName, string searchBorrowerName, int categoryId, string ItemName, int amountRangeId)
        {
            List<ProductsList> result = new List<ProductsList>();
            List<ProductsList> resultResponse = new List<ProductsList>();
            var allDataGoods = (from _good in _repGood.Table
                                join p in _goodShareDateRepository.Table
                                on _good.Id equals p.GoodId
                                //join cat in repGoodCategory.Table on good.Id equals cat.GoodId
                                select _good).Distinct().ToList();

            var getAllDatawithCategories = (from goodCat in allDataGoods
                                            join cat in _repGoodCategory.Table on goodCat.Id equals cat.GoodId
                                            select new
                                            {
                                                goodCat.Id,
                                                goodCat.Name,
                                                goodCat.CreateBy,
                                                goodCat.ModBy,
                                                goodCat.Description,
                                                goodCat.CreateDate,
                                                goodCat.ModDate,
                                                cat.CategoryId,
                                                goodCat.Price,
                                                goodCat.PricePerWeek,

                                            }).Distinct().ToList();
            getAllDatawithCategories = getAllDatawithCategories.GroupBy(x => x.Id).Select(y => y.First()).ToList();

            var newData = getAllDatawithCategories.Where(x => x.CreateDate.Date >= startRentalDate.Date && x.CreateDate.Date <= endRentalDate.Date).Distinct().ToList();

            var getUsersIds = newData.Select(x => x.CreateBy).Distinct().ToList();
            var getGoodIds = newData.Select(x => x.Id).ToList();


            if (searchShareName != "")
            {
                var sharerids = (from _user in _repUser.Table
                                 where (_user.FirstName.ToLower().Contains(searchShareName.ToLower())
                                 || _user.LastName.ToLower().Contains(searchShareName.ToLower()) || _user.FullName.ToLower() == searchShareName.ToLower())
                                 && getUsersIds.Contains(_user.Id)

                                 select _user.Id).ToList();
                newData = (from data in newData where sharerids.Contains(data.CreateBy) select data).ToList();

            }

            if (searchBorrowerName != "")
            {

                var listids = (from req in _repGoodRequest.Table
                               join user in _repUser.Table
                               on req.UserId equals user.Id
                               where (user.FirstName.ToLower().Contains(searchBorrowerName.ToLower())
                                   || user.LastName.ToLower().Contains(searchBorrowerName.ToLower()) || user.FullName.ToLower() == searchBorrowerName.ToLower())
                                   && getGoodIds.Contains(req.GoodId)
                               select req.GoodId).ToList();

                newData = (from data in newData where listids.Contains(data.Id) select data).ToList();
            }

            if (categoryId != 0)
            {

                newData = (from data in newData
                           join cat in _repositoryCategory.Table
                           on data.CategoryId equals cat.Id
                           where data.CategoryId == categoryId
                           select data).ToList();

            }

            if (ItemName != "")
            {

                newData = newData.Where(x => x.Name.ToLower().Contains(ItemName.ToLower())).ToList();

            }

            if (amountRangeId != 0)
            {
                double startRange = 0;
                double endrange = 0;
                if (amountRangeId == 1)
                {
                    startRange = 0; endrange = 50;
                }

                if (amountRangeId == 2)
                {
                    startRange = 50; endrange = 100;
                }

                if (amountRangeId == 3)
                {
                    startRange = 100; endrange = 200;
                }

                if (amountRangeId == 4)
                {
                    startRange = 200; endrange = 300;
                }
                if (amountRangeId == 5)
                {
                    startRange = 300; endrange = 400;
                }
                if (amountRangeId == 6)
                {
                    startRange = 400; endrange = 500;
                }
                if (amountRangeId == 7)
                {
                    startRange = 500; endrange = 1000;
                }
                if (amountRangeId == 8)
                {
                    newData = newData.Where(x => x.Price >= startRange).ToList();
                }

                newData = newData.Where(x => x.Price >= startRange && x.Price <= endrange).ToList();
            }


            result = (from res in newData
                      select new ProductsList
                      {
                          GoodId = res.Id,
                          GoodName = res.Name,
                          CategoryId = res.CategoryId,
                          CategoryName = (from cat in _repositoryCategory.Table where cat.Id == res.CategoryId select cat.Name).FirstOrDefault(),
                          GoodDescription = res.Description,
                          PricePerDay = res.Price,
                          PricePerWeek = res.PricePerWeek,
                          CreatedDate = res.CreateDate
                      }).ToList();

            if (result != null && result.Count > 0)
            {
                for (int i = 1; i <= 12; i++)
                {
                    string monthName = GetMonth_Name(i);

                    resultResponse.Add(new ProductsList
                    {
                        Month = monthName,
                        Count = result.Where(x => x.CreatedDate.Month == i).Count(),
                    });
                }

            }

            return resultResponse;
        }

        public List<CategoriesList> GetCategoryFilter(DateTime startRentalDate, DateTime endRentalDate, string searchShareName, string searchBorrowerName, int categoryId, string ItemName, int amountRangeId)
        {
            List<CategoriesList> resultNew = new List<CategoriesList>();
            List<CategoriesList> result = new List<CategoriesList>();
            List<CategoriesList> resultResponse = new List<CategoriesList>();

            if (categoryId != 0)
            {
                result = (from good in _repGood.Table
                          join goodCat in _repGoodCategory.Table
                          on good.Id equals goodCat.GoodId
                          where goodCat.CategoryId == categoryId
                          && good.CreateDate >= startRentalDate
                          && good.CreateDate <= endRentalDate
                          select new CategoriesList
                          {
                              GoodId = good.Id,
                              GoodName = good.Name,
                              CreatedDate = good.CreateDate,
                              GoodDescription = good.Description,
                              CategoryId = goodCat.CategoryId,
                              PricePerDay = good.Price,
                              PricePerWeek = good.PricePerWeek,
                              CreatedBy = good.CreateBy,
                              CategoryName = string.IsNullOrEmpty((from c in _repositoryCategory.Table where c.Id == goodCat.CategoryId select c.Name).FirstOrDefault()) ? "" : (from c in _repositoryCategory.Table where c.Id == goodCat.CategoryId select c.Name).FirstOrDefault(),
                              FullName = string.IsNullOrEmpty((from c in _repUser.Table where c.Id == good.CreateBy select c.FullName).FirstOrDefault()) ? "" : (from c in _repUser.Table where c.Id == good.CreateBy select c.FullName).FirstOrDefault(),
                              FirstName = string.IsNullOrEmpty((from c in _repUser.Table where c.Id == good.CreateBy select c.FirstName).FirstOrDefault()) ? "" : (from c in _repUser.Table where c.Id == good.CreateBy select c.FirstName).FirstOrDefault(),
                              LastName = string.IsNullOrEmpty((from c in _repUser.Table where c.Id == good.CreateBy select c.LastName).FirstOrDefault()) ? "" : (from c in _repUser.Table where c.Id == good.CreateBy select c.LastName).FirstOrDefault(),

                          }
                                     ).ToList();
            }
            else
            {
                result = (from good in _repGood.Table
                          join goodCat in _repGoodCategory.Table
                          on good.Id equals goodCat.GoodId
                          where good.CreateDate >= startRentalDate
                          && good.CreateDate <= endRentalDate
                          select new CategoriesList
                          {
                              GoodId = good.Id,
                              GoodName = good.Name,
                              CreatedDate = good.CreateDate,
                              GoodDescription = good.Description,
                              CategoryId = goodCat.CategoryId,
                              PricePerDay = good.Price,
                              PricePerWeek = good.PricePerWeek,
                              CreatedBy = good.CreateBy,
                              CategoryName = string.IsNullOrEmpty((from c in _repositoryCategory.Table where c.Id == goodCat.CategoryId select c.Name).FirstOrDefault()) ? "" : (from c in _repositoryCategory.Table where c.Id == goodCat.CategoryId select c.Name).FirstOrDefault(),
                              FullName = string.IsNullOrEmpty((from c in _repUser.Table where c.Id == good.CreateBy select c.FullName).FirstOrDefault()) ? "" : (from c in _repUser.Table where c.Id == good.CreateBy select c.FullName).FirstOrDefault(),
                              FirstName = string.IsNullOrEmpty((from c in _repUser.Table where c.Id == good.CreateBy select c.FirstName).FirstOrDefault()) ? "" : (from c in _repUser.Table where c.Id == good.CreateBy select c.FirstName).FirstOrDefault(),
                              LastName = string.IsNullOrEmpty((from c in _repUser.Table where c.Id == good.CreateBy select c.LastName).FirstOrDefault()) ? "" : (from c in _repUser.Table where c.Id == good.CreateBy select c.LastName).FirstOrDefault(),

                          }).ToList();
            }




            var getGoodIds = result.Select(x => x.GoodId).ToList();



            if (searchShareName != "")
            {
                var listids = (from req in _repGoodRequest.Table
                               join user in _repUser.Table
                               on req.UserId equals user.Id
                               where (user.FirstName.ToLower().Contains(searchBorrowerName.ToLower())
                                   || user.LastName.ToLower().Contains(searchBorrowerName.ToLower()) || user.FullName.ToLower() == searchBorrowerName.ToLower())
                                   && getGoodIds.Contains(req.GoodId)
                               select req.GoodId).ToList();

                result = (from data in result where listids.Contains(data.GoodId) select data).ToList();
            }

            if (searchBorrowerName != "")
            {

                var listids = (from req in _repGoodRequest.Table
                               join user in _repUser.Table
                               on req.UserId equals user.Id
                               where (user.FirstName.ToLower().Contains(searchBorrowerName.ToLower())
                                   || user.LastName.ToLower().Contains(searchBorrowerName.ToLower()) || user.FullName.ToLower() == searchBorrowerName.ToLower())
                                   && getGoodIds.Contains(req.GoodId)
                               select req.GoodId).ToList();

                result = (from data in result where listids.Contains(data.GoodId) select data).ToList();
            }
            if (ItemName != "")
            {

                result = result.Where(x => x.GoodName.ToLower().Contains(ItemName.ToLower())).ToList();

            }
            if (amountRangeId != 0)
            {
                List<double> range = getamountrange(amountRangeId);
                if (range[0] == range[1])
                {
                    result = result.Where(x => x.PricePerDay >= range[0]).ToList();
                }
                else
                {
                    result = result.Where(x => x.PricePerDay >= range[0] && x.PricePerDay <= range[1]).ToList();
                }
            }

            var categories = (from category in _repositoryCategory.Table where category.Name != "Momentarily" && category.Name != "All Categories" select category).ToList();
            foreach (var category in categories)
            {
                int itemCount = (from good in result
                                 where good.CategoryId == category.Id
                                 select good.GoodId).Count();
                resultResponse.Add(new CategoriesList
                {
                    CategoryId = category.Id,
                    Count = itemCount,
                    CategoryName = category.Name,

                });
            }
            resultResponse = resultResponse.OrderByDescending(x => x.Count).ToList();


            return resultResponse;
        }

        public List<sharers> GetSharerFilter(DateTime startRentalDate, DateTime endRentalDate, string searchShareName, string searchBorrowerName, int categoryId, string ItemName, int amountRangeId)
        {
            List<sharers> userlist = new List<sharers>();
            List<sharers> userlistResponse = new List<sharers>();

            var AllSharerlist = (from user in _repUser.Table
                                 join Good in _repGood.Table
                                 on user.Id equals Good.CreateBy
                                 where Good.CreateDate >= startRentalDate && Good.CreateDate <= endRentalDate
                                 select new sharers
                                 {
                                     UserId = user.Id,
                                     FirstName = user.FirstName,
                                     LastName = user.LastName,
                                     dayprice = Good.Price,
                                     weekprice = Good.PricePerWeek,
                                     GoodId = Good.Id,
                                     GoodName = Good.Name,
                                     CreatedDate = Good.CreateDate,
                                     FullName = string.IsNullOrEmpty(user.FullName.ToLower()) ? "" : user.FullName.ToLower()
                                 }).ToList();

            if (searchShareName != "")
            {
                AllSharerlist = AllSharerlist.Where(x => x.FirstName.ToLower().Contains(searchShareName)
                || x.LastName.ToLower().Contains(searchShareName)
                || x.FullName.ToLower().Contains(searchShareName.ToLower())
                ).ToList();


                //AllSharerlist = (from x in AllSharerlist
                //                 where x.FirstName.ToLower().Contains(searchShareName) || x.LastName.ToLower().Contains(searchShareName)
                //                 || string.IsNullOrEmpty(x.FullName.ToLower()) ? "" : x.FullName.ToLower() == searchShareName.ToLower()
                //                 ).ToList();
            }

            if (searchBorrowerName != "")
            {
                var borrowersgoods = (from req in _repGoodRequest.Table
                                      join user in _repUser.Table
                                      on req.UserId equals user.Id
                                      where user.FirstName.ToLower().Contains(searchBorrowerName.ToLower())
                                      || user.LastName.ToLower().Contains(searchBorrowerName.ToLower()) || user.FullName.ToLower().Contains(searchBorrowerName.ToLower())
                                      select req.GoodId).ToList();

                AllSharerlist = (from data in AllSharerlist where borrowersgoods.Contains(data.GoodId) select data).ToList();
            }


            if (ItemName != "")
            {
                AllSharerlist = AllSharerlist.Where(x => x.GoodName.ToLower().Contains(ItemName.ToLower())).ToList();
            }

            if (categoryId != 0)
            {
                var goodlist = (from cat in _repGoodCategory.Table where cat.CategoryId == categoryId select cat.GoodId).ToList();
                AllSharerlist = (from data in AllSharerlist where goodlist.Contains(data.GoodId) select data).ToList();
            }
            if (amountRangeId != 0)
            {
                List<double> range = getamountrange(amountRangeId);
                if (range[0] == range[1])
                {
                    AllSharerlist = AllSharerlist.Where(x => x.dayprice >= range[0]).ToList();
                }
                else
                {
                    AllSharerlist = AllSharerlist.Where(x => x.dayprice >= range[0] && x.dayprice <= range[1]).ToList();
                }
            }

            userlist = (from res in AllSharerlist
                        select new sharers
                        {
                            GoodId = res.GoodId,
                            GoodName = res.GoodName,
                            UserId = res.UserId,
                            FirstName = res.FirstName,
                            LastName = res.LastName,
                            CreatedDate = res.CreatedDate

                        }).ToList();

            if (userlist != null && userlist.Count > 0)
            {
                for (int i = 1; i <= 12; i++)
                {
                    string monthName = GetMonth_Name(i);

                    userlistResponse.Add(new sharers
                    {
                        Month = monthName,
                        Count = userlist.Where(x => x.CreatedDate.Month == i).Count(),
                    });
                }

            }

            return userlistResponse;
        }

        public List<Borrowers> GetBorrowerFilter(DateTime startRentalDate, DateTime endRentalDate, string searchShareName, string searchBorrowerName, int categoryId, string ItemName, int amountRangeId)
        {
            List<Borrowers> AllBorrowerlist = new List<Borrowers>();
            List<Borrowers> AllBorrowerlistResponse = new List<Borrowers>();


            AllBorrowerlist = (from user in _repUser.Table
                               join req in _repGoodRequest.Table
                               on user.Id equals req.UserId
                               join good in _repGood.Table on req.GoodId equals good.Id
                               where req.ModDate >= startRentalDate && req.ModDate <= endRentalDate
                               && (req.StatusId == 2 || req.StatusId == 5)
                               select new Borrowers
                               {
                                   UserId = user.Id,
                                   FirstName = user.FirstName != null ? user.FirstName : "",
                                   LastName = user.LastName != null ? user.LastName : "",
                                   FullName = user.FullName != null ? user.FullName : "",
                                   GoodId = good.Id,
                                   GoodName = good.Name,
                                   dayprice = good.Price,
                                   weekprice = good.PricePerWeek,
                                   GoodCreateddate = good.CreateDate
                               }).ToList();




            if (searchShareName != null && searchShareName != "")
            {
                var GoodIDs = (from user in _repUser.Table
                               join good in _repGood.Table on user.Id equals good.CreateBy
                               where user.FirstName.ToLower().Contains(searchShareName.ToLower()) ||
                               user.LastName.ToLower().Contains(searchShareName.ToLower()) ||
                               user.FullName.ToLower().Contains(searchShareName.ToLower())
                               select good.Id
                            ).ToList();

                AllBorrowerlist = (from data in AllBorrowerlist
                                   where
      GoodIDs.Contains(data.GoodId)
                                   select data).ToList();


            }

            if (searchBorrowerName != "")
            {
                AllBorrowerlist = AllBorrowerlist.Where(
                    x => x.FirstName.ToLower().Contains(searchBorrowerName.ToLower()) ||
                    x.LastName.ToLower().Contains(searchBorrowerName.ToLower()) ||
                    x.FullName.ToLower().Contains(searchBorrowerName.ToLower())
                    ).ToList();
            }


            if (ItemName != "")
            {
                AllBorrowerlist = AllBorrowerlist.Where(x => x.GoodName.ToLower().Contains(ItemName.ToLower())).ToList();
            }

            if (categoryId != 0)
            {
                var goodids = (from cat in _repGoodCategory.Table where cat.CategoryId == categoryId select cat.GoodId).ToList();
                AllBorrowerlist = (from data in AllBorrowerlist where goodids.Contains(data.GoodId) select data).ToList();
            }
            if (amountRangeId != 0)
            {
                List<double> range = getamountrange(amountRangeId);
                if (range[0] == range[1])
                {
                    AllBorrowerlist = AllBorrowerlist.Where(x => x.dayprice >= range[0]).ToList();
                }
                else
                {
                    AllBorrowerlist = AllBorrowerlist.Where(x => x.dayprice >= range[0] && x.dayprice <= range[1]).ToList();
                }
            }

            if (AllBorrowerlist != null && AllBorrowerlist.Count > 0)
            {
                for (int i = 1; i <= 12; i++)
                {
                    string monthName = GetMonth_Name(i);

                    AllBorrowerlistResponse.Add(new Borrowers
                    {
                        Month = monthName,
                        Count = AllBorrowerlist.Where(x => x.GoodCreateddate.Month == i).Count(),
                    });
                }

            }


            return AllBorrowerlistResponse;
        }

        public List<ProductsList> GetAllDataProductList(DateTime startRentalDate, DateTime endRentalDate, string searchShareName, string searchBorrowerName, int categoryId, string ItemName, int amountRangeId)
        {
            List<ProductsList> Products = new List<ProductsList>();

            Uow.Wrap(uow =>
            {
                Products = GetAllDataProductFilter(startRentalDate, endRentalDate, searchShareName, searchBorrowerName, categoryId, ItemName, amountRangeId);
            }, null, LogSource.GoodService);
            return Products;
        }
        public List<ProductsList> GetAllDataProductFilter(DateTime startRentalDate, DateTime endRentalDate, string searchShareName, string searchBorrowerName, int categoryId, string ItemName, int amountRangeId)
        {
            List<ProductsList> result = new List<ProductsList>();
            List<ProductsList> resultResponse = new List<ProductsList>();
            var allDataGoods = (from _good in _repGood.Table
                                join p in _goodShareDateRepository.Table
                                on _good.Id equals p.GoodId
                                //join cat in repGoodCategory.Table on good.Id equals cat.GoodId
                                select _good).Distinct().ToList();

            var getAllDatawithCategories = (from goodCat in allDataGoods
                                            join cat in _repGoodCategory.Table on goodCat.Id equals cat.GoodId
                                            select new
                                            {
                                                goodCat.Id,
                                                goodCat.Name,
                                                goodCat.CreateBy,
                                                goodCat.ModBy,
                                                goodCat.Description,
                                                goodCat.CreateDate,
                                                goodCat.ModDate,
                                                cat.CategoryId,
                                                goodCat.Price,
                                                goodCat.PricePerWeek,


                                            }).Distinct().ToList();
            getAllDatawithCategories = getAllDatawithCategories.GroupBy(x => x.Id).Select(y => y.First()).ToList();

            var newData = getAllDatawithCategories.Where(x => x.CreateDate.Date >= startRentalDate.Date && x.CreateDate.Date <= endRentalDate.Date).Distinct().ToList();

            var getUsersIds = newData.Select(x => x.CreateBy).Distinct().ToList();
            var getGoodIds = newData.Select(x => x.Id).ToList();


            if (searchShareName != "")
            {
                var sharerids = (from _user in _repUser.Table
                                 where (_user.FirstName.ToLower().Contains(searchShareName.ToLower())
                                 || _user.LastName.ToLower().Contains(searchShareName.ToLower()) || _user.FullName.ToLower() == searchShareName.ToLower())
                                 && getUsersIds.Contains(_user.Id)

                                 select _user.Id).ToList();
                newData = (from data in newData where sharerids.Contains(data.CreateBy) select data).ToList();

            }

            if (searchBorrowerName != "")
            {

                var listids = (from req in _repGoodRequest.Table
                               join user in _repUser.Table
                               on req.UserId equals user.Id
                               where (user.FirstName.ToLower().Contains(searchBorrowerName.ToLower())
                                   || user.LastName.ToLower().Contains(searchBorrowerName.ToLower()) || user.FullName.ToLower() == searchBorrowerName.ToLower())
                                   && getGoodIds.Contains(req.GoodId)
                               select req.GoodId).ToList();

                newData = (from data in newData where listids.Contains(data.Id) select data).ToList();
            }

            if (categoryId != 0)
            {

                newData = (from data in newData
                           join cat in _repositoryCategory.Table
                           on data.CategoryId equals cat.Id
                           where data.CategoryId == categoryId
                           select data).ToList();

            }

            if (ItemName != "")
            {

                newData = newData.Where(x => x.Name.ToLower().Contains(ItemName.ToLower())).ToList();

            }

            if (amountRangeId != 0)
            {
                double startRange = 0;
                double endrange = 0;
                if (amountRangeId == 1)
                {
                    startRange = 0; endrange = 50;
                }

                if (amountRangeId == 2)
                {
                    startRange = 50; endrange = 100;
                }

                if (amountRangeId == 3)
                {
                    startRange = 100; endrange = 200;
                }

                if (amountRangeId == 4)
                {
                    startRange = 200; endrange = 300;
                }
                if (amountRangeId == 5)
                {
                    startRange = 300; endrange = 400;
                }
                if (amountRangeId == 6)
                {
                    startRange = 400; endrange = 500;
                }
                if (amountRangeId == 7)
                {
                    startRange = 500; endrange = 1000;
                }
                if (amountRangeId == 8)
                {
                    newData = newData.Where(x => x.Price >= startRange).ToList();
                }

                newData = newData.Where(x => x.Price >= startRange && x.Price <= endrange).ToList();
            }


            result = (from res in newData
                      select new ProductsList
                      {
                          GoodId = res.Id,
                          GoodName = res.Name,
                          CategoryId = res.CategoryId,
                          CategoryName = (from cat in _repositoryCategory.Table where cat.Id == res.CategoryId select cat.Name).FirstOrDefault(),
                          GoodDescription = res.Description,
                          PricePerDay = res.Price,
                          PricePerWeek = res.PricePerWeek,
                          CreatedDate = res.CreateDate,
                          Month = OnlyGetMonthName(res.CreateDate.Month),
                          MonthId = res.CreateDate.Month

                      }).ToList();

            if (result != null && result.Count > 0)
            {
                resultResponse = result.OrderByDescending(x => x.MonthId).ToList();
            }

            return resultResponse;
        }
        public List<CategoriesList> GetAllDataCategoriesList(DateTime startRentalDate, DateTime endRentalDate, string searchShareName, string searchBorrowerName, int categoryId, string ItemName, int amountRangeId)
        {
            List<CategoriesList> Categories = new List<CategoriesList>();
            List<MomentarilyItem> momentarilyItems = new List<MomentarilyItem>();

            Uow.Wrap(uow =>
            {
                Categories = GetAllDataCategoryFilter(startRentalDate, endRentalDate, searchShareName, searchBorrowerName, categoryId, ItemName, amountRangeId);
            }, null, LogSource.GoodService);
            return Categories;
        }
        public List<CategoriesList> GetAllDataCategoryFilter(DateTime startRentalDate, DateTime endRentalDate, string searchShareName, string searchBorrowerName, int categoryId, string ItemName, int amountRangeId)
        {
            List<CategoriesList> resultNew = new List<CategoriesList>();
            List<CategoriesList> result = new List<CategoriesList>();
            List<CategoriesList> resultResponse = new List<CategoriesList>();

            if (categoryId != 0)
            {
                result = (from good in _repGood.Table
                          join goodCat in _repGoodCategory.Table
                          on good.Id equals goodCat.GoodId
                          where goodCat.CategoryId == categoryId
                          && good.CreateDate >= startRentalDate
                          && good.CreateDate <= endRentalDate
                          select new CategoriesList
                          {
                              GoodId = good.Id,
                              GoodName = good.Name,
                              CreatedDate = good.CreateDate,
                              GoodDescription = good.Description,
                              CategoryId = goodCat.CategoryId,
                              PricePerDay = good.Price,
                              PricePerWeek = good.PricePerWeek,
                              CreatedBy = good.CreateBy,
                              CategoryName = string.IsNullOrEmpty((from c in _repositoryCategory.Table where c.Id == goodCat.CategoryId select c.Name).FirstOrDefault()) ? "" : (from c in _repositoryCategory.Table where c.Id == goodCat.CategoryId select c.Name).FirstOrDefault(),
                              FullName = string.IsNullOrEmpty((from c in _repUser.Table where c.Id == good.CreateBy select c.FullName).FirstOrDefault()) ? "" : (from c in _repUser.Table where c.Id == good.CreateBy select c.FullName).FirstOrDefault(),
                              FirstName = string.IsNullOrEmpty((from c in _repUser.Table where c.Id == good.CreateBy select c.FirstName).FirstOrDefault()) ? "" : (from c in _repUser.Table where c.Id == good.CreateBy select c.FirstName).FirstOrDefault(),
                              LastName = string.IsNullOrEmpty((from c in _repUser.Table where c.Id == good.CreateBy select c.LastName).FirstOrDefault()) ? "" : (from c in _repUser.Table where c.Id == good.CreateBy select c.LastName).FirstOrDefault(),

                          }
                                     ).ToList();
            }
            else
            {
                result = (from good in _repGood.Table
                          join goodCat in _repGoodCategory.Table
                          on good.Id equals goodCat.GoodId
                          where good.CreateDate >= startRentalDate
                          && good.CreateDate <= endRentalDate
                          select new CategoriesList
                          {
                              GoodId = good.Id,
                              GoodName = good.Name,
                              CreatedDate = good.CreateDate,
                              GoodDescription = good.Description,
                              CategoryId = goodCat.CategoryId,
                              PricePerDay = good.Price,
                              PricePerWeek = good.PricePerWeek,
                              CreatedBy = good.CreateBy,
                              CategoryName = string.IsNullOrEmpty((from c in _repositoryCategory.Table where c.Id == goodCat.CategoryId select c.Name).FirstOrDefault()) ? "" : (from c in _repositoryCategory.Table where c.Id == goodCat.CategoryId select c.Name).FirstOrDefault(),
                              FullName = string.IsNullOrEmpty((from c in _repUser.Table where c.Id == good.CreateBy select c.FullName).FirstOrDefault()) ? "" : (from c in _repUser.Table where c.Id == good.CreateBy select c.FullName).FirstOrDefault(),
                              FirstName = string.IsNullOrEmpty((from c in _repUser.Table where c.Id == good.CreateBy select c.FirstName).FirstOrDefault()) ? "" : (from c in _repUser.Table where c.Id == good.CreateBy select c.FirstName).FirstOrDefault(),
                              LastName = string.IsNullOrEmpty((from c in _repUser.Table where c.Id == good.CreateBy select c.LastName).FirstOrDefault()) ? "" : (from c in _repUser.Table where c.Id == good.CreateBy select c.LastName).FirstOrDefault(),

                          }).ToList();
            }




            var getGoodIds = result.Select(x => x.GoodId).ToList();



            if (searchShareName != "")
            {
                var listids = (from req in _repGoodRequest.Table
                               join user in _repUser.Table
                               on req.UserId equals user.Id
                               where (user.FirstName.ToLower().Contains(searchBorrowerName.ToLower())
                                   || user.LastName.ToLower().Contains(searchBorrowerName.ToLower()) || user.FullName.ToLower() == searchBorrowerName.ToLower())
                                   && getGoodIds.Contains(req.GoodId)
                               select req.GoodId).ToList();

                result = (from data in result where listids.Contains(data.GoodId) select data).ToList();
            }

            if (searchBorrowerName != "")
            {

                var listids = (from req in _repGoodRequest.Table
                               join user in _repUser.Table
                               on req.UserId equals user.Id
                               where (user.FirstName.ToLower().Contains(searchBorrowerName.ToLower())
                                   || user.LastName.ToLower().Contains(searchBorrowerName.ToLower()) || user.FullName.ToLower() == searchBorrowerName.ToLower())
                                   && getGoodIds.Contains(req.GoodId)
                               select req.GoodId).ToList();

                result = (from data in result where listids.Contains(data.GoodId) select data).ToList();
            }
            if (ItemName != "")
            {

                result = result.Where(x => x.GoodName.ToLower().Contains(ItemName.ToLower())).ToList();

            }
            if (amountRangeId != 0)
            {
                List<double> range = getamountrange(amountRangeId);
                if (range[0] == range[1])
                {
                    result = result.Where(x => x.PricePerDay >= range[0]).ToList();
                }
                else
                {
                    result = result.Where(x => x.PricePerDay >= range[0] && x.PricePerDay <= range[1]).ToList();
                }
            }

            //var categories = (from category in _repositoryCategory.Table where category.Name != "Momentarily" && category.Name != "All Categories" select category).ToList();
            //foreach (var category in categories)
            //{
            //    int itemCount = (from good in result
            //                     where good.CategoryId == category.Id
            //                     select good.GoodId).Count();
            //    resultResponse.Add(new CategoriesList
            //    {
            //        CategoryId = category.Id,
            //        Count = itemCount,
            //        CategoryName = category.Name,

            //    });
            //}
            resultResponse = result.OrderByDescending(x => x.CategoryId).ToList();


            return resultResponse;
        }
        public List<sharers> GetAllDataUserList(DateTime startRentalDate, DateTime endRentalDate, string searchShareName, string searchBorrowerName, int categoryId, string ItemName, int amountRangeId)
        {
            List<MomentarilyItem> momentarilyItems = new List<MomentarilyItem>();
            List<sharers> Sharers = new List<sharers>();
            Uow.Wrap(uow =>
            {
                Sharers = GetAllDataSharerFilter(startRentalDate, endRentalDate, searchShareName, searchBorrowerName, categoryId, ItemName, amountRangeId);
            }, null, LogSource.GoodService);
            return Sharers;
        }
        public List<sharers> GetAllDataSharerFilter(DateTime startRentalDate, DateTime endRentalDate, string searchShareName, string searchBorrowerName, int categoryId, string ItemName, int amountRangeId)
        {
            List<sharers> userlist = new List<sharers>();
            List<sharers> userlistResponse = new List<sharers>();

            var AllSharerlist = (from user in _repUser.Table
                                 join Good in _repGood.Table
                                 on user.Id equals Good.CreateBy
                                 where Good.CreateDate >= startRentalDate && Good.CreateDate <= endRentalDate
                                 select new sharers
                                 {
                                     UserId = user.Id,
                                     FirstName = user.FirstName,
                                     LastName = user.LastName,
                                     dayprice = Good.Price,
                                     weekprice = Good.PricePerWeek,
                                     GoodId = Good.Id,
                                     GoodName = Good.Name,
                                     CreatedDate = Good.CreateDate,
                                     FullName = string.IsNullOrEmpty(user.FullName.ToLower()) ? "" : user.FullName.ToLower()
                                 }).ToList();

            if (searchShareName != "")
            {
                AllSharerlist = AllSharerlist.Where(x => x.FirstName.ToLower().Contains(searchShareName)
                || x.LastName.ToLower().Contains(searchShareName)
                || x.FullName.ToLower().Contains(searchShareName.ToLower())
                ).ToList();


                //AllSharerlist = (from x in AllSharerlist
                //                 where x.FirstName.ToLower().Contains(searchShareName) || x.LastName.ToLower().Contains(searchShareName)
                //                 || string.IsNullOrEmpty(x.FullName.ToLower()) ? "" : x.FullName.ToLower() == searchShareName.ToLower()
                //                 ).ToList();
            }

            if (searchBorrowerName != "")
            {
                var borrowersgoods = (from req in _repGoodRequest.Table
                                      join user in _repUser.Table
                                      on req.UserId equals user.Id
                                      where user.FirstName.ToLower().Contains(searchBorrowerName.ToLower())
                                      || user.LastName.ToLower().Contains(searchBorrowerName.ToLower()) || user.FullName.ToLower().Contains(searchBorrowerName.ToLower())
                                      select req.GoodId).ToList();

                AllSharerlist = (from data in AllSharerlist where borrowersgoods.Contains(data.GoodId) select data).ToList();
            }


            if (ItemName != "")
            {
                AllSharerlist = AllSharerlist.Where(x => x.GoodName.ToLower().Contains(ItemName.ToLower())).ToList();
            }

            if (categoryId != 0)
            {
                var goodlist = (from cat in _repGoodCategory.Table where cat.CategoryId == categoryId select cat.GoodId).ToList();
                AllSharerlist = (from data in AllSharerlist where goodlist.Contains(data.GoodId) select data).ToList();
            }
            if (amountRangeId != 0)
            {
                List<double> range = getamountrange(amountRangeId);
                if (range[0] == range[1])
                {
                    AllSharerlist = AllSharerlist.Where(x => x.dayprice >= range[0]).ToList();
                }
                else
                {
                    AllSharerlist = AllSharerlist.Where(x => x.dayprice >= range[0] && x.dayprice <= range[1]).ToList();
                }
            }

            userlist = (from res in AllSharerlist
                        select new sharers
                        {
                            GoodId = res.GoodId,
                            GoodName = res.GoodName,
                            UserId = res.UserId,
                            FirstName = res.FirstName,
                            LastName = res.LastName,
                            CreatedDate = res.CreatedDate,
                            Month = OnlyGetMonthName(res.CreatedDate.Month),
                            MonthId = res.CreatedDate.Month
                        }).ToList();

            if (userlist != null && userlist.Count > 0)
            {
                userlistResponse = userlist.OrderByDescending(x => x.MonthId).ToList();

                //for (int i = 1; i <= 12; i++)
                //{
                //    string monthName = GetMonth_Name(i);

                //    userlistResponse.Add(new sharers
                //    {
                //        Month = monthName,
                //        Count = userlist.Where(x => x.CreatedDate.Month == i).Count(),
                //    });
                //}

            }

            return userlistResponse;
        }

        public List<Borrowers> GetAllDataBorrowerList(DateTime startRentalDate, DateTime endRentalDate, string searchShareName, string searchBorrowerName, int categoryId, string ItemName, int amountRangeId)
        {
            List<Borrowers> borrowerList = new List<Borrowers>();
            List<MomentarilyItem> momentarilyItems = new List<MomentarilyItem>();

            Uow.Wrap(uow =>
            {
                borrowerList = GetAllDataBorrowerFilter(startRentalDate, endRentalDate, searchShareName, searchBorrowerName, categoryId, ItemName, amountRangeId);
            }, null, LogSource.GoodService);
            return borrowerList;
        }
        public List<Borrowers> GetAllDataBorrowerFilter(DateTime startRentalDate, DateTime endRentalDate, string searchShareName, string searchBorrowerName, int categoryId, string ItemName, int amountRangeId)
        {
            List<Borrowers> AllBorrowerlist = new List<Borrowers>();
            List<Borrowers> AllBorrowerlistResponse = new List<Borrowers>();


            AllBorrowerlist = (from user in _repUser.Table
                               join req in _repGoodRequest.Table
                               on user.Id equals req.UserId
                               join good in _repGood.Table on req.GoodId equals good.Id
                               where req.ModDate >= startRentalDate && req.ModDate <= endRentalDate
                               && (req.StatusId == 2 || req.StatusId == 5)
                               select new Borrowers
                               {
                                   UserId = user.Id,
                                   FirstName = user.FirstName != null ? user.FirstName : "",
                                   LastName = user.LastName != null ? user.LastName : "",
                                   FullName = user.FullName != null ? user.FullName : "",
                                   GoodId = good.Id,
                                   GoodName = good.Name,
                                   dayprice = good.Price,
                                   weekprice = good.PricePerWeek,
                                   GoodCreateddate = good.CreateDate,
                                   Month = OnlyGetMonthName(good.CreateDate.Month),
                                   MonthId = good.CreateDate.Month

                               }).ToList();




            if (searchShareName != null && searchShareName != "")
            {
                var GoodIDs = (from user in _repUser.Table
                               join good in _repGood.Table on user.Id equals good.CreateBy
                               where user.FirstName.ToLower().Contains(searchShareName.ToLower()) ||
                               user.LastName.ToLower().Contains(searchShareName.ToLower()) ||
                               user.FullName.ToLower().Contains(searchShareName.ToLower())
                               select good.Id
                            ).ToList();

                AllBorrowerlist = (from data in AllBorrowerlist
                                   where
      GoodIDs.Contains(data.GoodId)
                                   select data).ToList();


            }

            if (searchBorrowerName != "")
            {
                AllBorrowerlist = AllBorrowerlist.Where(
                    x => x.FirstName.ToLower().Contains(searchBorrowerName.ToLower()) ||
                    x.LastName.ToLower().Contains(searchBorrowerName.ToLower()) ||
                    x.FullName.ToLower().Contains(searchBorrowerName.ToLower())
                    ).ToList();
            }


            if (ItemName != "")
            {
                AllBorrowerlist = AllBorrowerlist.Where(x => x.GoodName.ToLower().Contains(ItemName.ToLower())).ToList();
            }

            if (categoryId != 0)
            {
                var goodids = (from cat in _repGoodCategory.Table where cat.CategoryId == categoryId select cat.GoodId).ToList();
                AllBorrowerlist = (from data in AllBorrowerlist where goodids.Contains(data.GoodId) select data).ToList();
            }
            if (amountRangeId != 0)
            {
                List<double> range = getamountrange(amountRangeId);
                if (range[0] == range[1])
                {
                    AllBorrowerlist = AllBorrowerlist.Where(x => x.dayprice >= range[0]).ToList();
                }
                else
                {
                    AllBorrowerlist = AllBorrowerlist.Where(x => x.dayprice >= range[0] && x.dayprice <= range[1]).ToList();
                }
            }

            if (AllBorrowerlist != null && AllBorrowerlist.Count > 0)
            {
                AllBorrowerlistResponse = AllBorrowerlist.OrderByDescending(x => x.MonthId).ToList();
                //for (int i = 1; i <= 12; i++)
                //{
                //    string monthName = GetMonth_Name(i);

                //    AllBorrowerlistResponse.Add(new Borrowers
                //    {
                //        Month = monthName,
                //        Count = AllBorrowerlist.Where(x => x.GoodCreateddate.Month == i).Count(),
                //    });
                //}

            }


            return AllBorrowerlistResponse;
        }

        private string OnlyGetMonthName(int monthID)
        {
            string monthName = GetMonth_Name(monthID);
            return monthName;
        }

        //Sanjeev
        private string GetMonth_Name(int monthInteger)
        {
            string daysInMonth = "";
            switch (monthInteger)
            {
                case 1: // jan
                    daysInMonth = "jan";
                    break;
                case 2: // feb
                    daysInMonth = "feb";
                    break;
                case 3: // mar
                    daysInMonth = "mar";
                    break;
                case 4: // apr
                    daysInMonth = "apr";
                    break;
                case 5: // may
                    daysInMonth = "may";
                    break;
                case 6: // jun
                    daysInMonth = "jun";
                    break;
                case 7: // jul
                    daysInMonth = "jul";
                    break;
                case 8: // aug
                    daysInMonth = "aug";
                    break;
                case 9: // sep
                    daysInMonth = "sep";
                    break;
                case 10: // oct
                    daysInMonth = "oct";
                    break;
                case 11: // nov
                    daysInMonth = "nov";
                    break;
                case 12: // dec
                    daysInMonth = "dec";
                    break;
            }
            return daysInMonth;
        }
        public List<double> getamountrange(int amountRangeId)
        {
            List<double> range = new List<double>();
            double startRange = 0;
            double endrange = 0;
            if (amountRangeId == 1)
            {
                startRange = 0; endrange = 50;
            }

            if (amountRangeId == 2)
            {
                startRange = 50; endrange = 100;
            }

            if (amountRangeId == 3)
            {
                startRange = 100; endrange = 200;
            }

            if (amountRangeId == 4)
            {
                startRange = 200; endrange = 300;
            }
            if (amountRangeId == 5)
            {
                startRange = 300; endrange = 400;
            }
            if (amountRangeId == 6)
            {
                startRange = 400; endrange = 500;
            }
            if (amountRangeId == 7)
            {
                startRange = 500; endrange = 1000;
            }
            if (amountRangeId == 8)
            {
                startRange = 1000; endrange = 1000;
            }
            range.Add(startRange);
            range.Add(endrange);
            return range;
        }


        public List<AllTransactionReports> getAllTransactionData()        {            List<AllTransactionReports> reportlist = new List<AllTransactionReports>();            try            {                Uow.Wrap(uow =>
                {
                    reportlist = (from req in _repGoodRequest.Table
                                  join user in _repUser.Table on req.UserId equals user.Id
                                  join usergood in _repUserGood.Table on req.GoodId equals usergood.GoodId
                                  join good in _repGood.Table on req.GoodId equals good.Id
                                  join goodLocation in _repGoodLocation.Table on good.Id equals goodLocation.GoodId
                                  join booking in _repositoryGoodBooking.Table on req.Id equals booking.GoodRequestId
                                  select new AllTransactionReports
                                  {
                                      TransactionId = req.Id,
                                      UserId = req.UserId,
                                      UserName = string.IsNullOrEmpty(user.FullName) ? "" : user.FullName,
                                      GoodId = req.GoodId,
                                      GoodName = string.IsNullOrEmpty(good.Name) ? "" : good.Name,
                                      StatusId = req.StatusId,
                                      StatusName = Convert.ToString(Enum.GetName(typeof(UserRequestStatus), req.StatusId)),
                                      GoodOwnerName = string.IsNullOrEmpty((from u in _repUser.Table where usergood.UserId == u.Id select u.FullName).FirstOrDefault()) ? "" :
                                      (from u in _repUser.Table where usergood.UserId == u.Id select u.FullName).FirstOrDefault(),
                                      GoodOwnerId = (from u in _repUser.Table where usergood.UserId == u.Id select u.Id).FirstOrDefault(),
                                      CreateDate = req.CreateDate,
                                      ModDate = req.ModDate,
                                      RentalStartDate = booking.StartDate,
                                      RentalEndDate = booking.EndDate,
                                      Price = req.Price,
                                      Days = req.Days,
                                      DaysCost = req.DaysCost,
                                      CustomerCost = req.CustomerCost,
                                      CustomerServiceFee = req.CustomerServiceFee,
                                      CustomerServiceFeeCost = req.CustomerServiceFeeCost,
                                      CustomerCharity = req.CustomerCharity,
                                      CustomerCharityCost = req.CustomerCharityCost,
                                      DiscountAmount = req.CouponCode == string.Empty ? 0 : Math.Round(req.DiscountAmount, 2),
                                      SharerCost = req.SharerCost,
                                      SharerServiceFee = req.SharerServiceFee,
                                      SharerServiceFeeCost = req.SharerServiceFeeCost,
                                      SharerCharity = req.SharerCharity,
                                      SharerCharityCost = req.SharerCharityCost,
                                      DiliveryCost = req.DiliveryCost,
                                      ShippingDistance = req.ShippingDistance,
                                      DiliveryPrice = req.DiliveryPrice,
                                      SecurityDeposit = req.SecurityDeposit,
                                      Earnings = req.CustomerServiceFeeCost + req.CustomerCharityCost + req.SharerServiceFeeCost + req.SharerCharityCost,
                                      Description = good.Description,
                                      City = getCity(goodLocation.Latitude, goodLocation.Longitude),
                                      State = getState(goodLocation.Latitude, goodLocation.Longitude),
                                      LateFee = GetAmountAccordingOnFinalFeedbackAndDispute(req.StatusId, req.Id, booking.EndDate, booking.EndTime, req)[0],
                                      ClaimAmount = GetAmountAccordingOnFinalFeedbackAndDispute(req.StatusId, req.Id, booking.EndDate, booking.EndTime, req)[1],
                                      OnDisputeAmountForSharer = GetAmountAccordingOnFinalFeedbackAndDispute(req.StatusId, req.Id, booking.EndDate, booking.EndTime, req)[2],
                                      OnDisputeAmountForBorrower = GetAmountAccordingOnFinalFeedbackAndDispute(req.StatusId, req.Id, booking.EndDate, booking.EndTime, req)[3],
                                      PendingAmount = GetAmountAccordingOnFinalFeedbackAndDispute(req.StatusId, req.Id, booking.EndDate, booking.EndTime, req)[4],
                                      PaymentProcess = (req.StatusId == (int)UserRequestStatus.Closed || req.StatusId == (int)UserRequestStatus.ClosedWithLate || req.StatusId == (int)UserRequestStatus.ClosedWithDamaged || req.StatusId == (int)UserRequestStatus.ClosedWithLateAndDamaged || req.StatusId == (int)UserRequestStatus.ClosedWithDispute) ? "Yes" : "No"
                                  }).ToList();
                }, null, LogSource.GoodRequestService);            }            catch (Exception ex)            {                Ioc.Get<IDbLogger>().LogError(LogSource.GoodRequestService, string.Format("Cannot  Momentarily Request from getAllTransactionData(): {0}", ex));            }            return reportlist;        }
        public double[] GetAmountAccordingOnFinalFeedbackAndDispute(int statusId, int requestId, DateTime bookingEndDate, string bookingEndTime, GoodRequest request)
        {
            double[] _data = new double[5];

            var feedback = _repositoryFinalFeedback.Table.Where(x => x.RequestId == requestId).FirstOrDefault();
            var disputeResolveDetail = _repositoryResolvedDisputeDetail.Table.Where(x => x.RequestId == requestId).FirstOrDefault();


            if (statusId == (int)UserRequestStatus.Late || statusId == (int)UserRequestStatus.ClosedWithLate)
            {
                if (feedback != null && feedback.ModDate.AddHours(48) >= DateTime.Now)
                {
                    var ExpectedReturnDate = bookingEndDate.ToString("MM/dd/yyyy");
                    var ExpectedReturnTime = bookingEndTime;
                    DateTime ExpectedReturnDateTime = DateTime.ParseExact(ExpectedReturnDate + " " + ExpectedReturnTime, "MM/dd/yyyy h:mm tt", CultureInfo.InvariantCulture);

                    var Actualreturndate = feedback.ReturnDate.ToString("MM/dd/yyyy");
                    var Actualreturntime = feedback.ReturnTime;
                    DateTime ActualReturnDateTime = DateTime.ParseExact(Actualreturndate + " " + Actualreturntime, "MM/dd/yyyy h:mm tt", CultureInfo.InvariantCulture);

                    var Differences = ActualReturnDateTime - ExpectedReturnDateTime;

                    var PerDayCost = request.DaysCost / request.Days;
                    var PerHourCost = PerDayCost / 21;

                    double DaysLateFee = 0.00;
                    double HoursLateFee = 0.00;
                    double TotalLateFee = 0.00;
                    double PendingAmount = 0.00;
                    DaysLateFee = Differences.Days * PerDayCost;

                    if (Differences.Hours >= 21)
                    {
                        HoursLateFee = PerDayCost;
                    }
                    else
                    {
                        HoursLateFee = (Differences.Hours * 2) * PerHourCost;
                        if (HoursLateFee > PerDayCost)
                        {
                            HoursLateFee = PerDayCost;
                        }
                    }
                    TotalLateFee = DaysLateFee + HoursLateFee;
                    if (TotalLateFee > request.SecurityDeposit)
                    {
                        PendingAmount = TotalLateFee - request.SecurityDeposit;
                        TotalLateFee = request.SecurityDeposit;
                    }

                    _data[0] = Math.Round(TotalLateFee, 2);
                    _data[2] = Math.Round((request.SharerCost + TotalLateFee), 2);
                    if (TotalLateFee != request.SecurityDeposit)
                    {
                        _data[3] = Math.Round((request.SecurityDeposit - TotalLateFee), 2);
                    }
                    _data[4] = Math.Round(PendingAmount, 2);

                }
            }
            else if (statusId == (int)UserRequestStatus.Damaged || statusId == (int)UserRequestStatus.ClosedWithDamaged)
            {
                if (feedback != null && feedback.ModDate.AddHours(48) >= DateTime.Now)
                {
                    double TotalClaim = feedback.Claim;
                    double PendingAmount = 0.00;

                    if (TotalClaim > request.SecurityDeposit)
                    {
                        PendingAmount = TotalClaim - request.SecurityDeposit;
                        TotalClaim = request.SecurityDeposit;

                    }
                    if (TotalClaim != request.SecurityDeposit)
                    {
                        var _borrowerAmount = request.SecurityDeposit - TotalClaim;
                        _borrowerAmount = Math.Round(_borrowerAmount, 2);
                        _data[3] = _borrowerAmount;
                    }
                    var _sharerAmount = request.SharerCost + TotalClaim;
                    _sharerAmount = Math.Round(_sharerAmount, 2);
                    _data[1] = TotalClaim;
                    _data[2] = _sharerAmount;
                    _data[4] = Math.Round(PendingAmount, 2);

                }

            }
            else if (statusId == (int)UserRequestStatus.LateAndDamaged || statusId == (int)UserRequestStatus.ClosedWithLateAndDamaged)
            {
                if (feedback != null && feedback.ModDate.AddHours(48) >= DateTime.Now)
                {

                    var ExpectedReturnDate = request.GoodBooking.EndDate.ToString("MM/dd/yyyy");
                    var ExpectedReturnTime = request.GoodBooking.EndTime;
                    DateTime ExpectedReturnDateTime = DateTime.ParseExact(ExpectedReturnDate + " " + ExpectedReturnTime, "MM/dd/yyyy h:mm tt", CultureInfo.InvariantCulture);

                    var Actualreturndate = feedback.ReturnDate.ToString("MM/dd/yyyy");
                    var Actualreturntime = feedback.ReturnTime;
                    DateTime ActualReturnDateTime = DateTime.ParseExact(Actualreturndate + " " + Actualreturntime, "MM/dd/yyyy h:mm tt", CultureInfo.InvariantCulture);

                    var Differences = ActualReturnDateTime - ExpectedReturnDateTime;

                    var PerDayCost = request.DaysCost / request.Days;
                    var PerHourCost = PerDayCost / 21;
                    double DaysLateFee = 0.00;
                    double HoursLateFee = 0.00;
                    double TotalLateFee = 0.00;
                    double PendingAmount = 0.00;
                    double TotalClaim = feedback.Claim;
                    DaysLateFee = Differences.Days * PerDayCost;

                    if (Differences.Hours >= 21)
                    {
                        HoursLateFee = PerDayCost;
                    }
                    else
                    {
                        HoursLateFee = (Differences.Hours * 2) * PerHourCost;
                        if (HoursLateFee > PerDayCost)
                        {
                            HoursLateFee = PerDayCost;
                        }
                    }
                    TotalLateFee = DaysLateFee + HoursLateFee;
                    double TotalRecoveryAmount = TotalLateFee + TotalClaim;

                    if (TotalRecoveryAmount > request.SecurityDeposit)
                    {
                        PendingAmount = TotalRecoveryAmount - request.SecurityDeposit;
                        TotalRecoveryAmount = request.SecurityDeposit;
                    }
                    if (TotalRecoveryAmount != request.SecurityDeposit)
                    {
                        var _borrowerAmount = request.SecurityDeposit - TotalRecoveryAmount;
                        _borrowerAmount = Math.Round(_borrowerAmount, 2);
                        _data[3] = _borrowerAmount;
                    }
                    var _sharerAmount = request.SharerCost + TotalRecoveryAmount;
                    _sharerAmount = Math.Round(_sharerAmount, 2);
                    _data[0] = Math.Round(TotalLateFee, 2);
                    _data[1] = Math.Round(TotalClaim, 2);
                    _data[2] = _sharerAmount;
                    _data[4] = Math.Round(PendingAmount, 2);

                }
            }
            else if (statusId == (int)UserRequestStatus.ClosedWithDispute)
            {
                if (disputeResolveDetail != null)
                {
                    if (disputeResolveDetail.OwnerShare > 0)
                    {
                        _data[2] = Math.Round(disputeResolveDetail.OwnerShare, 2);
                    }
                    if (disputeResolveDetail.BorrowerShare > 0)
                    {
                        _data[3] = Math.Round(disputeResolveDetail.BorrowerShare, 2);
                    }
                }
            }
            else if (statusId == (int)UserRequestStatus.CanceledBySharer)
            {
                _data[3] = Math.Round((request.DaysCost + request.DiliveryCost + request.SecurityDeposit), 2);
            }
            else if (statusId == (int)UserRequestStatus.CanceledByBorrower)
            {
                var StartDate = request.GoodBooking.StartDate.ToString("MM/dd/yyyy");
                var StartTime = request.GoodBooking.StartTime;
                DateTime startDateTime = DateTime.ParseExact(StartDate + " " + StartTime, "MM/dd/yyyy h:mm tt", CultureInfo.InvariantCulture);

                var diffrence = (startDateTime - DateTime.Now).TotalHours;

                if (diffrence >= 24)
                {
                    if (diffrence > 48)
                    {
                        _data[3] = Math.Round((request.DaysCost + request.DiliveryCost + request.SecurityDeposit), 2);
                    }
                    else if (diffrence >= 24 && diffrence <= 48)
                    {
                        _data[3] = Math.Round((request.DiliveryCost + (request.DaysCost / 2) + request.SecurityDeposit), 2);

                    }
                    else if (diffrence < 24)
                    {
                        _data[3] = Math.Round((request.SecurityDeposit), 2);
                    }

                }
            }
            return _data;
        }

        public string getCity(double lat, double lng)
        {
            string city = string.Empty;
            //string[] _data = new string[2];
            XmlDocument xDoc = new XmlDocument();

            xDoc.Load("https://maps.googleapis.com/maps/api/geocode/xml?latlng=" + lat + "," + lng + "&sensor=false&key=AIzaSyBP9cfi8Ngb5bgwFu253vGDaaTihNMGjXg");
            XmlNodeList xNodelst = xDoc.GetElementsByTagName("result");
            if (xNodelst.Count > 0)
            {
                XmlNode xNode = xNodelst.Item(0);

                var data = xNode.SelectNodes("address_component");
                for (int i = 0; i < data.Count; i++)
                {
                    if (data[i].OuterXml.Contains("locality"))
                    {
                        var outerx = data[i].OuterXml;
                        if(outerx.Contains("<long_name>"))
                        {
                            var startindex = outerx.IndexOf("<long_name>");
                            var endindex = outerx.IndexOf("</long_name>");
                            city = outerx.Substring(startindex + 11, endindex - (startindex + 11));
                        }
                        else
                        {
                            city = "";
                        }
                       
                    }
                }
                //string fulladdress = xNode.SelectSingleNode("formatted_address").InnerText;
                //city = xNode.SelectSingleNode("address_component[3]/long_name").InnerText;
                //string state = xNode.SelectSingleNode("address_component[4]/long_name").InnerText;
                //string country = xNode.SelectSingleNode("address_component[5]/long_name").InnerText;

            }
            return city;
        }
        public string getState(double lat, double lng)
        {
            string state = string.Empty;
            //string[] _data = new string[2];
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load("https://maps.googleapis.com/maps/api/geocode/xml?latlng=" + lat + "," + lng + "&sensor=false&key=AIzaSyBP9cfi8Ngb5bgwFu253vGDaaTihNMGjXg");

            XmlNodeList xNodelst = xDoc.GetElementsByTagName("result");
            if (xNodelst.Count > 0)
            {
                XmlNode xNode = xNodelst.Item(0);
                var data = xNode.SelectNodes("address_component");
                for (int i = 0; i < data.Count; i++)
                {
                    if (data[i].OuterXml.Contains("administrative_area_level_1"))
                    {
                        var outerx = data[i].OuterXml;
                        if (outerx.Contains("<long_name>"))
                        {
                            var startindex = outerx.IndexOf("<long_name>");
                            var endindex = outerx.IndexOf("</long_name>");
                            state = outerx.Substring(startindex + 11, endindex - (startindex + 11));
                        }
                        else
                        {
                            state = "";
                        }

                    }
                }

            }
            return state;
        }

        public string getUserEmail(int userID)
        {
            string userEmail = string.Empty;
            Uow.Wrap(uow =>
            {
                Ioc.Get<IDbLogger>().LogMessage(LogSource.PersonService, string.Format("Trying to get user email"));
                userEmail = _repUser.Table.Where(x => x.Id == userID).Select(y => y.Email).FirstOrDefault();
            }, null, LogSource.GoodService);

            return userEmail;

        }

        public List<CouponType> getCouponList()
        {
            List<CouponType> CouponList = new List<CouponType>();
            Uow.Wrap(uow =>
            {
                var coupons = (from coupon in _repositoryGlobalCode.Table where coupon.GlobalCodeCategoryId == 2 select coupon).ToList();
                foreach (var item in coupons)
                {
                    CouponList.Add(new CouponType { Id = item.Id, CouponName = item.GlobalCodeName });
                }
            }, null, LogSource.GoodService);
            return CouponList;
        }

        public bool SaveUserCoupon(UserCouponVM userCouponVM)
        {
            var result = false;
            Uow.Wrap(uow =>
            {
                UserCoupon obj = new UserCoupon();
                obj.CouponType = userCouponVM.CouponType;
                obj.CouponCode = userCouponVM.CouponCode;
                obj.CouponDiscountType = userCouponVM.CouponDiscountType;
                obj.CouponDiscount = userCouponVM.CouponDiscount;
                obj.ExpiryDate = userCouponVM.ExpiryDate;
                obj.NoExpiryDateStatus = userCouponVM.NoExpiryDateStatus;
                obj.CreateDate = DateTime.Now;
                obj.ModDate = DateTime.Now;
                var saveUserCoupon = _repositoryUserCoupon.Save(obj);
                result = true;
            }, null, LogSource.GoodService);
            return result;
        }

        public bool UpdateUserCoupon(UserCouponVM userCouponVM)
        {
            var result = false;
            Uow.Wrap(uow =>
            {
                UserCoupon obj = new UserCoupon();
                var getEditDettal = _repositoryUserCoupon.Get(userCouponVM.Id);

                if (getEditDettal != null)
                {
                    getEditDettal.CouponType = userCouponVM.CouponType;
                    getEditDettal.CouponCode = userCouponVM.CouponCode;
                    getEditDettal.CouponDiscountType = userCouponVM.CouponDiscountType;
                    getEditDettal.CouponDiscount = userCouponVM.CouponDiscount;
                    getEditDettal.ExpiryDate = userCouponVM.ExpiryDate;
                    getEditDettal.NoExpiryDateStatus = userCouponVM.NoExpiryDateStatus;
                    getEditDettal.ModDate = DateTime.Now;
                    var UpdateUserCoupon = _repositoryUserCoupon.SaveOrUpdate(getEditDettal);
                }
                result = true;
            }, null, LogSource.GoodService);
            return result;
        }

        public List<UserCouponVM> getAllCouponList()
        {
            List<UserCouponVM> listuserCouponVMs = new List<UserCouponVM>();
            Uow.Wrap(uow =>
            {
                listuserCouponVMs = (from cop in _repositoryUserCoupon.Table
                                     select new UserCouponVM
                                     {
                                         Id = cop.Id,
                                         CouponType = cop.CouponType,
                                         CouponTypeName = (_repositoryGlobalCode.Table.Where(x => x.Id == cop.CouponType && x.GlobalCodeCategoryId == 2).Select(y => y.GlobalCodeName).FirstOrDefault() ?? ""),
                                         CouponCode = cop.CouponCode,
                                         CouponDiscountType = cop.CouponDiscountType,
                                         CouponDiscountTypeInPercentage = cop.CouponDiscountType == 1 ? "%" : "$",
                                         //CouponDiscountTypeInAmount = cop.CouponDiscountType == 2 ? "Amt" : "--",
                                         CouponDiscount = cop.CouponDiscount,
                                         ExpiryDate = cop.ExpiryDate,
                                         NoExpiryDateStatus = cop.NoExpiryDateStatus,
                                         NoExpiryDateStatusInString = cop.NoExpiryDateStatus == true ? "Y" : "N",
                                         Status = cop.Status,
                                         StatusString = cop.Status == true ? "Y" : "N"
                                     }).ToList();
            }, null, LogSource.GoodService);
            return listuserCouponVMs;
        }

        public UserCouponVM getEditCouponDetail(int? id)
        {
            UserCouponVM obj = new UserCouponVM();
            Uow.Wrap(uow =>
            {
                var data = _repositoryUserCoupon.Table.Where(x => x.Id == id).FirstOrDefault();
                obj.Id = data.Id;
                obj.CouponType = data.CouponType;
                obj.CouponCode = data.CouponCode;
                obj.CouponDiscountType = data.CouponDiscountType;
                obj.CouponDiscount = data.CouponDiscount;
                obj.ExpiryDate = data.ExpiryDate;
                obj.NoExpiryDateStatus = data.NoExpiryDateStatus;
                obj.Status = data.Status;
            }, null, LogSource.GoodService);
            return obj;
        }

        public bool DeleteCoupon(int Id)
        {
            var result = false;
            try
            {
                Uow.Wrap(uow =>
                {
                    var deleteCoupon = (from g in _repositoryUserCoupon.Table
                                        where g.Id == Id
                                        select g).FirstOrDefault();
                    if (deleteCoupon != null)
                    {
                        _repositoryUserCoupon.Delete(deleteCoupon);
                        result = true;
                    }


                }, null, LogSource.GoodService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogError(LogSource.GoodService, string.Format("Cannot delete coupon: {0}", ex));
            }
            return result;
        }
        public int blockedCoupon(int CouponId, bool checkedValue)
        {
            int result = 0;
            try
            {
                Uow.Wrap(uow =>
                {
                    result = _repositoryUserCoupon.CouponBlockedStatusChanged(CouponId, checkedValue);

                }, null, LogSource.GoodService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogError(LogSource.GoodService, string.Format("Cannot blocked coupon: {0}", ex));
            }
            return result;
        }

        public bool AlreadyExsistByCouponType(int CouponType)
        {
            var result = false;
            try
            {
                Uow.Wrap(uow =>
                {
                    var deleteCoupon = (from g in _repositoryUserCoupon.Table
                                        where g.CouponType == CouponType
                                        select g).Count();
                    if (deleteCoupon > 0)
                    {
                        result = true;
                    }

                }, null, LogSource.GoodService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogError(LogSource.GoodService, string.Format("Cannot delete coupon: {0}", ex));
            }
            return result;
        }

        public Result<PriceViewModel> CalculateCouponAmount(string CouponCode, double CustomerCost, int GoodId, string StartDate, string EndDate, double ShippingDistance, bool ApplyForDelivery, int currentuservalue)
        {
            double result = 0;
            PriceViewModel model = new PriceViewModel();
            Result<PriceViewModel> resultNew = new Result<PriceViewModel>(CreateResult.Error, model);
            try
            {
                Uow.Wrap(uow =>
                {

                    var calculatePriceResult = _goodRequestService.CalculatePriceOnDiscount(
                    GoodId: GoodId,
                    StartDate: DateTime.ParseExact(StartDate, "MM/dd/yyyy", null),
                    EndDate: DateTime.ParseExact(EndDate, "MM/dd/yyyy", null),
                    ShippingDistance: ShippingDistance,
                    ApplyForDelivery: ApplyForDelivery);

                    if (calculatePriceResult.CreateResult == CreateResult.Success)
                    {
                        var usedcoupan = _repGoodRequest.Table.Where(x => x.CouponCode == CouponCode && x.UserId == currentuservalue).Count();
                        if (usedcoupan > 0)
                        {
                            result = -1;
                        }
                        else
                        {
                            var getCouponPercentageOrAmt = (from g in _repositoryUserCoupon.Table
                                                            where g.CouponCode == CouponCode && (g.ExpiryDate >= DateTime.Now || g.NoExpiryDateStatus == true) && g.Status == false
                                                            select g).FirstOrDefault();
                            if (getCouponPercentageOrAmt != null)
                            {
                                int couponDisType = getCouponPercentageOrAmt.CouponDiscountType;
                                double couponDisAmount = getCouponPercentageOrAmt.CouponDiscount;

                                if (couponDisType == 1)
                                {
                                    result = calculatePriceResult.Obj.CustomerCost * couponDisAmount / 100;
                                    //result = CustomerCost - result;
                                }
                                else
                                {
                                    //result = CustomerCost - couponDisAmount;
                                    double _couponDisc = Math.Round(couponDisAmount, 2);
                                    if (calculatePriceResult.Obj.CustomerCost < _couponDisc)
                                    {
                                        result = -2;
                                        //calculatePriceResult.Obj.CouponDiscount = result;
                                        //resultNew = calculatePriceResult;

                                    }
                                    else
                                    {
                                        result = couponDisAmount;
                                    }
                                   
                                }
                                var Discountlimit = (calculatePriceResult.Obj.CustomerServiceFeeCost / 2) <= 50 ? calculatePriceResult.Obj.CustomerServiceFeeCost / 2 : 50;

                                if (result > Discountlimit)
                                {
                                    result = Discountlimit;
                                }
                            }
                            else
                            {
                                result = -1;
                            }
                        }
                    }
                    calculatePriceResult.Obj.CouponDiscount = result;
                    resultNew = calculatePriceResult;
                }, null, LogSource.GoodService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogError(LogSource.GoodService, string.Format("Cannot less coupon amount: {0}", ex));
            }
            return resultNew;
        }

        public List<FAQList> GetFAQs()
        {
            List<FAQList> fAQs = new List<FAQList>();
            Uow.Wrap(uow =>
            {
                fAQs = (from data in _repositoryFAQ.Table
                        select new FAQList
                        {
                            Id = data.Id,
                            Question = data.Question,
                            Answer = data.Answer,
                            Type = data.Type,
                        }).ToList();

            }, null, LogSource.User);

            return fAQs;

        }
        public List<RecentlyRentedProduct> GetMostRecentlyRentedProduct()
        {
            List<RecentlyRentedProduct> recentlyRentedProducts = new List<RecentlyRentedProduct>();
            try
            {
                Uow.Wrap(uow =>
                {
                    List<RecentlyRentedProduct> resultv = (from pv in _repGood.Table
                                                           join usr in _repUser.TableFor<User>() on pv.CreateBy equals usr.Id
                                                           where usr.Id == 5175
                                                           select new RecentlyRentedProduct
                                                           {
                                                               //GoodRequestId = gr.Id,
                                                               GoodId = pv.Id,
                                                               Name = pv.Name,
                                                               Price = pv.Price,
                                                               PricePerWeek = pv.PricePerWeek,
                                                               RentPeriodDay = pv.RentPeriodDay,                                                               RentPeriodWeek = pv.RentPeriodWeek,
                                                               GoodImageUrl = (from g in _repositoryGoodImg.Table where g.GoodId == pv.Id select g.FileName).FirstOrDefault() == null ? "" : (from g in _repositoryGoodImg.Table where g.GoodId == pv.Id select g.FileName).FirstOrDefault()
                                                               //Count = (from v in _repGoodRequest.Table where v.GoodId == pv.Id select v.Id).Count()
                                                           }
                             ).Distinct().ToList();




                    resultv = resultv.GroupBy(x => x.GoodId).Select(y => y.First()).ToList();

                    foreach (var path in resultv)                    {                        var filePath = HttpContextFactory.Current.Server.MapPath(                            AppSettings.GetInstance().GoodImageLocalStoragePath + path.GoodImageFileName);                        if (!File.Exists(filePath))                        {                            string dummyImage = "error-bg.png";                            path.GoodImageUrl = dummyImage;                        }                        if (path.Price <= 0)                        {                            if (path.PricePerWeek > 0)                            {                                path.Price = path.PricePerWeek / 7;                                path.Price = Math.Round(path.Price, 2);                            }                        }                        if (path.PricePerWeek <= 0)                        {                            if (path.Price > 0)                            {                                path.PricePerWeek = path.Price * 7;                                path.PricePerWeek = Math.Round(path.PricePerWeek, 2);                            }                        }                    }

                    recentlyRentedProducts = resultv.OrderByDescending(x => x.GoodRequestId).ToList();

                }, null, LogSource.GoodService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogError(LogSource.GoodService, string.Format("No Recent Product Added: {0}", ex));
            }
            return recentlyRentedProducts;
        }

        //public List<RecentlyRentedProduct> GetMostRecentlyRentedProduct()
        //{
        //    List<RecentlyRentedProduct> recentlyRentedProducts = new List<RecentlyRentedProduct>();
        //    try
        //    {
        //        Uow.Wrap(uow =>
        //        {
        //            List<RecentlyRentedProduct> resultv = (from pv in _repGood.Table
        //                                                   join gr in _repGood.TableFor<GoodRequest>() on pv.Id equals gr.GoodId
        //                                                   //join gimg in _repGood.TableFor<GoodImg>() on pv.Id equals gimg.GoodId into g
        //                                                   //from result in g.DefaultIfEmpty()
        //                                                   where (gr.StatusId == (int)UserRequestStatus.Approved
        //                                                   || gr.StatusId == (int)UserRequestStatus.Paid
        //                                                   || gr.StatusId == (int)UserRequestStatus.Released
        //                                                   || gr.StatusId == (int)UserRequestStatus.Received
        //                                                   || gr.StatusId == (int)UserRequestStatus.Returned
        //                                                   || gr.StatusId == (int)UserRequestStatus.ReturnConfirmed
        //                                                   || gr.StatusId == (int)UserRequestStatus.Closed
        //                                                   || gr.StatusId == (int)UserRequestStatus.Late
        //                                                   || gr.StatusId == (int)UserRequestStatus.Damaged
        //                                                   || gr.StatusId == (int)UserRequestStatus.Reviewing
        //                                                   || gr.StatusId == (int)UserRequestStatus.LateAndDamaged)
        //                                                   select new RecentlyRentedProduct
        //                                                   {
        //                                                       GoodRequestId = gr.Id,
        //                                                       GoodId = gr.GoodId,
        //                                                       Name = pv.Name,
        //                                                       Price = pv.Price,
        //                                                       PricePerWeek = pv.PricePerWeek,
        //                                                       RentPeriodDay = pv.RentPeriodDay,        //                                                       RentPeriodWeek = pv.RentPeriodWeek,
        //                                                       GoodImageUrl = (from g in _repositoryGoodImg.Table where g.GoodId == pv.Id select g.FileName).FirstOrDefault() == null ? "" : (from g in _repositoryGoodImg.Table where g.GoodId == pv.Id select g.FileName).FirstOrDefault()
        //                                                       //Count = (from v in _repGoodRequest.Table where v.GoodId == pv.Id select v.Id).Count()
        //                                                   }
        //                     ).Distinct().ToList();




        //            resultv = resultv.GroupBy(x => x.GoodId).Select(y => y.First()).ToList();

        //            foreach (var path in resultv)        //            {        //                var filePath = HttpContextFactory.Current.Server.MapPath(        //                    AppSettings.GetInstance().GoodImageLocalStoragePath + path.GoodImageFileName);        //                if (!File.Exists(filePath))        //                {        //                    string dummyImage = "error-bg.png";        //                    path.GoodImageUrl = dummyImage;        //                }        //                if (path.Price <= 0)        //                {        //                    if (path.PricePerWeek > 0)        //                    {        //                        path.Price = path.PricePerWeek / 7;        //                        path.Price = Math.Round(path.Price, 2);        //                    }        //                }        //                if (path.PricePerWeek <= 0)        //                {        //                    if (path.Price > 0)        //                    {        //                        path.PricePerWeek = path.Price * 7;        //                        path.PricePerWeek = Math.Round(path.PricePerWeek, 2);        //                    }        //                }        //            }

        //            recentlyRentedProducts = resultv.OrderByDescending(x => x.GoodRequestId).Take(5).ToList();

        //        }, null, LogSource.GoodService);
        //    }
        //    catch (Exception ex)
        //    {
        //        Ioc.Get<IDbLogger>().LogError(LogSource.GoodService, string.Format("No Recent Product Added: {0}", ex));
        //    }
        //    return recentlyRentedProducts;
        //}

        public List<RecentlyRentedProduct> GetMostFeaturedProduct()
        {
            List<RecentlyRentedProduct> featuredRentedItems = null;
            Uow.Wrap(uow =>
            {
                List<RecentlyRentedProduct> resultv = (from pv in _repGood.Table
                                                           //join p in _repGood.TableFor<GoodShareDate>() on pv.Id equals p.GoodId
                                                           //join gimg in _repGood.TableFor<GoodImg>() on pv.Id equals gimg.GoodId
                                                       join gr in _repGood.TableFor<GoodRequest>() on pv.Id equals gr.GoodId
                                                       where (gr.StatusId == (int)UserRequestStatus.Approved
                                                            || gr.StatusId == (int)UserRequestStatus.Paid
                                                            || gr.StatusId == (int)UserRequestStatus.Released
                                                            || gr.StatusId == (int)UserRequestStatus.Received
                                                            || gr.StatusId == (int)UserRequestStatus.Returned
                                                            || gr.StatusId == (int)UserRequestStatus.ReturnConfirmed
                                                            || gr.StatusId == (int)UserRequestStatus.Closed
                                                            || gr.StatusId == (int)UserRequestStatus.Late
                                                            || gr.StatusId == (int)UserRequestStatus.Damaged
                                                            || gr.StatusId == (int)UserRequestStatus.Reviewing
                                                            || gr.StatusId == (int)UserRequestStatus.LateAndDamaged)
                                                       select new RecentlyRentedProduct
                                                       {
                                                           GoodRequestId = gr.Id,
                                                           GoodId = gr.GoodId,
                                                           Name = pv.Name,
                                                           Price = pv.Price,
                                                           PricePerWeek = pv.PricePerWeek,
                                                           RentPeriodDay = pv.RentPeriodDay,
                                                           RentPeriodWeek = pv.RentPeriodWeek,
                                                           GoodImageUrl = (from g in _repositoryGoodImg.Table where g.GoodId == pv.Id select g.FileName).FirstOrDefault() == null ? "" : (from g in _repositoryGoodImg.Table where g.GoodId == pv.Id select g.FileName).FirstOrDefault(),
                                                           Count = (from v in _repGoodRequest.Table where v.GoodId == pv.Id select v.Id).Count()
                                                       }
                              ).Distinct().ToList();

                resultv = resultv.GroupBy(x => x.GoodId).Select(y => y.First()).ToList();

                foreach (var path in resultv)                {                    var filePath = HttpContextFactory.Current.Server.MapPath(                        AppSettings.GetInstance().GoodImageLocalStoragePath + path.GoodImageFileName);                    if (!File.Exists(filePath))                    {                        string dummyImage = "error-bg.png";                        path.GoodImageUrl = dummyImage;                    }                    if (path.Price <= 0)                    {                        if (path.PricePerWeek > 0)                        {                            path.Price = path.PricePerWeek / 7;                            path.Price = Math.Round(path.Price, 2);                        }                    }                    if (path.PricePerWeek <= 0)                    {                        if (path.Price > 0)                        {                            path.PricePerWeek = path.Price * 7;                            path.PricePerWeek = Math.Round(path.PricePerWeek, 2);                        }                    }                }
                featuredRentedItems = resultv.OrderByDescending(y => y.Count).Take(4).ToList();

            }, null, LogSource.GoodService);
            return featuredRentedItems;
        }
        public List<ChatTopicsVM> GetChatTopics(bool status)
        {
            List<ChatTopicsVM> topics = new List<ChatTopicsVM>();
            Uow.Wrap(uow =>
            {
                topics = (from data in _repositoryChatTopics.Table
                        select new ChatTopicsVM
                        {
                            Id = data.Id,
                          Name = data.Name
                        }).ToList();
                if (status)
                    topics = topics.ToList();
                else
                    topics = topics.Where(x => x.Id == (int)GlobalTopics.Account || x.Id == (int)GlobalTopics.Others).ToList();

            }, null, LogSource.PersonService);

            return topics;

        }

        public bool getUserverificationForChatBot(string email, string phone)
        {
            bool result = false;
            Uow.Wrap(uow =>
            {
                result = _repUser.Table.Any(x => x.Email == email.Trim());

            }, null, LogSource.PersonService);

            return result;
        }

       public List<ChatBotQuestionsVM> GetChatQuestions(int topicId)
        {
            List<ChatBotQuestionsVM> questions = new List<ChatBotQuestionsVM>();
            Uow.Wrap(uow =>
            {
                questions = (from data in _repositoryChatQuestions.Table where data.TopicId == topicId
                          select new ChatBotQuestionsVM
                          {
                              Id = data.Id,
                              Question = data.Question,
                              TopicId = data.TopicId,
                              AnsId = data.AnsId,

                          }).ToList();

            }, null, LogSource.PersonService);

            return questions;
        }

      public List<ChatBotQuestionsŌrAnswersVM> GetChatQuestionsOrAnswers(int questionid)
        {
            List<ChatBotQuestionsŌrAnswersVM> questionsOrAnswer = new List<ChatBotQuestionsŌrAnswersVM>();
            Uow.Wrap(uow =>
            {

                var data = (from cbQues in _repositoryChatQuestions.Table
                            join cbAns in _repositoryChatAnswers.Table on cbQues.Id equals cbAns.QuesId
                            where cbQues.Id == questionid
                            select new { cbQues, cbAns }).ToList();


                //questionsOrAnswer = (from data in _repositoryChatQuestions.Table
                //             where data.TopicId == questionid
                //                     select new ChatBotQuestionsŌrAnswersVM
                //                     {
                //                 //Id = data.Id,
                //                 //Question = data.Question,
                //                 //TopicId = data.TopicId,
                //                 //AnsId = data.AnsId,

                                //             }).ToList();

            }, null, LogSource.PersonService);
            return questionsOrAnswer;
        }

    }
}