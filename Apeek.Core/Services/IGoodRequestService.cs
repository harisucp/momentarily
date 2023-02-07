using System;
using System.Collections.Generic;
using Apeek.Common.Interfaces;
using Apeek.Common.Models;
using Apeek.Common.UrlHelpers;
using Apeek.Core.Services.Impl;
using Apeek.Entities.Entities;
using Apeek.NH.DataAccessLayer.DataAccess;
using Apeek.ViewModels.Models;
using Apeek.ViewModels.Models.Impl;
using Momentarily.ViewModels.Models.Braintree;

namespace Apeek.Core.Services
{
    public interface IGoodRequestService : IDependency
    {
        Result<GoodRequestViewModel> GetGoodRequest(int userId, int requestId);
        Result<GoodRequestViewModel> GetGoodRequest(int requestId);
        List<GoodRequest> GetAllGoodRequest();
        Result<BookingListViewModel> GetGoodRequests(int userId, int goodId);
        Result<GoodRequestViewModel> GetUserRequest(int userId, int requestId);
        Result<IEnumerable<GoodRequestViewModel>> GetUserRequests(int userId);
        Result<GoodRequest> GetRequest(int requestId);
        bool ApproveGoodRequest(int userId, int requestId);
        bool DeclineGoodRequest(int userId, int requestId);
        bool NotResponedGoodRequest();
        bool CloseGoodRequest(int userId, int requestId);
        bool CancelGoodRequest(int userId, int requestId);

        bool CancelGoodRequestBeforePayment(int userId, int requestId);
        bool CancelUserRequest(int userId, int requestId);
        bool CancelUserRequestBeforePayment(int userId, int requestId);
        bool ReleaseGoodRequest(int userId, int requestId);
        bool ReceiveGoodRequest(int userId, int requestId);
        bool ReturnGoodRequest(int userId, int requestId);
        bool ReturnConfirmGoodRequest(int userId, int requestId);
        bool SeekerPaidUserRequest(int userId, int requestId);
        bool ReviewingUserRequest(int userId, int reuqestId);
        bool SharerTakeUserRequest(int userId, int requestId,IUnitOfWork unitOfWork = null);
        Result<RequestViewModel> BaseGetGoodRequest(int userId, RequestModel requestModel, IUnitOfWork unitOfWork = null);
        Result<GoodRequest> SaveGoodRequest(int userId, RequestViewModel requestModel);
        bool CanSeekerCancel(int statusId);
        bool CanSharerCancel(int statusId, DateTime createDate);
        bool CanSharerStartDispute(int statusId, DateTime endDate);
        bool CanSeekerStartDispute(int statusId, DateTime startDate, DateTime endDate);
        bool CanSeekerReview(int userId, int goodRequestId, int statusId, DateTime endDate);
        bool CanSharerReview(int userId, int goodRequestId, int statusId, DateTime endDate);
        //bool AmountChargedUserRequest(int userId, int requestId);
        //bool DepositChargedUserRequest(int userId, int requestId);
        bool NeedRefundAmount(int userId, int requestId);
        Result<IList<ApeekPayout>> GetGoodRequestForPayout();
        Result<List<AppekPaymentVoid>> GetTransactionForVoid();
        bool SharerStartDispute(int userId, int requestId);
        bool SeekerStartDispute(int userId, int requestId);
        Result<bool> SeekerLeavesReview(GoodRequestRankInsertModel reviewModel, int userId, QuickUrl quickUrl);
        Result<bool> SharerLeavesReview(GoodRequestRankInsertModel reviewModel, int userId, QuickUrl quickUrl);
        Result<List<ReviewEmailViewModel>> GetBookingForReview();
        Result<PriceViewModel> CalculatePrice(int GoodId, DateTime StartDate, DateTime EndDate, double ShippingDistance, bool ApplyForDelivery, IUnitOfWork unitOfWork = null);
        Result<PriceViewModel> CalculatePriceOnDiscount(int GoodId, DateTime StartDate, DateTime EndDate, double ShippingDistance, bool ApplyForDelivery);
        bool UpdateGoodRequest(GoodRequest request);

        bool SaveDisputeDetail(RequestChangeStatusViewModel model, int UserId, IUnitOfWork unitOfWork = null);

        bool CheckCouponForcurrentUserToSendThankYouTemplate(string couponCode,int currentUserId,int requestId);
        bool UpdateGoodRequestForCoupon(int currentUserId, int requestId);
        bool AddCancelledRequest(int userId, int requestId);

    }
}
