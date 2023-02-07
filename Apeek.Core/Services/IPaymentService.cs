using System.Collections.Generic;
using Apeek.Common.Interfaces;
using Apeek.Common.Models;
using Apeek.Common.UrlHelpers;
using Apeek.Entities.Entities;
using Apeek.ViewModels.Models;
using Apeek.ViewModels.Models.Impl;
using PayPal.Api;
namespace Apeek.Core.Services
{
    public interface IPaymentService : IDependency
    {
        Result<string> CreateAuthorizePayemnt(ApeekPaymentModel payment, QuickUrl quickUrl);
        Result<string> CapturePayment(double chargeAmount, string paymentId, string payerId);
        Result<BookingRequestModel> ExecutePayment(int goodRequestId, string payerId, string paymentId);
        Result<BookingRequestModel> CapturePayment(int goodRequestId);
        Result<BookingRequestModel> AddTransaction(int goodRequestId, string paymentType, string payerId, string paymentId, string captureId = null);
        Result<PaymentTransaction> GetTransaction(int goodReuestId);
        void SavePayment(Payment payment);
        bool CreateAdaptivePayment(int userId, int goodRequestId, QuickUrl quickUrl);
        bool VoidAuthorizePayment(string paymentId, string payerId);
        bool RefundAmount(string captureId, double refundAmount);
        Result<bool> Payout(ApeekPayout payout, string returnUrl, string cancelUrl);
        PaypalPayment SaveUpdatePaypalPayment(PaypalPayment payment);
        List<PaypalPayment> GetAllPaypalPayments();
        PaypalPayment GetPaypalPayment(int goodRequestId);
        void saveWebhookResponse(string requestJson);
        bool SaveFinalFeedback(FinalFeedbackVM feedback,int userId);
        FinalFeedback GetFinalFeedbackbyRequestId(int requestId);
    }
}
