
using System.Globalization;
using System.Web.Mvc;
using Apeek.Common;
using Apeek.Common.Definitions;
using Apeek.Core.Services;
using Apeek.ViewModels.Models.Impl;
using Apeek.Web.Framework;
using Apeek.Web.Framework.Controllers;
using Momentarily.UI.Service.Services;
using Momentarily.UI.Service.Services.Impl;
using Momentarily.ViewModels.Models.Braintree;
using Momentarily.Core.Services.Impl;
using System;
using System.Net;
using Apeek.Entities.Entities;
using System.Collections.Generic;
using Apeek.Core.Services.Impl;
using PayPal.Api;
using Momentarily.ViewModels.Models;
using Newtonsoft.Json;
using RestSharp;
using Apeek.Web.Framework.ControllerHelpers;
using Apeek.Common.Logger;
using Apeek.NH.Repository.Repositories;
//using PayPal.Api;
namespace Momentarily.Web.Areas.Frontend.Controller
{
    [RedirectNotConfirmedFilter]
    public class PaymentController : FrontendController
    {
        private readonly IMomentarilyGoodRequestService _goodRequestService;
        private readonly IMomentarilyUserMessageService _userMessageService;
        private readonly IPaymentService _paymentService;
        private readonly ISendMessageService _sendMessageService;
        private readonly IMomentarilyItemDataService _goodDataService;
        private readonly IMomentarilyAccountDataService _accountDataService;
        private readonly PinPaymentService _pinPaymentService;
        private readonly IPinPaymentStoreDataService _pinPaymentStoreDataService;
        private readonly BraintreePaymentsService _braintreePaymentsService;
        private readonly IMomentarilyItemDataService _itemDataService;
        private readonly AccountControllerHelper<MomentarilyRegisterModel> _helper;
        protected readonly IRepositoryGoodRequest _repGoodRequest;
        private readonly ITwilioNotificationService _twilioNotificationService;

