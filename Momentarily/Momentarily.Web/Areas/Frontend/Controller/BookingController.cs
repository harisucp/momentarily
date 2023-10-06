using System.Web.Mvc;
using Apeek.Common;
using Apeek.Common.Definitions;
using Apeek.Core.Services;
using Apeek.Core.Services.Impl;
using Apeek.ViewModels.Models;
using Apeek.Web.Framework;
using Apeek.Web.Framework.Controllers;
using Momentarily.UI.Service.Services;
using Momentarily.UI.Service.Services.Impl;
using System.Linq;
using PayPal.Api;
using System;
using Apeek.Common.Models;
using System.Globalization;
using System.Collections.Generic;
using Apeek.Entities.Entities;

namespace Momentarily.Web.Areas.Frontend.Controller
{
    [RedirectNotConfirmedFilter]
    public class BookingController : FrontendController
    {
        private readonly IMomentarilyGoodRequestService _goodRequestService;
        private readonly IMomentarilyItemDataService _goodItemService;
        private readonly IPaymentService _paymentService;
        private readonly IMomentarilyUserMessageService _userMessageService;
        private readonly ISendMessageService _emailMessageService;
        private readonly IAccountDataService _accountDataService;
        private readonly ITwilioNotificationService _twilioNotificationService;
        private readonly PinPaymentService _pinPaymentService;
        public BookingController(IMomentarilyGoodRequestService goodRequestService, IMomentarilyItemDataService goodItemService, IPaymentService paymentService, IMomentarilyUserMessageService userMessageService,
            ISendMessageService emailMessageService, IAccountDataService accountDataService, ITwilioNotificationService twilioNotificationService)
        {
            _goodRequestService = goodRequestService;
            _goodItemService = goodItemService;
            _paymentService = paymentService;
            _userMessageService = userMessageService;
            _emailMessageService = emailMessageService;
            _accountDataService = accountDataService;
            _twilioNotificationService = twilioNotificationService;
            _pinPaymentService = new PinPaymentService();
        }
        [Authorize]
        [HttpGet]
        public ActionResult Index()
        {
            if (!UserId.HasValue || !UserAccess.HasAccess(Privileges.CanViewUsers, UserId))
                return RedirectToHome();
            var result = _goodRequestService.GetUserRequests(UserId.Value);
            if (result.CreateResult == CreateResult.Success)
            {
                var goodRequests = result.Obj.OrderByDescending(x => x.CreateDate).AsEnumerable();
                var shape = _shapeFactory.BuildShape(null, goodRequests, PageName.UserRequests.ToString());

                var user = _accountDataService.GetUser(UserId.Value);
       
                //foreach (var booking in goodRequests)
                //{
                //    if (user != null)
                //    {
                //        var phoneNumber = _accountDataService.GetUserPhone(user.Id);
                //        var countryCode = _accountDataService.GetCountryCodeByPhoneNumber(phoneNumber);
                //        _twilioNotificationService.RentalDueDate(phoneNumber, countryCode, user.Id);

                //    }

                //}
                if (user != null)
                {
                    var phoneNumber = _accountDataService.GetUserPhone(user.Id);
                    var countryCode = _accountDataService.GetCountryCodeByPhoneNumber(phoneNumber);

                    foreach (var booking in goodRequests)
                    {
                        var bookingEndTimeStr = booking.EndTime; // Assuming booking.EndTime is a string
                        DateTime bookingEndTime;

                        if (DateTime.TryParse(bookingEndTimeStr, out bookingEndTime))
                        {
                            var timeUntilEnd = bookingEndTime - DateTime.Now;
                            if (timeUntilEnd.TotalHours <= 24)
                            {
                                // Send notification
                                var text = $"Your rental for {booking.GoodName} is due on {bookingEndTime.ToString("yyyy-MM-dd")} at {bookingEndTime.ToString("HH:mm")}. Please prepare for return.";
                                _twilioNotificationService.RentalDueDate(phoneNumber, countryCode, user.Id,text);
                            }
                        }
                    }
                }
                return DisplayShape(shape);
            }
            return RedirectToHome();
        }
        [Authorize]
        [HttpGet]
        public ActionResult Request(int id)
        {
            return GetRequest(id, "Request");
        }        

