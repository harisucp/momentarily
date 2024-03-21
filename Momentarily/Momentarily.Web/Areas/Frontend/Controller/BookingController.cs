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
using System.Security.Policy;
using Apeek.NH.Repository.Repositories;
using Momentarily.Entities.Entities;
using PusherServer;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Configuration;

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
        private readonly IUserNotificationService _userNotificationService;
        private readonly IUserDataService<MomentarilyItem> _userDataService;
        private readonly PinPaymentService _pinPaymentService;
        private readonly ILiveLocationService _liveLocationService;
        private string GoogleApiKey = ConfigurationManager.AppSettings["Google.ApiKey"];
        public BookingController(IMomentarilyGoodRequestService goodRequestService, IMomentarilyItemDataService goodItemService, IPaymentService paymentService, IMomentarilyUserMessageService userMessageService,
            ISendMessageService emailMessageService, IAccountDataService accountDataService, ITwilioNotificationService twilioNotificationService, IUserNotificationService userNotificationService, IUserDataService<MomentarilyItem> userDataService, ILiveLocationService liveLocationService)
        {
            _goodRequestService = goodRequestService;
            _goodItemService = goodItemService;
            _paymentService = paymentService;
            _userMessageService = userMessageService;
            _emailMessageService = emailMessageService;
            _accountDataService = accountDataService;
            _twilioNotificationService = twilioNotificationService;
            _userNotificationService = userNotificationService;
            _userDataService = userDataService;
            _pinPaymentService = new PinPaymentService();
            _liveLocationService = liveLocationService;
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

                if (user != null)
                {
                    var phoneNumber = _accountDataService.GetUserPhone(user.Id);
                    var countryCode = _accountDataService.GetCountryCodeByPhoneNumber(phoneNumber);

                    foreach (var booking in goodRequests)
                    {
                        var bookingEndTimeStr = booking.EndTime;
                        DateTime bookingEndTime;

                        if (DateTime.TryParse(bookingEndTimeStr, out bookingEndTime))
                        {
                            var text = $"Your rental for {booking.GoodName} is due on {bookingEndTime.ToString("yyyy-MM-dd")} at {bookingEndTime.ToString("HH:mm")}. Please prepare for return.";
                            var timeUntilEnd = bookingEndTime - DateTime.Now;
                            if (timeUntilEnd.TotalHours <= 24 && booking.IsViewed != true && booking.StatusName != "Pending")
                            {
                                _twilioNotificationService.RentalDueDate(phoneNumber, countryCode, user.Id, text);
                                var userNotificationCreateModel = new UserNotificationCreateModel
                                {
                                    UserId = user.Id,
                                    Text = text,
                                    Url = $"{HttpContext.Request.Url.GetLeftPart(UriPartial.Authority)}/Booking/Request/{booking.Id}"
                                };
                                _userNotificationService.AddNotification(userNotificationCreateModel);
                                _userDataService.UpdateIsViewedNotification(booking.GoodId);
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
                var liveLocation = _liveLocationService.GetByRequestId(id);
                if (liveLocation == null)
                {
                    getUserRequestResult.Obj.ReturnConfirm = false;
                    getUserRequestResult.Obj.ConfirmDelivery = false;

                }
                else
                {
                    getUserRequestResult.Obj.ReturnConfirm = liveLocation.ReturnConfirm;
                    getUserRequestResult.Obj.ConfirmDelivery = liveLocation.DeliveryConfirm;
                }

                if (getUserRequestResult.Obj.CouponCode != null && getUserRequestResult.Obj.CouponDiscount != 0)
                {
                    //result.Obj.CouponDiscount = requestModel.CouponDiscount;
                    getUserRequestResult.Obj.CustomerCost = getUserRequestResult.Obj.CustomerCost - getUserRequestResult.Obj.CouponDiscount;
                }

                if (getUserRequestResult.Obj.StatusId == (int)UserRequestStatus.Approved)
                    getUserRequestResult.Obj.PaymentUrl = QuickUrl.RequestPaymentAbsoluteUrl(id);
                //todo: for debug use result.Obj.EndDate.AddHours(24)
                getUserRequestResult.Obj.CanCancel = _goodRequestService.CanSeekerCancel(getUserRequestResult.Obj.StatusId);
                getUserRequestResult.Obj.CanStartDispute = _goodRequestService.CanSeekerStartDispute(getUserRequestResult.Obj.StatusId, getUserRequestResult.Obj.StartDate, getUserRequestResult.Obj.EndDate);

                getUserRequestResult.Obj.CanReview = _goodRequestService.CanSeekerReview(UserId.Value, getUserRequestResult.Obj.Id, getUserRequestResult.Obj.StatusId, getUserRequestResult.Obj.EndDate);
                getUserRequestResult.Obj.CanReceive = getUserRequestResult.Obj.StatusId == (int)UserRequestStatus.Released
                                                        && getUserRequestResult.Obj.StartDate.Date == DateTime.Now.Date
                                                        && getUserRequestResult.Obj.StartDate.Date <= getUserRequestResult.Obj.StartDate.AddDays(1).Date ? true : false;
                getUserRequestResult.Obj.CanReturn = getUserRequestResult.Obj.StatusId == (int)UserRequestStatus.Received
                                                     && getUserRequestResult.Obj.EndDate.Date <= DateTime.Now.Date ? true : false;
                if (getUserRequestResult.Obj.IsViewed != true)
                {
                    var userNotificationCreateModel = new UserNotificationCreateModel
                    {
                        UserId = getUserRequestResult.Obj.OwnerId,
                        Text = $"Your booking for {getUserRequestResult.Obj.GoodName} on {getUserRequestResult.Obj.StartDate} has been requested by {getUserRequestResult.Obj.UserName}.",
                        Url = $"{HttpContext.Request.Url.GetLeftPart(UriPartial.Authority)}/Listing/Booking/{id}"
                    };
                    _userNotificationService.AddNotification(userNotificationCreateModel);
                    _goodRequestService.UpdateIsViewedNotification(id);
                }
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

                        _goodRequestService.CancelUserRequest(UserId.Value, id);                        _goodRequestService.AddCancelledRequest(UserId.Value, id);                        var userNotificationCreateModel = new UserNotificationCreateModel
                        {
                            UserId = resultRequest.Obj.OwnerId,
                            Text = $"Your booking for {resultRequest.Obj.GoodName} on {resultRequest.Obj.StartDate} has been canceled. Check your email for more details.",
                            Url = $"{HttpContext.Request.Url.GetLeftPart(UriPartial.Authority)}/Booking/Request/{id}"
                        };                        _userNotificationService.AddNotification(userNotificationCreateModel);                    }                    else                    {
                        var _res = _goodRequestService.CancelUserRequestBeforePayment(UserId.Value, id);                        if (_res)
                        {
                            var countryCode = _accountDataService.GetCountryCodeByPhoneNumber(resultRequest.Obj.OwnerPhone);
                            _goodRequestService.AddCancelledRequest(UserId.Value, id);
                            var userNotificationCreateModel = new UserNotificationCreateModel
                            {
                                UserId = resultRequest.Obj.OwnerId,
                                Text = $"Your booking for {resultRequest.Obj.GoodName} on {resultRequest.Obj.StartDate} has been canceled. Check your email for more details.",
                                Url = $"{HttpContext.Request.Url.GetLeftPart(UriPartial.Authority)}/Booking"
                            };
                            _userNotificationService.AddNotification(userNotificationCreateModel);
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

        public bool RefundCapturedAmount(PaypalPayment payment, Result<GoodRequestViewModel> goodRequest, double diffrence)        {            try            {

                APIContext apiContext = PaypalConfiguration.GetAPIContext();
                Capture capture = Capture.Get(apiContext, payment.CaptureId);
                Amount amount = new Amount();
                amount.currency = "USD";
                amount.total = "0";
                if (diffrence > 48)
                {
                    amount.total = Convert.ToString(goodRequest.Obj.DaysCost + goodRequest.Obj.DiliveryCost + goodRequest.Obj.SecurityDeposit);
                }
                else if (diffrence >= 24 && diffrence <= 48)
                {
                    amount.total = Convert.ToString(goodRequest.Obj.DiliveryCost + (goodRequest.Obj.DaysCost / 2) + goodRequest.Obj.SecurityDeposit);

                }
                else if (diffrence < 24)
                {
                    amount.total = Convert.ToString(goodRequest.Obj.SecurityDeposit);

                }
                new Details { tax = "0", shipping = "0" };
                Refund refund = new Refund
                {
                    amount = amount
                };
                refund = capture.Refund(apiContext, refund);
                _emailMessageService.SendCancelEmailTemplateDetailForBorrower(payment, goodRequest, amount.total);
                //string state = refund.state;
                return true;

            }            catch (Exception ex)            {            }            return false;        }


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
                if (_goodRequestService.SeekerStartDispute(UserId.Value, requestChangeStatus.Id))
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
                                    requestChangeStatus.Message, goodBasedUser.Email);
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

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> Location(int id)
        {
            if (!UserId.HasValue || !UserAccess.HasAccess(Privileges.CanViewUsers, UserId) || id == 0)
                return RedirectToHome();
            var getUserRequestResult = _goodRequestService.GetUserRequest(UserId.Value, id);

            var momentarilyItem = _goodItemService.GetItem(getUserRequestResult.Obj.GoodId);

            if (DateTime.Now <= getUserRequestResult.Obj.EndDate && DateTime.Now >= getUserRequestResult.Obj.StartDate)
            {
                var Obj = new LiveLocation();

                var checkRequest = _liveLocationService.checkRequest(id);
                if (getUserRequestResult.Obj.ApplyForDelivery)
                {
                    Obj = _liveLocationService.GetByRequestId(id);
                }
                else
                {
                    if (checkRequest != true)
                    {
                        var userNotificationCreateModel = new UserNotificationCreateModel
                        {
                            UserId = getUserRequestResult.Obj.OwnerId,
                            Text = $"Borrower has started the ride.Click here to track location.",
                            Url = $"{HttpContext.Request.Url.GetLeftPart(UriPartial.Authority)}/Listing/Location/{id}"
                        };
                        _userNotificationService.AddNotification(userNotificationCreateModel);

                        var borrowerCoordinates = await FetchLiveLocation();

                        Obj = _liveLocationService.AddLocation(UserId.Value, id, getUserRequestResult.Obj.OwnerId, momentarilyItem.Obj.Latitude, momentarilyItem.Obj.Longitude, borrowerCoordinates.Item1, borrowerCoordinates.Item2, DeliverBy.BORROWER);
                    }
                    else
                    {
                        Obj = _liveLocationService.GetByRequestId(id);
                    }
                }
                LocationViewModel locationViewModel = new LocationViewModel();

                if (Obj != null)
                {
                    locationViewModel = new LocationViewModel()
                    {
                        SharerLatitude = Obj.SharerLatitude,
                        SharerLongitude = Obj.SharerLongitude,
                        BorrowerLatitude = Obj.BorrowerLatitude,
                        BorrowerLongitude = Obj.BorrowerLongitude,
                        LocationId = Obj.LocationId,
                        DeliverBy = Obj.DeliverBy.ToString(),
                        RideStarted = Obj.RideStarted,
                        RequestId = Obj.GoodRequestId,
                        ReturnConfirm = Obj.ReturnConfirm,
                        DeliveryConfirm = Obj.DeliveryConfirm
                    };
                }
                var shape = _shapeFactory.BuildShape(null, locationViewModel, PageName.UserRequest.ToString());
                return DisplayShape(shape);
            } else
            {
                if (DateTime.Now > getUserRequestResult.Obj.EndDate)
                {
                    var userNotificationCreateModel = new UserNotificationCreateModel
                    {
                        UserId = UserId.Value,
                        Text = $"Rental time has been completed.",
                        Url = $"{HttpContext.Request.Url.GetLeftPart(UriPartial.Authority)}/Listing/Booking/{id}"
                    };
                    _userNotificationService.AddNotification(userNotificationCreateModel);
                }
                else
                {
                    var userNotificationCreateModel = new UserNotificationCreateModel
                    {
                        UserId = UserId.Value,
                        Text = $"Delivery time has not started yet.",
                        Url = $"{HttpContext.Request.Url.GetLeftPart(UriPartial.Authority)}/Listing/Booking/{id}"
                    };
                    _userNotificationService.AddNotification(userNotificationCreateModel);
                }
                return RedirectToAction("Request", new { id = id });
            }

           
        }

        [HttpPost, ActionName("UpdateLocation")]
        public JsonResult UpdateLocation(string locationId, double lat, double lng)
        {
            var liveLocation = _liveLocationService.UpdateBorrowerLocation(locationId, UserId.Value, Convert.ToDouble(lat), Convert.ToDouble(lng));
            return Json(new { status = "success" });
        }

        [HttpPost, ActionName("Location")]
        public JsonResult PostLocation(string locationId)
        {
            var liveLocation = _liveLocationService.fetchLocation(locationId);
            return Json(new { lat = liveLocation.SharerLatitude, lng = liveLocation.SharerLongitude });
        }

        [HttpGet]
        public ActionResult ConfirmDelivery(int id)
        {
            var liveLocation = _liveLocationService.ConfirmDelivery(id);
            return RedirectToAction("Request", new { id = id });
        }

        [HttpGet]
        public ActionResult ReturnConfirm(int id)
        {
            var liveLocation = _liveLocationService.ReturnConfirm(id);
            return RedirectToAction("Request", new { id = id });
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> Return(int id)
        {
            if (!UserId.HasValue || !UserAccess.HasAccess(Privileges.CanViewUsers, UserId) || id == 0)
                return RedirectToHome();
            var getUserRequestResult = _goodRequestService.GetUserRequest(UserId.Value, id);
            
            var momentarilyItem = _goodItemService.GetItem(getUserRequestResult.Obj.GoodId);

            var liveLocation = _liveLocationService.GetByRequestId(id);

            var Obj = new LiveLocation();
            
            var yesterday = DateTime.Now.AddDays(-1);

            TimeSpan timeDifference = DateTime.Now - getUserRequestResult.Obj.EndDate;

            var checkRequest = _liveLocationService.checkRequest(id);

            if (liveLocation.ReturnConfirm == false && liveLocation.DeliveryConfirm == true)
            {
                if (DateTime.Now <= getUserRequestResult.Obj.EndDate && DateTime.Now >= getUserRequestResult.Obj.StartDate && timeDifference.TotalHours <= 24 && yesterday < DateTime.Now)
                {
                    if (getUserRequestResult.Obj.ApplyForDelivery == true)
                    {
                        Obj = _liveLocationService.GetByRequestId(id);
                    }
                    else
                    {
                        if (checkRequest != true)
                        {
                            var userNotificationCreateModel = new UserNotificationCreateModel
                            {
                                UserId = getUserRequestResult.Obj.OwnerId,
                                Text = $"Borrower has started the ride.Click here to track location.",
                                Url = $"{HttpContext.Request.Url.GetLeftPart(UriPartial.Authority)}/Listing/Return/{id}"
                            };
                            _userNotificationService.AddNotification(userNotificationCreateModel);

                            var borrowerCoordinates = await FetchLiveLocation();

                            Obj = _liveLocationService.AddLocation(UserId.Value, id, getUserRequestResult.Obj.OwnerId, momentarilyItem.Obj.Latitude, momentarilyItem.Obj.Longitude, borrowerCoordinates.Item1, borrowerCoordinates.Item2, DeliverBy.BORROWER);
                        }
                        else
                        {
                            var userNotificationCreateModel = new UserNotificationCreateModel
                            {
                                UserId = getUserRequestResult.Obj.OwnerId,
                                Text = $"Borrower has started the ride.Click here to track location.",
                                Url = $"{HttpContext.Request.Url.GetLeftPart(UriPartial.Authority)}/Listing/Return/{id}"
                            };
                            _userNotificationService.AddNotification(userNotificationCreateModel);

                            Obj = _liveLocationService.GetByRequestId(id);
                        }
                    }

                    LocationViewModel locationViewModel = new LocationViewModel();

                    if (Obj != null)
                    {
                        locationViewModel = new LocationViewModel
                        {
                            SharerLatitude = Obj.SharerLatitude,
                            SharerLongitude = Obj.SharerLongitude,
                            BorrowerLatitude = Obj.BorrowerLatitude,
                            BorrowerLongitude = Obj.BorrowerLongitude,
                            LocationId = Obj.LocationId,
                            DeliverBy = Obj.DeliverBy.ToString(),
                            RideStarted = Obj.RideStarted,
                            RequestId = Obj.GoodRequestId,
                            ReturnConfirm = Obj.ReturnConfirm,
                            DeliveryConfirm = Obj.DeliveryConfirm
                        };
                    }

                    var shape = _shapeFactory.BuildShape(null, locationViewModel, PageName.UserRequest.ToString());
                    return DisplayShape(shape);
                }
                else
                {
                    if (yesterday > getUserRequestResult.Obj.EndDate)
                    {
                        var userNotificationCreateModel = new UserNotificationCreateModel
                        {
                            UserId = UserId.Value,
                            Text = $"Rental time has been completed.",
                            Url = $"{HttpContext.Request.Url.GetLeftPart(UriPartial.Authority)}/Booking/Request/{id}"
                        };
                        _userNotificationService.AddNotification(userNotificationCreateModel);
                    }
                    else
                    {
                        var userNotificationCreateModel = new UserNotificationCreateModel
                        {
                            UserId = UserId.Value,
                            Text = $"Return time has not started yet.",
                            Url = $"{HttpContext.Request.Url.GetLeftPart(UriPartial.Authority)}/Booking/Request/{id}"
                        };
                        _userNotificationService.AddNotification(userNotificationCreateModel);
                    }
                    return RedirectToAction("Request", new { id = id });
                }
            }
            else if (liveLocation.ReturnConfirm == true && liveLocation.DeliveryConfirm == true)
            {
                var userNotificationCreateModel = new UserNotificationCreateModel
                {
                    UserId = UserId.Value,
                    Text = $"Rental has been successfully completed.",
                    Url = $"{HttpContext.Request.Url.GetLeftPart(UriPartial.Authority)}/Listing/Booking/{id}"
                };
                _userNotificationService.AddNotification(userNotificationCreateModel);
                return RedirectToAction("Request", new { id = id });
            }
            else
            {
                var userNotificationCreateModel = new UserNotificationCreateModel
                {
                    UserId = UserId.Value,
                    Text = $"You cannot start return. The Item is not delivered yet.",
                    Url = $"{HttpContext.Request.Url.GetLeftPart(UriPartial.Authority)}/Booking/Request/{id}"
                };
                _userNotificationService.AddNotification(userNotificationCreateModel);
                return RedirectToAction("Request", new { id = id });
            }
        }

        private async Task<(double, double)> FetchLiveLocation()
        {
            string apiUrl = $"https://www.googleapis.com/geolocation/v1/geolocate?key=AIzaSyB_ex7ilBX-t85Ytd3AG4hbeK6XlAjClmE";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.PostAsync(apiUrl, null);
                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        dynamic jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject(responseBody);
                        double latitude = jsonData.location.lat;
                        double longitude = jsonData.location.lng;
                        return (latitude, longitude);
                    }
                    else
                    {
                        Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                        return (0, 0);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching live location: " + ex.Message);
                return (0, 0);
            }
        }

        private async Task<(double, double)> GetCoordinatesAsync(string location)
        {
            var encodedLocation = Uri.EscapeDataString(location);
            var url = $"https://maps.googleapis.com/maps/api/geocode/json?address={encodedLocation}&key=AIzaSyB_ex7ilBX-t85Ytd3AG4hbeK6XlAjClmE";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                HttpResponseMessage response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Failed to retrieve geocoding data.");
                }

                var content = await response.Content.ReadAsStringAsync();
                var jsonObject = JObject.Parse(content);

                var results = jsonObject["results"];
                if (results.HasValues)
                {
                    var result = results[0];
                    var geometry = result["geometry"];
                    var locationObj = geometry["location"];
                    var lat = (double)locationObj["lat"];
                    var lng = (double)locationObj["lng"];
                    return (lat, lng);
                }
                else
                {
                    throw new Exception("No results found for the provided location.");
                }
            }
        }
    }
}