        public PaymentController(IMomentarilyGoodRequestService goodRequestService, IMomentarilyUserMessageService userMessageService,
            IPaymentService paymentService, ISendMessageService sendMessageService,
            IMomentarilyItemDataService goodDataService,
            IMomentarilyAccountDataService accountService,
            IPinPaymentStoreDataService pinPaymentStoreDataService,
            IMomentarilyItemDataService itemDataService,
            IRepositoryGoodRequest repGoodRequest,
            ITwilioNotificationService twilioNotificationService
            )
        {
            _goodRequestService = goodRequestService;
            _userMessageService = userMessageService;
            _paymentService = paymentService;
            _sendMessageService = sendMessageService;
            _goodDataService = goodDataService;
            _accountDataService = accountService;
            _pinPaymentStoreDataService = pinPaymentStoreDataService;
            _itemDataService = itemDataService;
            _pinPaymentService = new PinPaymentService();
            _braintreePaymentsService = new BraintreePaymentsService();
            _helper = new AccountControllerHelper<MomentarilyRegisterModel>();
            _repGoodRequest = repGoodRequest;
            _twilioNotificationService = twilioNotificationService;
        }
        [Authorize]
        [HttpGet]
        public ActionResult GetBookingPayment(int goodRequestId)
        {
            if (!UserId.HasValue || !UserAccess.HasAccess(Privileges.CanViewUsers, UserId))
                return RedirectToHome();
            var result = _goodRequestService.GetUserRequest(UserId.Value, goodRequestId);
            if (result.CreateResult == CreateResult.Success)
            {
                var good = _goodDataService.GetItem(result.Obj.GoodId);
                if (good.CreateResult == CreateResult.Success)
                {
                    var payment = new ApeekPaymentModel
                    {
                        GoodId = good.Obj.GoodId,
                        RequestId = goodRequestId,
                        Type = PaymentTransactionType.AuthorizeType,
                        Cost = result.Obj.DaysCost,
                        SecurityDeposit = good.Obj.Deposit,
                        GoodDescription = good.Obj.Description,
                        GoodName = good.Obj.Name,
                        ServiceFee = Ioc.Get<ISettingsDataService>().GetBorrowerPaymentTransactionCommision()
                    };
                    var paymentResult = _paymentService.CreateAuthorizePayemnt(payment, QuickUrl);
                    if (paymentResult.CreateResult == CreateResult.Success)
                    {
                        return Redirect(paymentResult.Obj);
                    }
                }
            }
            return RedirectToAction("Request", "Booking", new { id = goodRequestId });
        }
        [Authorize]
        [HttpGet]
        public ActionResult BookingRequestConfirm()
        {
            if (!UserId.HasValue || !UserAccess.HasAccess(Privileges.CanViewUsers, UserId))
                return RedirectToHome();
            var isSuccess = bool.Parse(Request.Params["success"]);
            var goodRequestId = Request.Params["goodRequestId"] != null ? int.Parse(Request.Params["goodRequestId"]) : 0;
            if (isSuccess)
            {
                var payerId = Request.Params["PayerID"];
                var paymentId = Request.Params["paymentId"];
                if (goodRequestId != 0)
                {
                    var goodRequest = _goodRequestService.GetUserRequest(UserId.Value, goodRequestId);
                    if (goodRequest.CreateResult == CreateResult.Success)
                    {
                        var good = _goodDataService.GetItem(goodRequest.Obj.GoodId);
                        if (good.CreateResult == CreateResult.Success)
                        {
                            var serviceFee = Ioc.Get<ISettingsDataService>().GetBorrowerPaymentTransactionCommision();
                            var result = _paymentService.CapturePayment(goodRequest.Obj.DaysCost + goodRequest.Obj.DaysCost * serviceFee, paymentId, payerId);
                            if (result.CreateResult == CreateResult.Success)
                            {
                                _paymentService.AddTransaction(goodRequestId, PaymentTransactionType.AuthorizeType, payerId, paymentId, result.Obj);
                                if (goodRequest.Obj.StatusId == (int)UserRequestStatus.Approved)
                                {
                                    _goodRequestService.SeekerPaidUserRequest(UserId.Value, goodRequestId);
                                    _userMessageService.SendPayGoodRequestMessage(UserId.Value, good.Obj.User.Id, goodRequestId, QuickUrl);
                                    _sendMessageService.SendEmailPayRequestMessage(good.Obj.User.Email, good.Obj.Name,
                                        QuickUrl.GoodRequestAbsoluteUrl(goodRequestId));
                                    var seekerUserInfo = _accountDataService.GetUser(UserId.Value);
                                    var sharerUserInfo = _accountDataService.GetUser(good.Obj.User.Id);
                                    _sendMessageService.SendConfirmEmailToSharer(sharerUserInfo.Email,
                                        good.Obj.Name,
                                        goodRequest.Obj.StartDate.ToString("MM/dd/yyyy"),
                                        goodRequest.Obj.EndDate.ToString("MM/dd/yyyy"),
                                        goodRequest.Obj.DaysCost.ToString("F2", CultureInfo.InvariantCulture),
                                        good.Obj.PickUpLoaction,
                                        new UserContactInfo
                                        {
                                            Email = seekerUserInfo.Email,
                                            Name = seekerUserInfo.FirstName,
                                            Phone = _accountDataService.GetUserPhone(UserId.Value)
                                        }, QuickUrl.GoodRequestAbsoluteUrl(goodRequestId));
                                    _sendMessageService.SendConfirmEmailToSeeker(seekerUserInfo.Email,
                                        good.Obj.Name,
                                        goodRequest.Obj.StartDate.ToString("MM/dd/yyyy"),
                                        goodRequest.Obj.EndDate.ToString("MM/dd/yyyy"),
                                        goodRequest.Obj.DaysCost.ToString("F2", CultureInfo.InvariantCulture),
                                        (goodRequest.Obj.CustomerServiceFee * goodRequest.Obj.DaysCost).ToString("F2", CultureInfo.InvariantCulture),
                                        good.Obj.Deposit.ToString("F2", CultureInfo.InvariantCulture),
                                        (goodRequest.Obj.DaysCost + goodRequest.Obj.CustomerServiceFee * goodRequest.Obj.DaysCost).ToString("F2", CultureInfo.InvariantCulture),
                                        good.Obj.PickUpLoaction,
                                         new UserContactInfo
                                         {
                                             Email = sharerUserInfo.Email,
                                             Name = sharerUserInfo.FirstName,
                                             Phone = _accountDataService.GetUserPhone(good.Obj.User.Id)
                                         },
                                        QuickUrl.UserRequestAbsoluteUrl(goodRequestId)
                                        );
                                }
                            }
                        }
                    }
                }
            }
            return RedirectToAction("Request", "Booking", new { id = goodRequestId });
        }
        [Authorize]
        [HttpGet]
        public ActionResult GetAdaptivePayment(int goodRequestId)
        {
            if (!UserId.HasValue || !UserAccess.HasAccess(Privileges.CanViewUsers, UserId) || goodRequestId == 0)
                return RedirectToHome();
            if (_paymentService.CreateAdaptivePayment(UserId.Value, goodRequestId, QuickUrl))
                _goodRequestService.CloseGoodRequest(UserId.Value, goodRequestId);
            return RedirectToAction("Booking", "Listing", new { id = goodRequestId });
        }
        //[HttpGet]
        //public ActionResult Pay(int goodRequestId)
        //{
        //    if (!UserId.HasValue || !UserAccess.HasAccess(Privileges.CanViewUsers, UserId) || goodRequestId == 0)
        //        return RedirectToHome();
        //    var model = new BraintreePayViewModel();
        //    var getUserRequestResult = _goodRequestService.GetUserRequest(UserId.Value, goodRequestId);
        //    if (getUserRequestResult.CreateResult == CreateResult.Success)
        //    {
        //        model.GoodRequestId = getUserRequestResult.Obj.Id;
        //        var getClientTokenResult = _braintreePaymentsService.GetClientToken();
        //        if (getClientTokenResult.CreateResult == CreateResult.Success)
        //        {
        //            model.ClientToken = getClientTokenResult.Obj.Token;
        //            var shape = _shapeFactory.BuildShape(null, model, PageName.Home.ToString());
        //            return DisplayShape(shape);
        //        }
        //    }
        //    return RedirectToAction("Request", "Booking", new { id = goodRequestId });
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Pay(BraintreePayViewModel model)
        //{
        //    if (!UserId.HasValue || !UserAccess.HasAccess(Privileges.CanViewUsers, UserId) || model.GoodRequestId == 0)
        //        return RedirectToHome();
        //    var payResult = _braintreePaymentsService.Pay(
        //        UserId: UserId.Value,
        //        GoodRequestId: model.GoodRequestId,
        //        PaymentMethodNonce: model.PaymentMethodNonce
        //    );
        //    if (payResult.CreateResult == CreateResult.Success)
        //    {
        //        _goodRequestService.SeekerPaidUserRequest(UserId.Value, model.GoodRequestId);
        //        _braintreePaymentsService.SendNotificationEmails(model.GoodRequestId, UserId.Value, QuickUrl);
        //        return RedirectToAction("Request", "Booking", new { id = model.GoodRequestId });
        //    }
        //    else
        //    {
        //        var shape = _shapeFactory.BuildShape(null, model, PageName.Home.ToString());
        //        shape.IsError = true;
        //        shape.Message = payResult.Message;
        //        return DisplayShape(shape);
        //    }
        //}
        [HttpGet]
        public ActionResult CalculatePrice(int GoodId, string StartDate, string EndDate, double ShippingDistance, bool ApplyForDelivery)
        {
            var calculatePriceResult = _goodRequestService.CalculatePrice(
                 GoodId: GoodId,
                 StartDate: DateTime.ParseExact(StartDate, "MM/dd/yyyy", null),
                 EndDate: DateTime.ParseExact(EndDate, "MM/dd/yyyy", null),
                 ShippingDistance: ShippingDistance,
                 ApplyForDelivery: ApplyForDelivery
             );

            if (calculatePriceResult.CreateResult != CreateResult.Success)
            {
                return Json(new { errorMessage = calculatePriceResult.Message }, JsonRequestBehavior.AllowGet);
            }
            return Json(calculatePriceResult.Obj, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        [HttpGet]
        public ActionResult PaymentCancelled()
        {
            PayResult result = new PayResult();
            result.IsError = true;
            result.Message = "Cancelled";
            result.GoodRequestId = 0;
            result.MailSend = false;
            return View(result);
        }

        [Authorize]
        [HttpGet]
        public ActionResult Pay(int goodRequestId = 0)        {            int currentuserid = 0;
            if (UserId.HasValue)
            {
                int.TryParse(Convert.ToString(UserId.Value), out currentuserid);
            }            if (goodRequestId > 0)            {                var Request = _goodRequestService.GetUserRequest(UserId.Value, goodRequestId);                if (Request != null && Request.Obj.StatusId != (int)UserRequestStatus.Approved)                {                    return RedirectToAction("Request", "Booking", new { id = goodRequestId });                }

            }            APIContext apiContext = PaypalConfiguration.GetAPIContext();            PayResult result = new PayResult();            try            {                string payerId = Request.Params["PayerID"];                if (string.IsNullOrEmpty(payerId))                {                    if (goodRequestId > 0)                    {                        Session["goodRequestId"] = goodRequestId;                    }                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority +                                "/Payment/Pay?";                    string cancelURI = Request.Url.Scheme + "://" + Request.Url.Authority +                               "/Payment/PaymentCancelled";                    var guid = Convert.ToString((new Random()).Next(100000));                    var createdPayment = CreatePayment(apiContext, baseURI + "guid=" + guid, cancelURI, UserId.Value, goodRequestId);                    var links = createdPayment.links.GetEnumerator();                    string paypalRedirectUrl = null;                    while (links.MoveNext())                    {                        Links lnk = links.Current;                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))                        {                            paypalRedirectUrl = lnk.href;                        }                    }                    Session.Add(guid, createdPayment.id);                    return Redirect(paypalRedirectUrl);                }                else                {                    var guid = Request.Params["guid"];

                    // Using the information from the redirect, setup the payment to execute.
                    var paymentId = Session[guid] as string;                    var paymentExecution = new PaymentExecution() { payer_id = payerId };                    var payment = new Payment() { id = paymentId };
                    // Execute authorization.
                    var executedPayment = payment.Execute(apiContext, paymentExecution);// Execute the payment
                    if (executedPayment.state.ToLower() == "approved")                    {
                        var authorization = executedPayment.transactions[0].related_resources[0].authorization;//get Authorization to get payment
                        var capture = CapturePayment(apiContext, payerId, paymentId, authorization, UserId.Value, goodRequestId);//Capture Payment here
                        goodRequestId = (int)Session["goodRequestId"];                        _goodRequestService.SeekerPaidUserRequest(UserId.Value, goodRequestId);
                        var getUserRequestResult = _goodRequestService.GetUserRequest(UserId.Value, goodRequestId);
                        var sendmessage = _userMessageService.SendPayGoodRequestMessage(UserId.Value, getUserRequestResult.Obj.OwnerId, goodRequestId, QuickUrl);

                        string getPickupLocation = _goodRequestService.getItemPickupLocation(getUserRequestResult.Obj.GoodId);                        var item = _itemDataService.GetItem(getUserRequestResult.Obj.GoodId);                        
                        User sharerInfo = _accountDataService.GetUser(getUserRequestResult.Obj.OwnerId);
                        var sharerPhoneNumber = _accountDataService.GetUserPhone(sharerInfo.Id);                        var sharerCountryCode = _accountDataService.GetCountryCodeByPhoneNumber(sharerPhoneNumber);                        User borrowerInfo = _accountDataService.GetUser(UserId.Value);                        var borrowerPhoneNumber = _accountDataService.GetUserPhone(borrowerInfo.Id);                        var borrowerCountryCode = _accountDataService.GetCountryCodeByPhoneNumber(borrowerPhoneNumber);                        string sharerPhone = _accountDataService.getUserPhoneNumberForTemplate(getUserRequestResult.Obj.OwnerId);                        string borrowerPhone = _accountDataService.getUserPhoneNumberForTemplate(UserId.Value);
                        //var send = _sendMessageService.SendPaymentEmailTemplate(executedPayment, getUserRequestResult, item.Obj.GoodImageUrl);
                       
                        _twilioNotificationService.PaymentConfirmation(borrowerPhoneNumber, borrowerCountryCode, borrowerInfo.Id, getUserRequestResult.Obj.CustomerCost.ToString(), getUserRequestResult.Obj.GoodName);
                        _twilioNotificationService.PaymentConfirmation(sharerPhoneNumber, sharerCountryCode, sharerInfo.Id, getUserRequestResult.Obj.CustomerCost.ToString(), getUserRequestResult.Obj.GoodName);

                        var sendInvoceDetail = _sendMessageService.SendPaymentEmailTemplateInvoiceDetail(executedPayment, getUserRequestResult, item.Obj.GoodImageUrl, sharerInfo, borrowerInfo, sharerPhone, borrowerPhone, getPickupLocation);                        var sendInvoceDetailForOwner = _sendMessageService.SendPaymentEmailTemplateInvoiceDetailForOwnerEmail(executedPayment, getUserRequestResult, item.Obj.GoodImageUrl, sharerInfo, borrowerInfo, sharerPhone, borrowerPhone, getPickupLocation);                        int borrowerId = getUserRequestResult.Obj.UserId;                        string borrowerName = getUserRequestResult.Obj.UserName;                        string borrowerEmail = getUserRequestResult.Obj.UserEmail;
                        //string promoCode = "PR0ABCD12";
                        var couponDetail = _accountDataService.GetDetailThankYouCoupon();                        string couponCode = string.Empty;                        double discountAmount = 0;                        string discountType = string.Empty;                        if (couponDetail != null)                        {                            couponCode = couponDetail.CouponCode;                            discountAmount = couponDetail.CouponDiscount;                            discountType = couponDetail.CouponDiscountType == 1 ? "%" : "$";                        }                        else                        {                            couponCode = "XXXXXXX";                            discountAmount = 0;                            discountType = "";                        }                        string ItemListURL = "/Search/Map?Location=&Latitude=0&Longitude=0&SearchByMap=false&NeLatitude=0&NeLongitude=0&SwLatitude=0&SwLongitude=0&DateStart=%2FDate(1575969961000)%2F&DateEnd=%2FDate(1577179561000)%2F&Page=1&PageSize=21&Radius=25&Keyword=&RentPeriod=1&SortBy=1";                        bool checkIdCouponCodeAlreadyUsed = _goodRequestService.CheckCouponForcurrentUserToSendThankYouTemplate(couponCode, currentuserid, goodRequestId);                        if (!checkIdCouponCodeAlreadyUsed)
                        {
                            var sendMsgBorrower = _sendMessageService.SendEmailThankYouTemplate(borrowerId, borrowerName, borrowerEmail, couponCode, ItemListURL, discountAmount, discountType);
                            _goodRequestService.UpdateGoodRequestForCoupon(currentuserid, goodRequestId);                        }                        PaypalPayment paydetail = GetPayDetail(executedPayment, authorization, capture, UserId.Value, item.Obj.User.Id);                        var data = _paymentService.SaveUpdatePaypalPayment(paydetail);
                        // bool refund = RefundCapturedAmount(capture.id, goodRequestId); // Refund captured payment
                        //string userEmail = _itemDataService.getUserEmail(UserId.Value);
                        //bool checkExsistSubscriber = _helper.ExsistSubscriber(userEmail);
                        //if (checkExsistSubscriber == false)
                        //{
                        //    string subscribertURL = "/Account/NewsLetterSubscribe?Email=" + userEmail;
                        //    var sendMsgSubscriber = _sendMessageService.SendEmailNewsLetterTemplate(userEmail, subscribertURL);
                        //}
                        result.IsError = false;                        result.Message = "Payment Successful";                        result.GoodRequestId = goodRequestId;                        result.MailSend = sendInvoceDetail;                        return View(result);                    }                    else                    {                        result.IsError = false;                        result.Message = "Failed";                        result.GoodRequestId = goodRequestId;                        result.MailSend = false;                        return View(result);                    }                }            }            catch (Exception ex)            {                throw ex;            }        }
        private PaypalPayment GetPayDetail(Payment payment, PayPal.Api.Authorization authorization, Capture capture, int payerID, int payeeId)
        {
            PaypalPayment detail = new PaypalPayment();
            detail.PaymentPayId = payment.id;
            detail.GoodRequestId = (int)Session["goodRequestId"];
            detail.PayerId = Convert.ToString(payerID);
            detail.PayeeId = Convert.ToString(payeeId);
            detail.PaymentCart = payment.cart;
            detail.PaymentStatus = payment.state;
            detail.PaymentCreatedDate = Convert.ToDateTime(payment.create_time);
            detail.PaymentIntent = payment.intent;
            detail.PaymentUpdateDate = Convert.ToDateTime(payment.update_time);
            detail.PaymentInvoiceNumber = payment.transactions[0].invoice_number;
            detail.AuthorizationCreatedDate = Convert.ToDateTime(authorization.create_time);
            detail.AuthorizationId = authorization.id;
            detail.AuthorizationCount = 1;
            detail.AuthorizationStatus = authorization.state;
            detail.AuthorizationUpdateDate = Convert.ToDateTime(authorization.update_time);
            detail.AuthorizationValidUntill = Convert.ToDateTime(authorization.valid_until);
            detail.CaptureCreatedDate = Convert.ToDateTime(capture.create_time);
            detail.CaptureId = capture.id;
            detail.CaptureIsFinal = false;
            detail.CaptureStatus = capture.state;
            detail.CaptureUpdateDate = Convert.ToDateTime(capture.update_time);
            detail.CreateDate = DateTime.Now;
            detail.ModDate = DateTime.Now;
            detail.CreateBy = Convert.ToInt32(UserId);
            detail.ModBy = Convert.ToInt32(UserId);
            return detail;
        }

        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            Ioc.Get<IDbLogger>().LogMessage(LogSource.PayPalPaymentService, string.Format("Trying to execute payment."));
            var paymentExecution = new PaymentExecution() { payer_id = payerId };
            var payment = new Payment() { id = paymentId };
            try
            {
                payment.Execute(apiContext, paymentExecution);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.PayPalPaymentService, string.Format("Get execute payment fail. Ex: {0}.", ex));
            }
            return payment;
        }

        private Payment CreatePayment(APIContext apiContext, string redirectUrl, string cancelURI, int userid, int goodrequestid)
        {
            var payment = (dynamic)null;
            try
            {
                Ioc.Get<IDbLogger>().LogMessage(LogSource.PayPalPaymentService, string.Format("Trying to create payment."));
                var getUserRequestResult = _goodRequestService.GetUserRequest(userid, goodrequestid);
                //similar to credit card create itemlist and add item objects to it
                var itemList = new ItemList() { items = new List<Item>() };
                itemList.items.Add(new Item()
                {
                    name = getUserRequestResult.Obj.GoodName,
                    currency = "USD",
                    price = Convert.ToString(getUserRequestResult.Obj.CustomerCost + getUserRequestResult.Obj.SecurityDeposit),
                    quantity = "1",
                    sku = "sku"
                });
                var payer = new Payer() { payment_method = "paypal" };
                // Configure Redirect Urls here with RedirectUrls object
                var redirUrls = new RedirectUrls()
                {
                    cancel_url = cancelURI,
                    return_url = redirectUrl
                };

                var amount = new Amount()
                {
                    currency = "USD",
                    total = Convert.ToString(getUserRequestResult.Obj.CustomerCost + getUserRequestResult.Obj.SecurityDeposit),

                };
                var transactionList = new List<Transaction>();
                transactionList.Add(new Transaction()
                {
                    description = "Request id for this payment is " + getUserRequestResult.Obj.Id,
                    invoice_number = Convert.ToString((new Random()).Next(100000)),
                    amount = amount,
                    item_list = itemList
                });
                payment = new Payment()
                {
                    intent = "authorize",
                    payer = payer,
                    transactions = transactionList,
                    redirect_urls = redirUrls
                };
                // Create a payment using a APIContext

            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.PayPalPaymentService, string.Format("Get create payment fail. Ex: {0}.", ex));
            }
            return payment.Create(apiContext);
        }

        private Capture CapturePayment(APIContext apiContext, string payerId, string paymentId, PayPal.Api.Authorization authorization, int UserId = 0, int goodRequestId = 0)
        {
            Ioc.Get<IDbLogger>().LogMessage(LogSource.PayPalPaymentService, string.Format("Trying to capture payment."));
            var responseCapture = (dynamic)null;
            try
            {
                int tempGoodRequestId = (int)Session["goodRequestId"];
                var getUserRequestResult = _goodRequestService.GetUserRequest(UserId, tempGoodRequestId);

                var capture = new Capture()
                {
                    amount = new Amount()
                    {
                        currency = "USD",
                        total = Convert.ToString(getUserRequestResult.Obj.CustomerCost + getUserRequestResult.Obj.SecurityDeposit)
                    },
                    is_final_capture = false
                };

                responseCapture = authorization.Capture(apiContext, capture);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.PayPalPaymentService, string.Format("Get capture payment fail. Ex: {0}.", ex));
            }
            return responseCapture;
        }

        private void CapturePendingPayments()
        {
            APIContext apiContext = PaypalConfiguration.GetAPIContext();
            var allPayments = _paymentService.GetAllPaypalPayments();
            foreach (var payment in allPayments)
            {
                try
                {
                    var Request = _goodRequestService.GetGoodRequest(payment.GoodRequestId);
                    if (Request != null && Request.Obj != null && Request.Obj.StatusId == (int)UserRequestStatus.Paid || Request.Obj.StatusId == (int)UserRequestStatus.Released
                        || Request.Obj.StatusId == (int)UserRequestStatus.Received || Request.Obj.StatusId == (int)UserRequestStatus.Returned
                        || Request.Obj.StatusId == (int)UserRequestStatus.ReturnConfirmed || Request.Obj.StatusId == (int)UserRequestStatus.Late
                        || Request.Obj.StatusId == (int)UserRequestStatus.Damaged || Request.Obj.StatusId == (int)UserRequestStatus.LateAndDamaged)
                    {
                        DateTime capture_date = payment.ModDate.AddDays(29);
                        if (capture_date.ToString("MM/dd/yyyy") == DateTime.Now.ToString("MM/dd/yyyy"))
                        {
                            PayPal.Api.Authorization authorize = PayPal.Api.Authorization.Get(apiContext, payment.AuthorizationId);
                            var capture = new Capture()
                            {
                                amount = new Amount()
                                {
                                    currency = "USD",
                                    total = Convert.ToString(Request.Obj.CustomerCost + Request.Obj.SecurityDeposit)
                                },
                                is_final_capture = false
                            };
                            var reCapture = authorize.Capture(apiContext, capture);

                            payment.AuthorizationCreatedDate = Convert.ToDateTime(authorize.create_time);
                            payment.AuthorizationId = authorize.id;
                            payment.AuthorizationCount = payment.AuthorizationCount + 1;
                            payment.AuthorizationStatus = authorize.state;
                            payment.AuthorizationUpdateDate = Convert.ToDateTime(authorize.update_time);
                            payment.AuthorizationValidUntill = Convert.ToDateTime(authorize.valid_until);
                            payment.CaptureCreatedDate = Convert.ToDateTime(reCapture.create_time);
                            payment.CaptureId = reCapture.id;
                            payment.CaptureIsFinal = false;
                            payment.CaptureStatus = reCapture.state;
                            payment.CaptureUpdateDate = Convert.ToDateTime(reCapture.update_time);
                            payment.ModDate = DateTime.Now;
                            _paymentService.SaveUpdatePaypalPayment(payment);
                        }
                    }
                }
                catch
                {
                    continue;
                }
            }
        }

        public bool RefundCapturedAmount(string captureId, int goodRequestId)
        {
            try
            {
                var Request = _goodRequestService.GetUserRequest(UserId.Value, goodRequestId);
                APIContext apiContext = PaypalConfiguration.GetAPIContext();
                Capture capture = Capture.Get(apiContext, captureId);
                Amount amount = new Amount();

                amount.currency = "USD";
                amount.total = Convert.ToString(Request.Obj.CustomerCost);
                new Details { tax = "0", shipping = "0" };
                Refund refund = new Refund
                {
                    amount = amount
                };
                refund = capture.Refund(apiContext, refund);
                string state = refund.state;
                return true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        [HttpGet]
        public bool Webhook()
        {
            try
            {
                var requestParams = Request.Params;
                var requestParamsJson = JsonConvert.SerializeObject(requestParams);
                _paymentService.saveWebhookResponse(requestParamsJson);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        [HttpGet]
        public bool RestSharp(APIContext apiContext)
        {
            try
            {
                var amounts = new RestAmount()
                {
                    currency_code = "USD",
                    value = Convert.ToString(100),
                };
                var purchase_unit = new { intent = "CAPTURE", purchase_units = new[] { new { amount = amounts } } };
                var json = JsonConvert.SerializeObject(purchase_unit);
                var client = new RestClient("https://api.sandbox.paypal.com/");
                var request = new RestRequest("v2/checkout/orders", Method.POST);
                request.AddHeader("Content-Type: application/json", "Authorization: " + apiContext.AccessToken);
                //request.AddHeader("Content-Type", "application/json");
                //request.AddHeader("Authorization", apiContext.AccessToken);
                request.Parameters.Clear();
                request.AddParameter("application/json", json, ParameterType.RequestBody);

                IRestResponse response = client.Execute(request);
                var content = response.Content;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
    public class RestAmount
    {
        public string currency_code { get; set; }
        public string value { get; set; }
    }
    public static class PaypalConfiguration
    {
        public readonly static string ClientId;

        public readonly static string ClientSecret;

        static PaypalConfiguration()
        {
            var config = GetConfig();
            ClientId = config["clientId"];
            ClientSecret = config["clientSecret"];
        }
        public static Dictionary<string, string> GetConfig()
        {
            return ConfigManager.Instance.GetProperties();
        }
        private static string GetAccessToken()
        {
            string accessToken = new OAuthTokenCredential
            (ClientId, ClientSecret, GetConfig()).GetAccessToken();
            return accessToken;
        }
        public static APIContext GetAPIContext()
        {
            APIContext apiContext = new APIContext(GetAccessToken());
            apiContext.Config = GetConfig();
            return apiContext;
        }
    }
}

   