        private ActionResult GetRequest(int id, string viewName)
        {
            if (!UserId.HasValue || !UserAccess.HasAccess(Privileges.CanViewUsers, UserId) || id == 0)
                return RedirectToHome();
            var getUserRequestResult = _goodRequestService.GetUserRequest(UserId.Value, id);
            if (getUserRequestResult.CreateResult == CreateResult.Success)
            {
                var momentarilyItem = _goodItemService.GetItem(getUserRequestResult.Obj.GoodId);
                getUserRequestResult.Obj.SecurityDeposit = getUserRequestResult.Obj.SecurityDeposit;
                getUserRequestResult.Obj.PickUpLocation = momentarilyItem.Obj.PickUpLoaction;
                getUserRequestResult.Obj.Location = momentarilyItem.Obj.Location;
                getUserRequestResult.Obj.Latitude = momentarilyItem.Obj.Latitude;
                getUserRequestResult.Obj.Longitude = momentarilyItem.Obj.Longitude;

                if (getUserRequestResult.Obj.CouponCode != null && getUserRequestResult.Obj.CouponDiscount != 0)
                {
                    //result.Obj.CouponDiscount = requestModel.CouponDiscount;
                    getUserRequestResult.Obj.CustomerCost = getUserRequestResult.Obj.CustomerCost - getUserRequestResult.Obj.CouponDiscount;
                }

                if (getUserRequestResult.Obj.StatusId == (int)UserRequestStatus.Approved )
                    getUserRequestResult.Obj.PaymentUrl = QuickUrl.RequestPaymentAbsoluteUrl(id);
                //todo: for debug use result.Obj.EndDate.AddHours(24)
                getUserRequestResult.Obj.CanCancel = _goodRequestService.CanSeekerCancel(getUserRequestResult.Obj.StatusId);
                getUserRequestResult.Obj.CanStartDispute = _goodRequestService.CanSeekerStartDispute(getUserRequestResult.Obj.StatusId, getUserRequestResult.Obj.StartDate, getUserRequestResult.Obj.EndDate);
               
                getUserRequestResult.Obj.CanReview = _goodRequestService.CanSeekerReview(UserId.Value, getUserRequestResult.Obj.Id, getUserRequestResult.Obj.StatusId, getUserRequestResult.Obj.EndDate);
                getUserRequestResult.Obj.CanReceive = getUserRequestResult.Obj.StatusId== (int)UserRequestStatus.Released 
                                                        && getUserRequestResult.Obj.StartDate.Date == DateTime.Now.Date 
                                                        && getUserRequestResult.Obj.StartDate.Date <= getUserRequestResult.Obj.StartDate.AddDays(1).Date ? true : false;
                getUserRequestResult.Obj.CanReturn = getUserRequestResult.Obj.StatusId == (int)UserRequestStatus.Received
                                                     && getUserRequestResult.Obj.EndDate.Date <= DateTime.Now.Date ? true : false;

                var shape = _shapeFactory.BuildShape(null, getUserRequestResult.Obj, PageName.UserRequest.ToString());               
                return DisplayShape(shape);
            }
            return RedirectToAction("Index");
        }
        [Authorize]
        [HttpGet]
        public ActionResult CancelRequest(int id)        {            if (!UserId.HasValue || !UserAccess.HasAccess(Privileges.CanViewUsers, UserId) || id == 0)                return RedirectToHome();            var resultRequest = _goodRequestService.GetUserRequest(UserId.Value, id);            if (resultRequest.CreateResult == CreateResult.Success)            {                if (_goodRequestService.CanSeekerCancel(resultRequest.Obj.StatusId))                {                    if (resultRequest.Obj.StatusId == (int)UserRequestStatus.Paid)//if paid
                    {                        var StartDate = resultRequest.Obj.StartDate.ToString("MM/dd/yyyy");                        var StartTime = resultRequest.Obj.StartTime;                        DateTime startDateTime = DateTime.ParseExact(StartDate + " " + StartTime, "MM/dd/yyyy h:mm tt", CultureInfo.InvariantCulture);                        var Differences = (startDateTime - DateTime.Now).TotalHours;

                        var payment = _paymentService.GetPaypalPayment(resultRequest.Obj.Id);
                        bool refund = RefundCapturedAmount(payment, resultRequest, Differences);

                        _goodRequestService.CancelUserRequest(UserId.Value, id);                        _goodRequestService.AddCancelledRequest(UserId.Value, id);                    }                    else                    {
                        var _res = _goodRequestService.CancelUserRequestBeforePayment(UserId.Value, id);                        if (_res)
                        {
                            var countryCode = _accountDataService.GetCountryCodeByPhoneNumber(resultRequest.Obj.OwnerPhone);
                            _goodRequestService.AddCancelledRequest(UserId.Value, id);
                            _twilioNotificationService.CancellationAlert(resultRequest.Obj.OwnerPhone, countryCode, resultRequest.Obj.OwnerId, resultRequest.Obj.GoodName, resultRequest.Obj.StartDate);
                            _emailMessageService.SendEmailCancelBookingByBorrower(resultRequest.Obj.OwnerId, resultRequest.Obj.OwnerName, resultRequest.Obj.OwnerEmail);
                        }
                    }
                }            }            return RedirectToAction("Request", new { id = id });        }

        [Authorize]
        [HttpGet]
        public ActionResult ReceiveItem(int id)
        {
            if (!UserId.HasValue || !UserAccess.HasAccess(Privileges.CanViewUsers, UserId) || id == 0)
                return RedirectToHome();
            var resultRequest = _goodRequestService.ReceiveGoodRequest(UserId.Value, id);
      
            return RedirectToAction("Request", new { id = id });
        }

