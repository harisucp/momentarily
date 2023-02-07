using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Apeek.Common;
using Apeek.Common.Definitions;
using Apeek.Common.Logger;
using Apeek.Common.Models;
using Apeek.Common.UrlHelpers;
using Apeek.Entities.Entities;
using Apeek.NH.DataAccessLayer.DataAccess;
using Apeek.NH.Repository.Repositories;
using Apeek.NH.Repository.Repositories.Impl;
using Apeek.ViewModels.Models;
using Apeek.ViewModels.Models.Impl;
using PayPal.AdaptivePayments;
using PayPal.AdaptivePayments.Model;
using PayPal.Api;
namespace Apeek.Core.Services.Impl
{
    public class PayPalPaymentService : IPaymentService
    {
        private readonly ISettingsDataService _settingsService;
        private readonly IRepositoryPaymentTransaction _repPaymentTransaction;
        private readonly IRepositoryPaypalPayment _repositoryPaypalPayment;
        private readonly IRepositoryGoodRequest _repGoodRequest;
        private readonly ISettingsDataService _settingsDataService;
        private readonly IRepositoryUserGood _repUserGood;
        private readonly IRepositoryUser _repUser;
        private readonly IRepositoryWebhookResponse _repWebhookResponse;
        private readonly IRepositoryFinalFeedback _repFinalFeedback;
        private APIContext _apiContext;
        public PayPalPaymentService(ISettingsDataService settingsService,
            IRepositoryPaymentTransaction repPaymentTransaction,
            IRepositoryGoodRequest repGoodRequest, ISettingsDataService settingsDataService,
            IRepositoryUserGood repUserGood, IRepositoryUser repUser,
            RepositoryPaypalPayment repositoryPaypalPayment,
            IRepositoryWebhookResponse repositoryWebhookResponse, IRepositoryFinalFeedback repFinalFeedback)
        {
            _settingsService = settingsService;
            _repPaymentTransaction = repPaymentTransaction;
            _repositoryPaypalPayment = repositoryPaypalPayment;
            _repGoodRequest = repGoodRequest;
            _settingsDataService = settingsDataService;
            _repUserGood = repUserGood;
            _repUser = repUser;
            _repWebhookResponse = repositoryWebhookResponse;
            _repFinalFeedback = repFinalFeedback;
        }
        public APIContext ApiContext
        {
            get { return _apiContext ?? (_apiContext = GetApiContext(_settingsService)); }
            set { _apiContext = value; }
        }
        private APIContext GetApiContext(ISettingsDataService settingService)
        {
            var paypalSetting = settingService.GetPayPalSetting();
            var accessToken = new OAuthTokenCredential(paypalSetting.ClientId,
                paypalSetting.ClientSecret, paypalSetting.Config).GetAccessToken();
            var apiContext = new APIContext(accessToken)
            {
                Config = paypalSetting.Config
            };
            return apiContext;
        }
        private Dictionary<string, string> _apiConfig;
        public Dictionary<string, string> ApiConfig
        {
            get { return _apiConfig ?? (_apiConfig = _settingsService.GetPayPalSetting().Config); }
            set { _apiConfig = value; }
        }
        private string _currency;
        public string Currency
        {
            get { return _currency ?? (_currency = _settingsService.GetPaymentTransactionCurrency()); }
            set { _currency = value; }
        }
        public Result<string> CapturePayment(double chargeAmount, string paymentId, string payerId)
        {
            var result = new Result<string>(CreateResult.Error, null);
            try
            {
                Payment payment = Payment.Execute(ApiContext, paymentId, new PaymentExecution() { payer_id = payerId });
                //Payment holdPayment = Payment.Get(ApiContext, paymentId);
                Authorization authorize = Authorization.Get(ApiContext, payment.transactions[0].related_resources[0].authorization.id);
                Amount captureAmount = new Amount
                {
                    currency = Currency,
                    total = chargeAmount.ToString("F2", CultureInfo.InvariantCulture),
                    details = new Details { tax = "0", shipping = "0" }
                };
                Capture capture = new Capture
                {
                    amount = captureAmount,
                    parent_payment = paymentId,
                    is_final_capture = false
                };
                Capture authCapture = authorize.Capture(ApiContext, capture);
                string state = authCapture.state;
                result.Obj = authCapture.id;
                result.CreateResult = CreateResult.Success;
                Ioc.Get<IDbLogger>().LogMessage(LogSource.PayPalPaymentService, string.Format("Capture payment paymentId: {0} payerId: {1} charge amount: {2}", paymentId, payerId, chargeAmount));
                return result;
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.PayPalPaymentService, string.Format("Create payment fail. Ex: {0}.", ex));
            }
            return result;
        }
        public bool RefundAmount(string captureId, double refundAmount)
        {
            try
            {
                Capture capture = Capture.Get(ApiContext, captureId);
                Amount amount = new Amount
                {
                    currency = Currency,
                    total = refundAmount.ToString("F2", CultureInfo.InvariantCulture),
                    details = new Details { tax = "0", shipping = "0" }
                };
                Refund refund = new Refund
                {
                    amount = amount
                };
                refund = capture.Refund(ApiContext, refund);
                string state = refund.state;
                Ioc.Get<IDbLogger>().LogMessage(LogSource.PayPalPaymentService,string.Format("Refund amount: {0} captureId: {1}", refundAmount, captureId));
                return true;
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.PayPalPaymentService, string.Format("Refund amount fail. Ex: {0}.", ex));
            }
            return false;
        }
        public Result<string> CreateAuthorizePayemnt(ApeekPaymentModel payment, QuickUrl quickUrl)
        {
            var result = new Result<string>(CreateResult.Error, null);
            try
            {
                string returnUrl = quickUrl.RequestConfirmUrl();
                var itemList = new ItemList
                {
                    items = new List<Item>
                    {
                        new Item
                        {
                            sku = payment.GoodId.ToString(),
                            name = "Booking for listing " + payment.GoodName,
                            //description = payment.GoodDescription,
                            currency = Currency,
                            price = (payment.Cost + (payment.Cost * payment.ServiceFee)).ToString("F2", CultureInfo.InvariantCulture),
                            quantity = "1"
                        },
                        new Item
                        {
                            sku = payment.GoodId.ToString(),
                            name ="Security deposit for listing " + payment.GoodName,
                            //description = payment.GoodDescription,
                            currency = Currency,
                            price = payment.SecurityDeposit.ToString("F2", CultureInfo.InvariantCulture),
                            quantity = "1"
                        }
                    }
                };
                var payer = new Payer
                {
                    payment_method = "paypal"
                };
                var redirUrls = new RedirectUrls
                {
                    cancel_url = returnUrl + "?success=false&utm_nooverride=1&goodRequestId=" + payment.RequestId,
                    return_url = returnUrl + "?success=true&utm_nooverride=1&goodRequestId=" + payment.RequestId
                };
                var amount = new Amount
                {
                    currency = Currency,
                    total = (payment.Cost + (payment.Cost * payment.ServiceFee) + payment.SecurityDeposit).ToString("F2", CultureInfo.InvariantCulture),
                    details = new Details
                    {
                        tax = "0",
                        shipping = "0"
                        //subtotal = (payment.Cost + (payment.Cost * payment.ServiceFee) + payment.SecurityDeposit).ToString("0.00")
                    }
                };
                var transactionList = new List<Transaction>
                {
                    new Transaction
                    {
                        description = "Order Good",
                        invoice_number = payment.RequestId + (new Random().Next(1000000)).ToString(CultureInfo.InvariantCulture),
                        amount = amount,
                        item_list = itemList
                    }
                };
                var paypalPayment = new Payment
                {
                    intent = payment.Type,
                    payer = payer,
                    transactions = transactionList,
                    redirect_urls = redirUrls
                };
                paypalPayment = Payment.Create(ApiContext, paypalPayment);
                if (paypalPayment != null)
                {
                    string url = GetApprovalUrl(paypalPayment);
                    if (!string.IsNullOrWhiteSpace(url))
                    {
                        result.CreateResult = CreateResult.Success;
                        result.Obj = url;
                    }
                }
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.PayPalPaymentService, string.Format("Create payment fail. Ex: {0}.", ex));
            }
            return result;
        }
        private string GetApprovalUrl(Payment payPalPayment)
        {
            string payPalUrl = null;
            var links = payPalPayment.links.GetEnumerator();
            while (links.MoveNext())
            {
                var lnk = links.Current;
                if (lnk != null && lnk.rel.ToLower().Trim().Equals("approval_url"))
                {
                    payPalUrl = lnk.href;
                }
            }
            return payPalUrl;
        }
        public Result<BookingRequestModel> ExecutePayment(int goodRequestId, string payerId, string paymentId)
        {
            try
            {
                var paymentExecution = new PaymentExecution
                {
                    payer_id = payerId
                };
                var payment = new Payment
                {
                    id = paymentId
                };
                var executedPayment = payment.Execute(ApiContext, paymentExecution);
                if (executedPayment != null)
                {
                    Ioc.Get<IDbLogger>().LogMessage(LogSource.PayPalPaymentService,string.Format("Execute payment. goodRequestId: {0} payerID: {1} paymentId: {2}",goodRequestId, payerId, paymentId));
                    return AddTransaction(goodRequestId, executedPayment.intent, payerId,
                        paymentId);
                }
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.PayPalPaymentService, string.Format("Execute payment fail. Ex: {0}.", ex));
            }
            return null;
        }
        private PayResponse CreatePayout(ApeekPayout payout, string returnUrl, string cancelUrl)
        {
            try
            {
                var actionType = "PAY";
                //TODO add reutrnUrl
                returnUrl = returnUrl + "?success=true";
                cancelUrl = cancelUrl + "?success=false";
                var currencyCode = Currency;
                var receiverList = new ReceiverList()
                {
                    receiver = new List<Receiver>
                    {
                        new Receiver
                        {
                            email = payout.Email, amount = (decimal)payout.Amount
                        }
                    }
                };
                var requestEnvelope = new RequestEnvelope("en_US");
                var payRequest = new PayRequest(requestEnvelope, actionType, cancelUrl, currencyCode, receiverList,
                    returnUrl)
                {
                    senderEmail = _settingsDataService.GetPayPalSetting().EmailId
                };
                var adaptivePaymentsService = new AdaptivePaymentsService(ApiConfig);
                return adaptivePaymentsService.Pay(payRequest);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.PayPalPaymentService, string.Format("Create adaptive payment fail. Ex: {0}.", ex));
            }
            return null;
        }
        public Result<bool> Payout(ApeekPayout payout, string returnUrl, string cancelUrl)
        {
            var result = new Result<bool>(CreateResult.Error, false);
            try
            {
                Uow.Wrap(u =>
                {
                    var payResponse = CreatePayout(payout, returnUrl, cancelUrl);
                    if (payResponse != null)
                    {
                        if (payResponse.paymentExecStatus == "COMPLETED")
                        {
                            var request = _repGoodRequest.Get(payout.GoodRequestId);
                            if (request != null)
                            {
                                request.StatusId = (int)UserRequestStatus.Released;
                                _repGoodRequest.Update(request);
                                result.CreateResult = CreateResult.Success;
                                result.Obj = true;
                            }
                            Ioc.Get<IDbLogger>().LogMessage(LogSource.PayPalPaymentService,string.Format("Payout for email: {0} with amount: {1} for request: {2} complete",payout.Email, payout.Amount, payout.GoodRequestId));
                        }
                        else
                        {
                            //TODO: write payment error to log
                            Ioc.Get<IDbLogger>().LogWarning(LogSource.PayPalPaymentService, string.Format("Execute payouts not completed. paykey: {0} good request: {1}", payResponse.payKey, payout.GoodRequestId));
                        }
                    }
                },
                null,
                LogSource.PayPalPaymentService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.PayPalPaymentService, string.Format("Execute payouts fail. Ex: {0}.", ex));
            }
            return result;
        }
        private PayResponse CreatePayment(string payPalEmail, decimal amount, string action, string currency, QuickUrl quickUrl)
        {
            try
            {
                var actionType = action;
                var returnUrl = quickUrl.AdaptivePaymentConfirmAbsoluteUrl() + "?success=true";
                var cancelUrl = quickUrl.AdaptivePaymentConfirmAbsoluteUrl() + "?success=false";
                var currencyCode = currency;
                var receiverList = new ReceiverList
                {
                    receiver = new List<Receiver>
                    {
                        new Receiver
                        {
                            email = payPalEmail,
                            amount = amount
                        }
                    }
                };
                var requestEnvelope = new RequestEnvelope("en_US");
                var payRequest = new PayRequest(requestEnvelope, actionType, cancelUrl, currencyCode, receiverList,
                    returnUrl)
                {
                    senderEmail = _settingsDataService.GetPayPalSetting().EmailId
                };
                var adaptivePaymentsService = new AdaptivePaymentsService(ApiConfig);
                return adaptivePaymentsService.Pay(payRequest);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.PayPalPaymentService, string.Format("Create adaptive payment fail. Ex: {0}.", ex));
            }
            return null;
        }
        public bool CreateAdaptivePayment(int userId, int goodRequestId, QuickUrl quickUrl)
        {
            var result = false;
            try
            {
                Uow.Wrap(u =>
                {
                    var paymentTransaction =
                        _repPaymentTransaction.Table.FirstOrDefault(p => p.GoodRequestId == goodRequestId);
                    if (paymentTransaction != null &&
                        paymentTransaction.GoodRequest.StatusId == (int)UserRequestStatus.Paid)
                    {
                        var userGood = _repUserGood.Table
                            .FirstOrDefault(p => p.GoodId == paymentTransaction.GoodRequest.GoodId);
                        if (userGood != null && userGood.UserId == userId)
                        {
                            var user = _repUser.GetUser(userGood.UserId);
                            if (user != null)
                            {
                                var payResponse = CreatePayment(user.Email,
                                     (decimal)paymentTransaction.Cost, "PAY", Currency, quickUrl);
                                if (payResponse != null)
                                {
                                    if (payResponse.paymentExecStatus == "COMPLETED")
                                    {
                                        //TODO: 
                                        //result = VoidAuthorizePayment(paymentTransaction.TransactionId,
                                        //    paymentTransaction.PayerId);
                                    }
                                }
                            }
                        }
                    }
                },
                null,
                LogSource.PayPalPaymentService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.PayPalPaymentService, string.Format("Execute adaptive payment fail. Ex: {0}.", ex));
            }
            return result;
        }
        public Result<BookingRequestModel> CapturePayment(int goodRequestId)
        {
            var result = new Result<BookingRequestModel>(CreateResult.Error, new BookingRequestModel());
            try
            {
                Uow.Wrap(u =>
                {
                    var paymentTransaction = _repPaymentTransaction.Table.FirstOrDefault(p => p.GoodRequestId == goodRequestId);
                    if (paymentTransaction != null)
                        result = ExecutePayment(paymentTransaction.GoodRequestId,
                            paymentTransaction.PayerId, paymentTransaction.TransactionId);
                },
                null,
                LogSource.PayPalPaymentService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.PayPalPaymentService, string.Format("Execute payment fail. Ex: {0}.", ex));
            }
            return result;
        }
        public Result<BookingRequestModel> AddTransaction(int goodRequestId, string paymentType, string payerId, string paymentId, string captureId = null)
        {
            var result = new Result<BookingRequestModel>(CreateResult.Error, new BookingRequestModel());
            var commision = _settingsDataService.GetBorrowerPaymentTransactionCommision();
            try
            {
                Uow.Wrap(u =>
                {
                    Ioc.Get<IDbLogger>().LogMessage(LogSource.PayPalPaymentService, string.Format("Trying to add transaction."));
                    var goodRequest = _repGoodRequest.Get(goodRequestId);
                    if (goodRequest != null)
                    {
                        var paymentTransaction = new PaymentTransaction
                        {
                            TransactionId = paymentId,
                            PayerId = payerId,
                            GoodRequestId = goodRequest.Id,
                            Type = paymentType,
                            Cost = goodRequest.DaysCost,
                            Commision = goodRequest.DaysCost * commision,
                            CapureId = captureId,
                            StatusId = (int)PaymentTransactionStatus.Pending
                        };
                        _repPaymentTransaction.SaveOrUpdateAudit(paymentTransaction, goodRequest.UserId);
                        var userGood = _repUserGood.Table
                            .FirstOrDefault(p => p.GoodId == goodRequest.GoodId);
                        if (userGood != null)
                            result.Obj = new BookingRequestModel
                            {
                                GoodRequest = goodRequest,
                                Good = goodRequest.Good,
                                UserSeller = userGood.User,
                                UserBuyer = goodRequest.User
                            };
                        result.CreateResult = CreateResult.Success;
                    }
                },
                null,
                LogSource.PayPalPaymentService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.PayPalPaymentService, string.Format("Add transaction fail. Ex: {0}.", ex));
            }
            return result;
        }
        public bool VoidAuthorizePayment(string paymentId, string payerId)
        {
            try
            {
                Payment payment = Payment.Get(ApiContext, paymentId);
                Authorization authorize = Authorization.Get(ApiContext, payment.transactions[0].related_resources[0].authorization.id);
                authorize = authorize.Void(ApiContext);
                var state = authorize.state;
                if (state == "voided")
                {
                    Ioc.Get<IDbLogger>().LogMessage(LogSource.PayPalPaymentService, string.Format("Void paymentId: {0} payerId: {1}", paymentId, payerId));
                    return true;
                }
                else
                {
                    //Todo: log state, payment id
                    Ioc.Get<IDbLogger>().LogWarning(LogSource.PayPalPaymentService, string.Format("Void authorize state:{0} PaymentId: {1}.", state, paymentId));
                }
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogError(LogSource.PayPalPaymentService, string.Format("Create payment fail. Ex: {0}.", ex));
            }
            return false;
        }
        public void SavePayment(Payment payment)
        {
            if (payment != null)
                using (var streamWriter = new StreamWriter(
                            new FileStream(AppDomain.CurrentDomain.BaseDirectory + "paymentLog.txt",
                                FileMode.Append, FileAccess.Write, FileShare.ReadWrite),
                                Encoding.UTF8))
                {
                    streamWriter.WriteLine(DateTime.Now.ToLongDateString());
                    streamWriter.WriteLine(payment.ConvertToJson());
                    streamWriter.WriteLine();
                }
        }
        public Result<PaymentTransaction> GetTransaction(int goodReuestId)
        {
            var result = new Result<PaymentTransaction>(CreateResult.Error, null);
            try
            {
                Uow.Wrap(u =>
                {
                    var trans = _repPaymentTransaction.Table.FirstOrDefault(t => t.GoodRequestId == goodReuestId);
                    if (trans != null)
                    {
                        result.Obj = trans;
                        result.CreateResult = CreateResult.Success;
                    }
                },
                null,
                LogSource.PayPalPaymentService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.PayPalPaymentService, string.Format("Get  payment transaction fail. Ex: {0}.", ex));
            }
            return result;
        }

        public PaypalPayment SaveUpdatePaypalPayment(PaypalPayment payment)
        {
            var data = new PaypalPayment();
            try
            {
                Uow.Wrap(u =>
                {
                    Ioc.Get<IDbLogger>().LogMessage(LogSource.PayPalPaymentService, string.Format("Trying to add payment transaction."));
                    data = _repositoryPaypalPayment.SaveOrUpdate(payment);
                },
               null,
               LogSource.PayPalPaymentService);
            }
            catch(Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.PayPalPaymentService, string.Format("Get  payment transaction fail. Ex: {0}.", ex));
            }
            return data;
        }

        public List<PaypalPayment> GetAllPaypalPayments()
        {
            List<PaypalPayment> payments = new List<PaypalPayment>();
            try
            {
                Uow.Wrap(u =>
                {
                    payments = _repositoryPaypalPayment.Table.ToList();
                },
                  null,
                  LogSource.PayPalPaymentService);
            }
            catch
            {

            }
            return payments;
        }

        public PaypalPayment GetPaypalPayment(int goodRequestId)
        {
            PaypalPayment payment = new PaypalPayment();
            try
            {
                Uow.Wrap(u =>
                {
                    payment = _repositoryPaypalPayment.Table.FirstOrDefault(p => p.GoodRequestId == goodRequestId);
                },
                  null,
                  LogSource.PayPalPaymentService);
            }
            catch
            {

            }
            return payment;
        }
        public void saveWebhookResponse(string requestResponse)        {            WebhookResponse webhookResponse = new WebhookResponse();            webhookResponse.Json = requestResponse;            webhookResponse.CreateDate = DateTime.Now;            webhookResponse.ModDate = DateTime.Now;            try            {                Uow.Wrap(u =>                {                    webhookResponse = _repWebhookResponse.SaveOrUpdate(webhookResponse);                },                  null,                  LogSource.PayPalPaymentService);            }            catch(Exception ex)            {            }        }

        public bool SaveFinalFeedback(FinalFeedbackVM feedback, int userId)        {            var result = false;            try            {                Uow.Wrap(u =>                {
                    Ioc.Get<IDbLogger>().LogMessage(LogSource.UserNotificationService, string.Format("Trying to submit good request feedback."));                    var requestFeedback = _repFinalFeedback.Table.Where(x => x.RequestId == feedback.RequestId).FirstOrDefault();                    var goodRequest = _repGoodRequest.Table.Where(x => x.Id == feedback.RequestId).FirstOrDefault();                    if (requestFeedback != null && goodRequest != null)                    {                        if (feedback.NoIssue)                        {                            requestFeedback.NoIssue = true;                            requestFeedback.Late = false;                            requestFeedback.Damaged = false;                            goodRequest.StatusId = (int)UserRequestStatus.ReturnConfirmed;                        }                        else if (feedback.Late && !feedback.Damaged)                        {                            requestFeedback.Late = true;                            requestFeedback.NoIssue = false;                            requestFeedback.Damaged = false;                            goodRequest.StatusId = (int)UserRequestStatus.Late;                        }                        else if (feedback.Damaged && !feedback.Late)                        {                            requestFeedback.Damaged = true;                            requestFeedback.Late = false;                            requestFeedback.NoIssue = false;                            goodRequest.StatusId = (int)UserRequestStatus.Damaged;                        }                        else if (feedback.Late && feedback.Damaged)                        {                            requestFeedback.Damaged = true;                            requestFeedback.Late = true;                            requestFeedback.NoIssue = false;                            goodRequest.StatusId = (int)UserRequestStatus.LateAndDamaged;                        }                        requestFeedback.Description = feedback.Description == null ? "" : feedback.Description;                        requestFeedback.Claim = feedback.Claim;                        requestFeedback.ReturnDate = feedback.ReturnDate;                        requestFeedback.ReturnTime = feedback.ReturnTime;                        requestFeedback.ModDate = DateTime.Now;
                        _repFinalFeedback.SaveOrUpdate(requestFeedback);
                        _repGoodRequest.SaveOrUpdate(goodRequest);                    }                    else                    {                        FinalFeedback feedbackentity = new FinalFeedback();                        feedbackentity.RequestId = feedback.RequestId;                        feedbackentity.NoIssue = feedback.NoIssue;                        feedbackentity.Late = feedback.Late;                        feedbackentity.Damaged = feedback.Damaged;                        feedbackentity.ReturnDate = feedback.ReturnDate;                        feedbackentity.ReturnTime = feedback.ReturnTime;                        feedbackentity.Claim = feedback.Claim;                        feedbackentity.Description = feedback.Description == null ? "" : feedback.Description;                        feedbackentity.CreateDate = DateTime.Now;                        feedbackentity.ModDate = DateTime.Now;                        feedbackentity.CreateBy = userId;                        feedbackentity.ModBy = userId;                        var checkFinalFeedback = _repFinalFeedback.Save(feedbackentity);                        if (goodRequest != null)                        {                            if (feedback.NoIssue)                            {                                goodRequest.StatusId = (int)UserRequestStatus.ReturnConfirmed;                            }                            else if (feedback.Late && !feedback.Damaged)                            {                                goodRequest.StatusId = (int)UserRequestStatus.Late;                            }                            else if (!feedback.Late && feedback.Damaged)                            {                                goodRequest.StatusId = (int)UserRequestStatus.Damaged;                            }                            else if (feedback.Late && feedback.Damaged)                            {                                goodRequest.StatusId = (int)UserRequestStatus.LateAndDamaged;                            }                            _repGoodRequest.SaveOrUpdate(goodRequest);                        }                    }                    result = true;                },                null,                LogSource.GoodRequestService);            }            catch (Exception ex)            {                Ioc.Get<IDbLogger>()                    .LogWarning(LogSource.ImageProcessor, string.Format("Update good request feedback fail. Ex: {0}.", ex));            }            return result;        }

        public FinalFeedback GetFinalFeedbackbyRequestId(int requestId)        {            var result = new FinalFeedback();            try            {                Uow.Wrap(u =>                {

                    result = _repFinalFeedback.Table.Where(x=>x.RequestId==requestId).FirstOrDefault();                },                null,                LogSource.GoodRequestService);            }            catch (Exception ex)            {                Ioc.Get<IDbLogger>()                    .LogWarning(LogSource.ImageProcessor, string.Format("Get good request feedback fail. Ex: {0}.", ex));            }            return result;        }
    }
}
