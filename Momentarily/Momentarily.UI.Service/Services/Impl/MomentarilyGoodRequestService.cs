using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Apeek.Common;
using Apeek.Common.Definitions;
using Apeek.Common.Logger;
using Apeek.Common.Models;
using Apeek.Core.Services;
using Apeek.Core.Services.Impl;
using Apeek.Entities.Entities;
using Apeek.NH.DataAccessLayer.DataAccess;
using Apeek.NH.Repository.Repositories;
using Apeek.ViewModels.Models;
using Apeek.ViewModels.Models.Impl;
using Momentarily.Entities.Entities;
using System.Text;
using Momentarily.ViewModels.Models;
namespace Momentarily.UI.Service.Services.Impl
{
    public class MomentarilyGoodRequestService : GoodRequestService<MomentarilyItem>, IMomentarilyGoodRequestService
    {
        private readonly IRepositoryGood<MomentarilyItem, MomentarilyItem> _repGood;
        private readonly IRepositoryGoodPropertyValue _repGoodPropertyValue;
        private readonly IRepositoryGoodPropertyValueDefinition _repGoodPropertyValueDefinition;
        private readonly IRepositoryPaymentTransaction _repPaymentTransaction;
        private readonly IRepositoryDisputes _repDisputes;
        private readonly IRepositoryPaypalInfoPaymentDetail _repyPaypalInfoPaymentDetail;
        private readonly IRepositoryGoodImg _repositoryGoodImg;
        protected readonly IRepositoryCancelledRequest _repositoryCancelledRequest;
        public MomentarilyGoodRequestService(IRepositoryUser repUser, IRepositoryGoodRequest repGoodRequest,
            IRepositoryGoodBooking repGoodBooking, IRepositoryGood<MomentarilyItem, MomentarilyItem> repGood,
            IRepositoryGoodPropertyValue repGoodPropertyValue, IRepositoryGoodPropertyValueDefinition repGoodPropertyValueDefinition,
            IRepositoryUserGood repUserGood, IRepositoryPaymentTransaction repPaymentTransaction,
            IRepositoryPhoneNumber repPhoneNumber, IRepositoryGoodBookingRank repBookingRank,
            IRepositoryDisputes repDisputes, IRepositoryPaypalInfoPaymentDetail repyPaypalInfoPaymentDetail, IRepositoryGoodImg repositoryGoodImg, IRepositoryCancelledRequest repositoryCancelledRequest)
            : base(repUser, repGoodRequest, repGoodBooking, repGood, repUserGood, repPhoneNumber, repBookingRank,repDisputes, repyPaypalInfoPaymentDetail, repositoryGoodImg, repositoryCancelledRequest)
        {
            _repGood = repGood;
            _repGoodPropertyValue = repGoodPropertyValue;
            _repGoodPropertyValueDefinition = repGoodPropertyValueDefinition;
            _repPaymentTransaction = repPaymentTransaction;
            _repDisputes = repDisputes;
            _repyPaypalInfoPaymentDetail = repyPaypalInfoPaymentDetail;
            _repositoryGoodImg = repositoryGoodImg;
            _repositoryCancelledRequest = repositoryCancelledRequest;
        }
        public int CloseHoursAfterEnd {
            get { return 24; }
        }
        public Result<RequestViewModel> GetGoodRequest(int userId, RequestModel requestModel)
        {
            var result = new Result<RequestViewModel>(CreateResult.Error, new RequestViewModel());
            try
            {
                Uow.Wrap(u =>
                {
                    result = BaseGetGoodRequest(userId, requestModel, u);
                    if (result.CreateResult == CreateResult.Success)
                    {
                        var good = _repGood.Get(result.Obj.GoodId);
                        if (good != null)
                        {
                            good.GoodPropertyValues = _repGoodPropertyValue.GetGoodProperties(good.Id);                            
                            result.Obj.GoodLocation = good.Location;
                            result.Obj.SecurityDeposit = good.Deposit;
                            result.Obj.ServiceFee = Ioc.Get<ISettingsDataService>().GetBorrowerPaymentTransactionCommision();
                            result.Obj.SharerAgreeToShareImmediately = good.AgreeToShareImmediately;
                            var type = _repGoodPropertyValueDefinition.Get(good.TypeId);
                            if (type != null)
                            {
                                result.Obj.GoodType = type.Value;
                            }
                        }
                    }
                },
                null,
                LogSource.GoodRequestService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>()
                    .LogWarning(LogSource.ImageProcessor, string.Format("Get good request fail. Ex: {0}.", ex));
            }
            return result;
        }
        public override Result<IList<ApeekPayout>> GetGoodRequestForPayout()
        {
            var result = new Result<IList<ApeekPayout>>(CreateResult.Error, new List<ApeekPayout>());
            try
            {
                Uow.Wrap(u =>
                {
                    //todo test 1h after start date
                    //var payoutDay = DateTime.Now.AddHours(-1);
                    var payoutDay = DateTime.Now.AddHours(-24);
                    var queryBuilder = new StringBuilder();
                    queryBuilder.Append(@"SELECT    pt.cost as Amount, 
		                                            gr.Id as GoodRequestId, 
		                                            g.name as GoodName,
		                                            pr.recipient_token as RecipientToken,
                                                    g.id as GoodId,
                                                    ug.user_id as UserId,
                                                    gr.user_id as GoodRequestUserId
                                            FROM [dbo].[c_good_request] as gr
                                            JOIN [dbo].[c_good] as g ON gr.good_id = g.id
                                            JOIN [dbo].[c_good_booking] as gb ON gr.id = gb.good_request_id
                                            JOIN [dbo].[c_payment_transaction] as pt ON gr.id = pt.good_request_id
                                            JOIN [dbo].[c_user_good] as ug ON gr.good_id = ug.good_id
                                            LEFT JOIN [dbo].[p_pinpayment_recipient] as pr ON ug.user_id = pr.user_id");
                    queryBuilder.Append(String.Format(@"  WHERE pt.type LIKE '{0}' AND gr.status_id={1} AND  gb.start_date < CONVERT( DATETIME, '{2}') AND pt.status_id={3}", PaymentTransactionType.SaleType, (int)UserRequestStatus.Paid, payoutDay.ToString(CultureInfo.InvariantCulture),(int) PaymentTransactionStatus.Pending));
                    var payouts = _repGoodRequest.GetItems<ApeekPayout>(queryBuilder.ToString());
                    result.Obj = payouts;
                    result.CreateResult = CreateResult.Success;
                },
                null,
                LogSource.GoodRequestService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>()
                    .LogWarning(LogSource.ImageProcessor, string.Format("Get good request fail. Ex: {0}.", ex));
            }
            return result;
        }
        public override Result<List<AppekPaymentVoid>> GetTransactionForVoid()
        {
            var result = new Result<List<AppekPaymentVoid>>(CreateResult.Error, new List<AppekPaymentVoid>());
            try
            {
                Uow.Wrap(u =>
                {
                    //EndDate+24h+2h<Current releale time
                    //todo close 2h after end(in DB time of EndDate = 00:00, must substract 24h)
                    var closeDay = DateTime.Now.AddHours(-24 - 2);
                    //var closeDay = DateTime.Now.Date.AddHours(-72);
                    var transaction = (from gr in _repGoodRequest.Table
                                       join gb in _repGoodRequest.TableFor<GoodBooking>() on gr.Id equals gb.GoodRequestId
                                       join pt in _repGoodRequest.TableFor<PaymentTransaction>() on gr.Id equals pt.GoodRequestId
                                       join ug in _repGoodRequest.TableFor<UserGood>() on gr.GoodId equals ug.GoodId
                                       //join us in _repGoodRequest.TableFor<User>() on gr.UserId equals us.Id
                                       //TODO: for fast testing exclude date
                                       //where gr.StatusId == (int)UserRequestStatus.Released && pt.Type == PaymentTransactionType.AuthorizeType
                                       where gb.EndDate <= closeDay && gr.StatusId == (int)UserRequestStatus.Released && pt.Type == PaymentTransactionType.AuthorizeType
                                       select new AppekPaymentVoid
                                       {
                                           GoodsUserId = ug.UserId,
                                           PayerId = pt.PayerId,
                                           PaymentId = pt.TransactionId,
                                           GoodRequestId = pt.GoodRequestId
                                       }).ToList();
                    result.Obj = transaction;
                    result.CreateResult = CreateResult.Success;
                },
                null,
                LogSource.GoodRequestService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>()
                    .LogWarning(LogSource.ImageProcessor, string.Format("Get good request fail. Ex: {0}.", ex));
            }
            return result;
        }
        public virtual Result<List<ReviewEmailViewModel>> GetBookingForReview()
        {
            var result = new Result<List<ReviewEmailViewModel>>(CreateResult.Error, new List<ReviewEmailViewModel>());
            try
            {
                Uow.Wrap(u =>
                {
                    var reviewDate = DateTime.Now.AddHours(-24 - 48);
                    var request = (from gr in _repGoodRequest.Table
                                  join g in _repGood.Table on gr.GoodId equals g.Id
                                  join grb in _repGoodBooking.Table on gr.Id equals grb.GoodRequestId
                                  join gu in _repUserGood.Table on gr.GoodId equals gu.GoodId
                                  join gUser in _repUser.Table on gu.UserId equals gUser.Id
                                  join gUserRequest in _repUser.Table on gr.UserId equals gUserRequest.Id
                                  where grb.EndDate <= reviewDate && (gr.StatusId == (int)UserRequestStatus.Released || gr.StatusId == (int)UserRequestStatus.Closed)
                                  select new ReviewEmailViewModel
                                  {
                                      GoodName = g.Name,
                                      SeekerId = gUserRequest.Id,
                                      SeekerFullName = gUserRequest.FirstName + " " + gUserRequest.LastName,
                                      SeekerEmail = gUserRequest.Email,
                                      SharerId = gUser.Id,
                                      SharerFullName = gUser.FirstName + " " + gUser.LastName,
                                      SharerEmail = gUser.Email,
                                      GoodRequestId = gr.Id
                                  }).Distinct().ToList();
                    result.Obj = request;
                    result.CreateResult = CreateResult.Success;
                },
                null,
                LogSource.GoodRequestService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>()
                    .LogWarning(LogSource.ImageProcessor, string.Format("Get good request fail. Ex: {0}.", ex));
            }
            return result;
        }

        public string getItemPickupLocation(int goodId)        {            string resultPickupLocation = string.Empty;            try            {                Uow.Wrap(u =>                {
                    var getLocation = _repGoodPropertyValue.GetGoodProperties(goodId);                    foreach (var item in getLocation)                    {                        if (item.Key == "MomentarilyItem_Location")                        {                            resultPickupLocation = item.Value.Value;                            break;                        }                    }                },                null,                LogSource.GoodRequestService);            }            catch (Exception ex)            {                Ioc.Get<IDbLogger>()                    .LogWarning(LogSource.ImageProcessor, string.Format("Pick Up Location Not Found. Ex: {0}.", ex));            }            return resultPickupLocation;        }
    }
}