        [Authorize]
        [HttpGet]
        public ActionResult ReturnItem(int id)
        {
            if (!UserId.HasValue || !UserAccess.HasAccess(Privileges.CanViewUsers, UserId) || id == 0)
                return RedirectToHome();
            var resultRequest = _goodRequestService.ReturnGoodRequest(UserId.Value, id);

            return RedirectToAction("Request", new { id = id });
        }

        public bool RefundCapturedAmount(PaypalPayment payment, Result<GoodRequestViewModel> goodRequest, double diffrence)        {            try            {                                 APIContext apiContext = PaypalConfiguration.GetAPIContext();                    Capture capture = Capture.Get(apiContext, payment.CaptureId);                    Amount amount = new Amount();                    amount.currency = "USD";
                    amount.total = "0";                    if (diffrence > 48)                    {                        amount.total = Convert.ToString(goodRequest.Obj.DaysCost + goodRequest.Obj.DiliveryCost + goodRequest.Obj.SecurityDeposit);                    }                    else if (diffrence >= 24 && diffrence <= 48)                    {                        amount.total = Convert.ToString(goodRequest.Obj.DiliveryCost + (goodRequest.Obj.DaysCost / 2) + goodRequest.Obj.SecurityDeposit);                    }
                    else if (diffrence < 24)                    {                        amount.total = Convert.ToString(goodRequest.Obj.SecurityDeposit);                    }                    new Details { tax = "0", shipping = "0" };                    Refund refund = new Refund                    {                        amount = amount                    };                    refund = capture.Refund(apiContext, refund);
                    _emailMessageService.SendCancelEmailTemplateDetailForBorrower(payment, goodRequest, amount.total);
                    //string state = refund.state;
                    return true;                           }            catch (Exception ex)            {            }            return false;        }


        [Authorize]
        [HttpGet]
        public ActionResult BookingDispute(int id)
        {
            List<SelectListItem> reasons = new List<SelectListItem>();
            reasons.Add(new SelectListItem { Text = "Late", Value = "1" });
            reasons.Add(new SelectListItem { Text = "Not Received", Value = "2" });
            reasons.Add(new SelectListItem { Text = "Damaged", Value = "3" });
            reasons.Add(new SelectListItem { Text = "Late And Damaged", Value = "4" });
            reasons.Add(new SelectListItem { Text = "Stolen", Value = "5" });
            reasons.Add(new SelectListItem { Text = "Lost", Value = "6" });
            reasons.Add(new SelectListItem { Text = "Delivered Late", Value = "7" });
            ViewBag.Reasons = reasons;

            return GetRequest(id, "BookingDispute");
            
        }
        [Authorize]
        [HttpPost]
        public ActionResult BookingDispute(RequestChangeStatusViewModel requestChangeStatus)
        {
            if (!UserId.HasValue || !UserAccess.HasAccess(Privileges.CanViewUsers, UserId))
                return RedirectToHome();
            if (!string.IsNullOrWhiteSpace(requestChangeStatus.Message))
            {
                if (_goodRequestService.SeekerStartDispute(UserId.Value, requestChangeStatus.Id) )
                {
                    if (_accountDataService.SaveDisputeDetail(requestChangeStatus, UserId.Value))
                    {
                        var result = _goodRequestService.GetGoodRequest(requestChangeStatus.UserId, requestChangeStatus.Id);
                        if (result.CreateResult == CreateResult.Success)
                        {
                            var user = _accountDataService.GetUser(UserId.Value);
                            var goodBasedUser = _accountDataService.GetGoodBasedUser(result.Obj.GoodId);
                            if (user != null)
                            {
                                _emailMessageService.SendEmailSeekerStartDispute(result.Obj.GoodName, requestChangeStatus.Id, user.FirstName,
                                    requestChangeStatus.Message,goodBasedUser.Email);
                                _userMessageService.SendSeekerStartDisputeMessage(UserId.Value, requestChangeStatus.UserId,
                                    requestChangeStatus.Id, QuickUrl);
                            }
                        }
                    }
                }
            }
            return RedirectToAction("Request", new { id = requestChangeStatus.Id });
        }
        [Authorize]
        [HttpGet]
        public ActionResult BookingReview(int id)
        {
            if (!UserId.HasValue || !UserAccess.HasAccess(Privileges.CanViewUsers, UserId))
                return RedirectToHome();
            return GetRequest(id, "BookingReview");
        }
        [Authorize]
        [HttpPost]
        public ActionResult BookingReview(GoodRequestRankInsertModel model)
        {
            if (!UserId.HasValue || !UserAccess.HasAccess(Privileges.CanViewUsers, UserId))
                return RedirectToHome();
            if (ModelState.IsValid)
            {
                var result = _goodRequestService.SeekerLeavesReview(model, UserId.Value, QuickUrl);
                if (result.CreateResult == CreateResult.Success)
                {
                    return RedirectToAction("Request", new { id = model.GoodRequestId });
                }
            }
            return RedirectToAction("Request", new { id = model.GoodRequestId });
        }
    }
}