using System;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Apeek.Common;
using Apeek.Common.Definitions;
using Apeek.Core.Services;
using Apeek.Core.Services.Impl;
using Apeek.ViewModels.Models;
using Apeek.Web.Framework;
using Apeek.Web.Framework.Controllers;
using Momentarily.Entities.Entities;
using Momentarily.UI.Service.Services;
using Momentarily.ViewModels.Models;
using Momentarily.ViewModels.Mappers;
using Momentarily.Core.Services.Impl;
using Apeek.Common.Controllers;
using Apeek.Common.Models;
using System.Linq;
using Apeek.Entities.Entities;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using PayPal.Api;

using Apeek.Web.Framework.ControllerHelpers;
using System.Globalization;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Net.Http;

namespace Momentarily.Web.Areas.Frontend.Controller
{
    [RedirectNotConfirmedFilter]
    public class ListingController : FrontendController
    {
        private readonly IMomentarilyItemDataService _itemDataService;
        private readonly IMomentarilyItemTypeService _typeService;
        private readonly ICategoryService _categoryService;
        private readonly IMomentarilyGoodRequestService _goodRequestService;
        private readonly IMomentarilyUserMessageService _userMessageService;
        private readonly ISendMessageService _emailMessageService;
        private readonly IAccountDataService _accountDataService;
        private readonly IMomentarilyItemDataService _goodItemService;
        private readonly BraintreePaymentsService _braintreePaymentsService;
        private readonly IPaymentService _paymentService;
        private readonly AccountControllerHelper<MomentarilyRegisterModel> _helper;
        private readonly ISendMessageService _sendMessageService;
        private readonly ITwilioNotificationService _twilioNotificationService;
        private readonly IUserNotificationService _userNotificationService;
        private readonly ILiveLocationService _liveLocationService;
        public ListingController(IMomentarilyItemDataService itemDataService,
            IMomentarilyItemTypeService typeService, ICategoryService categoryService,
            IMomentarilyGoodRequestService goodRequestService, IMomentarilyUserMessageService userMessageService,
            ISendMessageService emailMessageService, IAccountDataService accountDataService, IPaymentService paymentService,
            IMomentarilyItemDataService goodItemService, ISendMessageService sendMessageService, ITwilioNotificationService twilioNotificationService, IUserNotificationService userNotificationService, ILiveLocationService liveLocationService)
        {
            _itemDataService = itemDataService;
            _typeService = typeService;
            _categoryService = categoryService;
            _goodRequestService = goodRequestService;
            _userMessageService = userMessageService;
            _emailMessageService = emailMessageService;
            _accountDataService = accountDataService;
            _goodItemService = goodItemService;
            _paymentService = paymentService;
            _braintreePaymentsService = new BraintreePaymentsService();
            _sendMessageService = sendMessageService;
            _twilioNotificationService = twilioNotificationService;
            _userNotificationService = userNotificationService;
            _liveLocationService = liveLocationService;
            _helper = new AccountControllerHelper<MomentarilyRegisterModel>();
        }
        [Authorize]
        [HttpGet]
        public ActionResult Index()        {            if (!UserId.HasValue || !UserAccess.HasAccess(Privileges.CanViewUsers, UserId))                return RedirectToHome();            var usersItems = _itemDataService.GetUsersItems(UserId.Value).OrderByDescending(x => x.CreateDate).ToList();            var shape = _shapeFactory.BuildShape(null, usersItems, PageName.Listing.ToString());            bool getPaypalInfoUser = _accountDataService.getExsistPaypalInfoOrNotInDb(UserId.Value);            TempData["PaypalUserInfo"] = getPaypalInfoUser;            return DisplayShape(shape);        }

        [Authorize]
        [RedirectUserBlockedFilterAttribute]
        [HttpGet]
        public ActionResult Create()
        {
            bool getPaypalInfoUser = _accountDataService.getExsistPaypalInfoOrNotInDb(UserId.Value);            if (getPaypalInfoUser != true)            {                return RedirectToAction("Index");            }
            ViewBag.Title = "List Item | momentarily";
            ViewBag.Description = "Make money off items you own. Rent out to thousands of potential users waiting to borrow.";
            if (!UserId.HasValue || !UserAccess.HasAccess(Privileges.CanViewUsers, UserId))
                return RedirectToHome();
            if (UserId != null && UserId.HasValue)
            {
                bool isblocked = _accountDataService.UserBlocked(UserId.Value);
                if (isblocked)
                    return RedirectToAction("BlockedUser", "User");
            }

            var newModel = new CreateMomentarilyItemViewModel
            {
                Categories = _categoryService.GetRootChilds(),
                Types = _typeService.GetAllTypes(),
                Item = MomentarilyItem.CreateEmpty()
            };
            newModel.Item.StartTime = "09:00 AM";            newModel.Item.EndTime = "06:00 AM";

            // ViewBag.Categories =new MultiSelectList(newModel.Categories,"Key","Value") ;
            return View("Create", newModel);
        }
        [Authorize]
        [RedirectUserBlockedFilterAttribute]
        [HttpGet]
        public ActionResult CreateTwo(MomentarilyItem model)
        {
            if (!UserId.HasValue || !UserAccess.HasAccess(Privileges.CanViewUsers, UserId))
                return RedirectToHome();
            if (UserId != null && UserId.HasValue)
            {
                bool isblocked = _accountDataService.UserBlocked(UserId.Value);
                if (isblocked)
                    return RedirectToAction("BlockedUser", "User");
            }
            var newModel = new CreateMomentarilyItemViewModel
            {
                Categories = _categoryService.GetRootChilds(),
                Types = _typeService.GetAllTypes(),
                Item = model
            };
            return View("Edit", newModel);
        }
        [Authorize]
        [RedirectUserBlockedFilterAttribute]
        [HttpPost]
        public ActionResult Create(MomentarilyItem model)
        {
            if (UserId != null && UserId.HasValue)
            {
                bool isblocked = _accountDataService.UserBlocked(UserId.Value);
                if (isblocked)
                    return RedirectToAction("BlockedUser", "User");
            }
            if (model.CategorList != null)
            {
                string _catData = model.CategorList.Replace("\"", "").Replace("[", "").Replace("]", "");
                model.CategorList = _catData;
            }
            return SaveItem(model, "create");
        }
        [Authorize]
        [RedirectUserBlockedFilterAttribute]
        [HttpGet]
        public ActionResult Edit(int itemId)
        {
            if (!UserId.HasValue || !UserAccess.HasAccess(Privileges.CanViewUsers, UserId))
                return RedirectToHome();
            if (UserId != null && UserId.HasValue)
            {
                bool isblocked = _accountDataService.UserBlocked(UserId.Value);
                if (isblocked)
                    return RedirectToAction("BlockedUser", "User");
            }
            var result = _itemDataService.GetMyItem(UserId.Value, itemId);
            //TempData["categoryTags"] = result.Obj.CategorList;
            if (result.CreateResult == CreateResult.Success)
            {
                result.Obj.Description = result.Obj.Description.Replace("<br/>", Environment.NewLine);
                var newModel = new CreateMomentarilyItemViewModel
                {
                    Categories = _categoryService.GetRootChilds(),
                    Types = _typeService.GetAllTypes(),
                    Item = result.Obj
                };
                return View("Edit", newModel);
            }
            return RedirectToAction("Index");
        }
        [Authorize]
        [RedirectUserBlockedFilterAttribute]
        [HttpPost]
        public ActionResult Edit(MomentarilyItem model)
        {
            if (UserId != null && UserId.HasValue)
            {
                bool isblocked = _accountDataService.UserBlocked(UserId.Value);
                if (isblocked)
                    return RedirectToAction("BlockedUser", "User");
            }
            if (model.CategorList != null)
            {
                string _catData = model.CategorList.Replace("\"", "").Replace("[", "").Replace("]", "");
                model.CategorList = _catData;
            }

            return SaveItem(model, "edit");
        }
        //public class test
        //{
        //    public string tests { get; set; }
        //}
        [Authorize]
        [HttpGet]
        public ActionResult BookingList(int id)
        {
            if (!UserId.HasValue || !UserAccess.HasAccess(Privileges.CanViewUsers, UserId) || id == 0)
                return RedirectToHome();
            var result = _goodRequestService.GetGoodRequests(UserId.Value, id);
            if (result.CreateResult == CreateResult.Success)
            {
                result.Obj.GoodRequests = result.Obj.GoodRequests.OrderByDescending(x => x.CreateDate).ToList();
                var shape = _shapeFactory.BuildShape(null, result.Obj, PageName.GoodRequests.ToString());
                return DisplayShape(shape);
            }
            return RedirectToAction("Index");
        }
        [Authorize]
        [HttpGet]
        public ActionResult Booking(int id)
        {
            return GetBooking(id, "Booking");
        }
        [Authorize]
        [HttpPost]
        public ActionResult ApproveBooking(RequestChangeStatusViewModel requestChangeStatus)
        {
            if (!UserId.HasValue || !UserAccess.HasAccess(Privileges.CanViewUsers, UserId))
                return RedirectToHome();
            if (_goodRequestService.ApproveGoodRequest(UserId.Value, requestChangeStatus.Id))
            {
                var result = _goodRequestService.GetGoodRequest(UserId.Value, requestChangeStatus.Id);
                var user = _accountDataService.GetUser(requestChangeStatus.UserId);
                if (user != null)
                {
                    _userMessageService.SendApproveGoodRequestMessage(UserId.Value, requestChangeStatus.UserId,
                        requestChangeStatus.Id, QuickUrl);
                    var phoneNumber = _accountDataService.GetUserPhone(user.Id);
                    var countryCode = _accountDataService.GetCountryCodeByPhoneNumber(phoneNumber);
                    var userNotificationCreateModel = new UserNotificationCreateModel
                    {
                        UserId = user.Id,
                        Text = $"Your booking for {result.Obj.GoodName} is confirmed for {result.Obj.StartDate.ToString("dd/MM/yyyy")} at {result.Obj.StartDate.ToString("hh:mm:ss")}.",
                        Url = $"{HttpContext.Request.Url.GetLeftPart(UriPartial.Authority)}/Booking"
                    };
                    _userNotificationService.AddNotification(userNotificationCreateModel);
                    _twilioNotificationService.BookingConfirmation(phoneNumber, countryCode, user.Id, result.Obj.GoodName, result.Obj.StartDate);
                    _emailMessageService.SendEmailApproveBookingMessage(user.Email, result.Obj.GoodName,
                        QuickUrl.UserRequestAbsoluteUrl(requestChangeStatus.Id),
                        QuickUrl.RequestPaymentAbsoluteUrl(requestChangeStatus.Id), user.FullName);
                }
            }
            return RedirectToAction("Booking", new { id = requestChangeStatus.Id });
        }
        [Authorize]
        [HttpGet]
        public ActionResult CancelBooking(int id)        {            if (!UserId.HasValue || !UserAccess.HasAccess(Privileges.CanViewUsers, UserId))                return RedirectToHome();            var resultRequest = _goodRequestService.GetUserRequest(UserId.Value, id);            if (resultRequest != null && resultRequest.Obj != null && resultRequest.CreateResult == CreateResult.Success)            {                if (resultRequest.Obj.StatusId == 5)//if paid
                {                    var payment = _paymentService.GetPaypalPayment(resultRequest.Obj.Id);                    bool refund = RefundCapturedAmount(payment.CaptureId, resultRequest);                    _goodRequestService.CancelGoodRequest(UserId.Value, id);                    _goodRequestService.AddCancelledRequest(UserId.Value, id);                }                else                {                     var _res = _goodRequestService.CancelGoodRequestBeforePayment(UserId.Value, id);                    if(_res)
                    {
                        _goodRequestService.AddCancelledRequest(UserId.Value, id);
                        var cuoponDetail = _accountDataService.GetDetailThankYouCoupon();
                        string couponCode = string.Empty;
                        double discountAmount = 0;
                        string discountType = string.Empty;
                        if (cuoponDetail != null)
                        {
                            couponCode = cuoponDetail.CouponCode;
                            discountAmount = cuoponDetail.CouponDiscount;
                            discountType = cuoponDetail.CouponDiscountType == 1 ? "%" : "$";
                        }
                        else
                        {
                            couponCode = "XXXXXXX";
                            discountAmount = 0;
                            discountType = "";
                        }
                        var phoneNumber = _accountDataService.GetUserPhone(resultRequest.Obj.UserId);
                        var countryCode = _accountDataService.GetCountryCodeByPhoneNumber(phoneNumber);
                        var userNotificationCreateModel = new UserNotificationCreateModel
                        {
                            UserId = resultRequest.Obj.OwnerId,
                            Text = $"Your booking for {resultRequest.Obj.GoodName} on {resultRequest.Obj.StartDate} has been canceled. Check your email for more details.",
                            Url = $"{HttpContext.Request.Url.GetLeftPart(UriPartial.Authority)}/Booking"
                        };
                        _userNotificationService.AddNotification(userNotificationCreateModel);
                        _twilioNotificationService.CancellationAlert(phoneNumber, countryCode, resultRequest.Obj.UserId, resultRequest.Obj.GoodName, resultRequest.Obj.StartDate);
                        _emailMessageService.SendEmailCancelBookingBySharer(resultRequest.Obj.UserId, resultRequest.Obj.UserName, resultRequest.Obj.UserEmail, couponCode);
                    }                }            }

            return RedirectToAction("Booking", new { id = id });        }

        public ActionResult ReleaseItem(int id)
        {
            if (!UserId.HasValue || !UserAccess.HasAccess(Privileges.CanViewUsers, UserId))
                return RedirectToHome();
            var data = _goodRequestService.ReleaseGoodRequest(UserId.Value, id);
            return RedirectToAction("Booking", new { id = id });
        }
        [Authorize]
        [HttpGet]
        public ActionResult ReturnedItemConfirm(int id)
        {
            if (!UserId.HasValue || !UserAccess.HasAccess(Privileges.CanViewUsers, UserId))
                return RedirectToHome();
            return View();
        }

        //[Authorize]
        //[HttpPost]
        //public ActionResult ReturnedItemConfirm()
        //{
        //    var id = 0;
        //    if (!UserId.HasValue || !UserAccess.HasAccess(Privileges.CanViewUsers, UserId))
        //        return RedirectToHome();
        //    //  var data = _goodRequestService.ReturnConfirmGoodRequest(UserId.Value, id);
        //    return RedirectToAction("Booking", new { id = id });
        //}
        public bool RefundCapturedAmount(string captureId, Result<GoodRequestViewModel> goodRequest)
        {
            try
            {

                APIContext apiContext = PaypalConfiguration.GetAPIContext();
                PayPal.Api.Capture capture = PayPal.Api.Capture.Get(apiContext, captureId);
                Amount amount = new Amount();

                amount.currency = "USD";
                amount.total = Convert.ToString(goodRequest.Obj.DaysCost + goodRequest.Obj.DiliveryCost + goodRequest.Obj.SecurityDeposit);
                new Details { tax = "0", shipping = "0" };
                Refund refund = new Refund
                {
                    amount = amount
                };
                var refunded = capture.Refund(apiContext, refund);
                string state = refunded.state;
                return true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        [Authorize]
        [HttpGet]
        public ActionResult BookingDecline(int id)
        {
            return GetBooking(id, "BookingDecline");
        }
        [Authorize]
        [HttpPost]
        public ActionResult BookingDecline(RequestChangeStatusViewModel requestChangeStatus)
        {
            if (!UserId.HasValue || !UserAccess.HasAccess(Privileges.CanViewUsers, UserId))
                return RedirectToHome();
            if (!string.IsNullOrWhiteSpace(requestChangeStatus.Message))
            {
                if (_goodRequestService.DeclineGoodRequest(UserId.Value, requestChangeStatus.Id))
                {
                    var result = _goodRequestService.GetGoodRequest(UserId.Value, requestChangeStatus.Id);
                    if (result.CreateResult == CreateResult.Success)
                    {
                        var user = _accountDataService.GetUser(requestChangeStatus.UserId);
                        if (user != null)
                        {
                            _userMessageService.SendDeclineGoodRequestMessage(UserId.Value, requestChangeStatus.UserId,
                                requestChangeStatus.Id, requestChangeStatus.Message, QuickUrl);
                            //_emailMessageService.SendEmailDeclineBookingMessage(user.Email, result.Obj.GoodName,
                            //        QuickUrl.UserRequestAbsoluteUrl(result.Obj.Id), user.FullName);
                            string coupon = getThankyouCoupon();
                            _emailMessageService.SendEmailBookingDeniedForBorrower(user.Id, user.FullName, user.Email, coupon, result.Obj.GoodName, requestChangeStatus.Message);
                            
                        }
                    }
                }
            }
            return GetBooking(requestChangeStatus.Id, "Booking");
        }
        [Authorize]
        [HttpPost]
        public ActionResult BookingClose(RequestChangeStatusViewModel requestChangeStatus)
        {
            if (!UserId.HasValue || !UserAccess.HasAccess(Privileges.CanViewUsers, UserId))
                return RedirectToHome();
            var result = _goodRequestService.GetGoodRequest(UserId.Value, requestChangeStatus.Id);
            if (result.CreateResult == CreateResult.Success && result.Obj.EndDate <= DateTime.Now
                && result.Obj.StatusId == (int)UserRequestStatus.Paid)
                return RedirectToAction("GetAdaptivePayment", "Payment", new { goodRequestId = requestChangeStatus.Id });
            return GetBooking(requestChangeStatus.Id, "Booking");
        }

        private ActionResult GetBooking(int id, string viewName)
        {
            if (!(UserId.HasValue && id > 0 && (UserAccess.HasAccess(Privileges.CanViewUsers, UserId) || this.User.IsInRole(Privileges.Admin))))
            {
                return RedirectToHome();
            }
            Result<GoodRequestViewModel> result = null;
            if (this.User.IsInRole(Privileges.Admin))
            {
                result = _goodRequestService.GetGoodRequest(id);
            }
            else
            {

                result = _goodRequestService.GetGoodRequest(UserId.Value, id);
            }
            if (result.CreateResult == CreateResult.Success)
            {
                var momentarilyItem = _goodItemService.GetItem(result.Obj.GoodId);
                result.Obj.Location = momentarilyItem.Obj.Location;
                result.Obj.Latitude = momentarilyItem.Obj.Latitude;
                result.Obj.Longitude = momentarilyItem.Obj.Longitude;
                result.Obj.GoodImageUrl = momentarilyItem.Obj.GoodImageUrl;
                if (_goodRequestService.CanSharerCancel(result.Obj.StatusId, result.Obj.CreateDate) && result.Obj.StatusId == (int)UserRequestStatus.Approved)
                    result.Obj.CanCancel = true;
                if (_goodRequestService.CanSharerStartDispute(result.Obj.StatusId, result.Obj.EndDate))
                {
                    result.Obj.CanStartDispute = true;
                }
                var liveLocation = _liveLocationService.GetByRequestId(id);
                if(liveLocation == null)
                {
                    result.Obj.ReturnConfirm = false;
                    result.Obj.ConfirmDelivery = false;
                } else
                {
                    result.Obj.ConfirmDelivery = liveLocation.DeliveryConfirm;
                    result.Obj.ReturnConfirm = liveLocation.ReturnConfirm;
                }

                //if (result.Obj.CouponCode != null && result.Obj.CouponDiscount != 0)
                //{
                //    //result.Obj.CouponDiscount = requestModel.CouponDiscount;
                //    result.Obj.CustomerCost = result.Obj.CustomerCost - result.Obj.CouponDiscount;
                //}
                result.Obj.CustomerServiceFee = Ioc.Get<ISettingsDataService>().GetSharerPaymentTransactionCommision();
                result.Obj.CustomerCharity = Ioc.Get<ISettingsDataService>().GetCharity();
                result.Obj.CanReview = _goodRequestService.CanSharerReview(UserId.Value, result.Obj.Id, result.Obj.StatusId, result.Obj.EndDate);
                result.Obj.CanRelease = result.Obj.StatusId == (int)UserRequestStatus.Paid && result.Obj.StartDate.Date == DateTime.Now.Date && result.Obj.StartDate.Date <= result.Obj.StartDate.AddDays(1).Date ? true : false;
                result.Obj.CanConfirmReturn = result.Obj.StatusId == (int)UserRequestStatus.Returned
                    && result.Obj.EndDate.Date <= DateTime.Now.Date ? true : false;

                FinalFeedbackVM finalFeedbackVM = new FinalFeedbackVM();                finalFeedbackVM.RequestId = result.Obj.Id;                finalFeedbackVM.NoIssue = true;                finalFeedbackVM.Late = false;                finalFeedbackVM.Damaged = false;                finalFeedbackVM.ReturnDate = DateTime.Now;                finalFeedbackVM.ReturnTime = "09:00 AM";                finalFeedbackVM.Claim = 0.00;
                result.Obj.finalFeedbackVM = finalFeedbackVM;


                result.Obj.CanRefund = result.Obj.StatusId != (int)UserRequestStatus.Refunded && this.User.IsInRole(Privileges.Admin);
                var shape = _shapeFactory.BuildShape(viewName, result.Obj, PageName.GoodRequest.ToString());
                return DisplayShape(shape);
            }
            return RedirectToAction("Index");
        }

        private ActionResult SaveItem(MomentarilyItem model, string actiontype)
        {
            if (!UserId.HasValue || !UserAccess.HasAccess(Privileges.CanViewUsers, UserId))
                return RedirectToHome();

            if (model.Images != null)
            {
                model.Images = model.Images.Where(x => x.FileName != null).ToList();
                ModelState.Clear();
                foreach (var dd in model.Images)
                {
                    dd.Url = "/Content/Images/Good/" + dd.FileName;
                }
            }
            else            {
                ModelState.AddModelError("images", "Please select atleast one image.");                ViewBag.Requires = "Please select atleast one image.";            }
            if (model.RentPeriodDay == false)
            {
                model.Price = 0;
            }
            if (model.RentPeriodWeek == false)
            {
                model.PricePerWeek = 0;
            }
            if (model.CategorList == "" || model.CategorList == null)
            {
                ModelState.AddModelError("Category", "Category Required");
                ViewBag.Requires = "Category Required";
            }
            else if (model.Name == null || model.Name == "")
            {
                ModelState.AddModelError("Name", "Name Required");
                ViewBag.Requires = "Name Required";
            }
            else if (model.Description == null || model.Description == "")
            {
                ModelState.AddModelError("Description", "Description Required");
                ViewBag.Requires = "Description Required";
            }
            else if (model.Price <= 0 && model.PricePerWeek <= 0)
            {
                ModelState.AddModelError("Price", "Price Required");
                ViewBag.Requires = "Price Required";
            }
            else if (model.Deposit <= 0)
            {
                ModelState.AddModelError("Deposit", "Security Deposit Required");
                ViewBag.Requires = "Security Deposit Required";
            }

            else if (model.Location == null || model.Location == "")
            {
                ModelState.AddModelError("Location", "Location Required");
                ViewBag.Requires = "Location Required";
            }

            if (model.StartTime == null)            {                ModelState.AddModelError("StartTime", "StartTime Required");                ViewBag.Requires = "StartTime Required";            }            else            {                TimeSpan Start = DateTime.ParseExact(model.StartTime, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay;                DateTime NewTime = DateTime.Today.Add(Start);                model.EndTime = NewTime.AddHours(21).ToString(@"hh:mm tt", CultureInfo.InvariantCulture);            }
            if (model.MinimumRentPeriod == 0 || model.MinimumRentPeriod == null)            {                ModelState.AddModelError("MinimumRentPeriod", "Minimum Rent Period Required");                ViewBag.Requires = "Minimum Rent Period Required";            }
            else if ((model.RentPeriodDay || model.RentPeriodWeek) && model.MinimumRentPeriod > 21)
            {
                ModelState.AddModelError("MinimumRentPeriod", "Maximum 21 days allowed for Minimum Rent Period");                ViewBag.Requires = "Maximum 21 days allowed";
            }
            else if (!model.RentPeriodDay && model.RentPeriodWeek && model.MinimumRentPeriod < 7)
            {
                ModelState.AddModelError("MinimumRentPeriod", "Minimum 7 days allowed for Minimum Rent Period");                ViewBag.Requires = "Minimum 7 days allowed for Minimum Rent Period";

            }
            if (ModelState.IsValid)
            {
                if (model.Images != null)
                {
                    model.Images = model.Images.Where(x => x.FileName != null).ToList();
                }
                //if (!string.IsNullOrWhiteSpace(model.Description))
                //    model.Description = Regex.Replace(model.Description, @"\r\n?|\n", "<br/>");
                model.IsViewed = false;
                var result = _itemDataService.SaveUserItem(model, UserId.Value);


                if (result.CreateResult == CreateResult.Success)
                {
                    string userEmail = _itemDataService.getUserEmail(UserId.Value);
                    bool checkExsistSubscriber = _helper.ExsistSubscriber(userEmail);
                    if (checkExsistSubscriber == false)
                    {
                        string subscribertURL = "/Account/NewsLetterSubscribe?Email=" + userEmail;
                        //string subscribertURL = "/Home/Index";
                        if (actiontype != "edit")
                        {
                            //  var sendMsgSubscriber = _sendMessageService.SendEmailNewsLetterTemplate(userEmail, subscribertURL);
                        }
                    }
                    return RedirectToAction("Index");
                }

                // return RedirectToAction("Index");
            }
            var newModel = new CreateMomentarilyItemViewModel
            {
                Categories = _categoryService.GetRootChilds(),
                Types = _typeService.GetAllTypes(),
                Item = model
            };
            if (actiontype == "create")
                return View("Create", newModel);
            return View("Edit", newModel);
        }
        [Authorize]
        [RedirectUserBlockedFilterAttribute]
        [HttpGet]
        public ActionResult BookingRequest(RequestModel requestModel)
        {
            double difference = requestModel.EndDate.Subtract(requestModel.StartDate).TotalDays;
            TempData["differ"] = "";
            if (difference > 20)
            {
                TempData["differ"] = difference;
                return RedirectToAction("item", "search", new { id = requestModel.GoodId });
            }
            if (!UserId.HasValue || !UserAccess.HasAccess(Privileges.CanViewUsers, UserId) ||
                requestModel == null || requestModel.GoodId == 0)
                return RedirectToHome();
            if (UserId != null && UserId.HasValue)
            {
                bool isblocked = _accountDataService.UserBlocked(UserId.Value);
                if (isblocked)
                    return RedirectToAction("BlockedUser", "User");
            }
            if (ModelState.IsValid)
            {
                if ((requestModel.EndDate - requestModel.StartDate).Days >= 0)
                {
                    var result = _goodRequestService.GetGoodRequest(UserId.Value, requestModel);
                    if (result.CreateResult == CreateResult.Success)
                    {
                        var item = _goodItemService.GetItem(result.Obj.GoodId);
                        result.Obj.CurrentUserIsOwner = UserId != null && UserId.Value == item.Obj.User.Id;
                        var shape = _shapeFactory.BuildShape(null, result.Obj, PageName.GoodRequest.ToString());
                        return DisplayShape(shape);
                    }
                }
            }
            return RedirectToAction("Index");
        }
        [Authorize]
        [RedirectUserBlockedFilterAttribute]
        [HttpPost]
        public ActionResult BookingRequest(RequestViewModel requestModel)        {            if (!UserId.HasValue || !UserAccess.HasAccess(Privileges.CanViewUsers, UserId))                return RedirectToHome();            if (UserId != null && UserId.HasValue)            {                bool isblocked = _accountDataService.UserBlocked(UserId.Value);                if (isblocked)                    return RedirectToAction("BlockedUser", "User");            }            if (ModelState.IsValid)            {

                bool result = _accountDataService.getExsistPaypalInfoOrNotInDb(UserId.Value);                var getItemResult = _goodItemService.GetItem(requestModel.GoodId);                if (getItemResult.CreateResult == CreateResult.Success)                {                    if (!result)                    {                        TempData["paymentDetailNotFound"] = "False";                        return RedirectToAction("BookingRequest", requestModel);                    }                    requestModel.Deposit = getItemResult.Obj.Deposit;                    var saveGoodRequestResult = _goodRequestService.SaveGoodRequest(UserId.Value, requestModel);                    if (saveGoodRequestResult.CreateResult == CreateResult.Success)                    {                        var goodRequest = saveGoodRequestResult.Obj;                        if (getItemResult.Obj.AgreeToShareImmediately)                        {                            var user = _accountDataService.GetUser(UserId.Value);                            if (user != null)                            {                                _userMessageService.SendBookingGoodRequestMessage(UserId.Value,                                goodRequest.Good.UserGood.User.Id,                                goodRequest.Id, QuickUrl);                            }                            if (_goodRequestService.ApproveGoodRequest(getItemResult.Obj.User.Id, goodRequest.Id))                            {                                return RedirectToAction("Pay", "Payment", new { goodRequestId = goodRequest.Id });                            }                        }                        else                        {                            var user = _accountDataService.GetUser(UserId.Value);
                            //string userEmail = _itemDataService.getUserEmail(UserId.Value);
                            //bool checkExsistSubscriber = _helper.ExsistSubscriber(userEmail);
                            //if (checkExsistSubscriber == false)
                            //{
                            //    string subscribertURL = "/Account/NewsLetterSubscribe?Email=" + userEmail;
                            //    //string subscribertURL = "/Home/Index";
                            //    var sendMsgSubscriber = _sendMessageService.SendEmailNewsLetterTemplate(userEmail, subscribertURL);
                            //}
                            if (user != null)                            {                                _userMessageService.SendBookingGoodRequestMessage(UserId.Value,                                goodRequest.Good.UserGood.User.Id,                                goodRequest.Id, QuickUrl);                                var sendOfferReceived = _emailMessageService.SendPaymentEmailTemplateOfferReceivedForSharer(user, goodRequest, QuickUrl.GoodRequestAbsoluteUrl(goodRequest.Id));                                return RedirectToAction("Request", "Booking", new { id = goodRequest.Id });                            }                        }                    }                }            }            return RedirectToAction("Index", "Listing");        }
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
            return GetBooking(id, "BookingDispute");
        }
        [Authorize]
        [HttpPost]
        public ActionResult BookingDispute(RequestChangeStatusViewModel requestChangeStatus)
        {
            if (!UserId.HasValue || !UserAccess.HasAccess(Privileges.CanViewUsers, UserId))
                return RedirectToHome();
            if (!string.IsNullOrWhiteSpace(requestChangeStatus.Message))
            {
                if (_goodRequestService.SharerStartDispute(requestChangeStatus.UserId, requestChangeStatus.Id))
                {
                    if (_goodRequestService.SaveDisputeDetail(requestChangeStatus, UserId.Value))
                    {
                        var result = _goodRequestService.GetGoodRequest(UserId.Value, requestChangeStatus.Id);
                        if (result.CreateResult == CreateResult.Success)
                        {
                            var user = _accountDataService.GetUser(UserId.Value);
                            if (user != null)
                            {
                                _emailMessageService.SendEmailSharerStartDispute(result.Obj.GoodName, user.FirstName,
                                    requestChangeStatus.Message, result.Obj.Id,result.Obj.UserEmail);
                                _userMessageService.SendSahrerStartDisputeMessage(UserId.Value, requestChangeStatus.UserId,
                                    requestChangeStatus.Id, QuickUrl);
                            }
                        }
                    }
                }
            }
            return RedirectToAction("Booking", new { id = requestChangeStatus.Id });
        }
        [Authorize]
        [HttpGet]
        public ActionResult RefundSecurityDeposit(int id)
        {
            if (!(UserId.HasValue && id > 0 && (UserAccess.HasAccess(Privileges.CanViewUsers, UserId) || this.User.IsInRole(Privileges.Admin))))
            {
                return RedirectToHome();
            }
            var refundSecurityDepositResult = _braintreePaymentsService.RefundSecurityDeposit(
                GoodRequestId: id,
                userId: UserId.Value
            );
            return RedirectToAction("Booking", new { id = id });
        }
        [Authorize]
        [HttpGet]
        public ActionResult BookingReview(int id)
        {
            if (!UserId.HasValue || !UserAccess.HasAccess(Privileges.CanViewUsers, UserId))
                return RedirectToHome();
            return GetBooking(id, "BookingReview");
        }
        [Authorize]
        [HttpPost]
        public ActionResult BookingReview(GoodRequestRankInsertModel model)
        {
            if (!UserId.HasValue || !UserAccess.HasAccess(Privileges.CanViewUsers, UserId))
                return RedirectToHome();
            if (ModelState.IsValid)
            {
                var result = _goodRequestService.SharerLeavesReview(model, UserId.Value, QuickUrl);
                if (result.CreateResult == CreateResult.Success)
                {
                    return RedirectToAction("Booking", new { id = model.GoodRequestId });
                }
            }
            return RedirectToAction("Booking", new { id = model.GoodRequestId });   
        }


        public JsonResult GetCouponDetail(string CouponCode, string CustomerCost, int GoodId, string StartDate, string EndDate, double ShippingDistance, bool ApplyForDelivery)
        {
            var result = (dynamic)null;
            string message = string.Empty;
            double newCustomerCost = 0;
            double newdiscountAmount = 0;
            //double res = 0;

            string customerCost = string.Empty;
            customerCost = CustomerCost.Replace("$", string.Empty);
            double customerCostInDouble = 0;
            double.TryParse(customerCost, out customerCostInDouble);

            int currentuserid = 0;
            if (UserId.HasValue)
            {
                int.TryParse(Convert.ToString(UserId.Value), out currentuserid);
            }

            Result<PriceViewModel> result1 = _itemDataService.CalculateCouponAmount(CouponCode, customerCostInDouble, GoodId,
                StartDate, EndDate, ShippingDistance, ApplyForDelivery, currentuserid);
            if (result1 != null)
            {
                if (result1.Obj.CouponDiscount == -1)
                {
                    result = Json(new { message = "Invalid Coupon", result1.Obj.CouponDiscount });
                }
                else if (result1.Obj.CouponDiscount == -2)
                {
                    result = Json(new { message = "Discount amount cannot be greater than total amount.", result1.Obj.CouponDiscount });
                }
                else
                {
                    newCustomerCost = result1.Obj.CustomerCost - result1.Obj.CouponDiscount;
                    result = Json(new { message = "", result1.Obj.CouponDiscount, newCustomerCost });
                }
            }
            else
            {
                result = Json(new { message = "Something went wrong!", result1.Obj.CouponDiscount });
            }


            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Authorize]        [HttpPost]        public ActionResult FinalFeedback(FinalFeedbackVM model)        {            int userid = 0;
            userid = UserId.Value;
            var getgoodRequestData = _goodRequestService.GetGoodRequest(model.RequestId);            var result = _paymentService.SaveFinalFeedback(model, userid);            string status = "";            if (model.NoIssue == true)
            {
                status = "No Issue";
            }
            if (model.Late == true)
            {
                status = "Returned Late";
            }            if (model.Damaged == true)
            {
                status = "Returned Damaged";
            }
            if (model.Late == true && model.Damaged == true)
            {
                status = "Returned Late And Damaged";
            }            var user = _accountDataService.GetUser(userid);            if (user != null)
            {
                var sent = _sendMessageService.SendFinalConfirmation(getgoodRequestData.Obj.UserEmail, status, getgoodRequestData.Obj.UserName, user.FullName == "" ? user.FirstName + " " + user.LastName:user.FullName);
            }            if (result)            {                return Json("Success", JsonRequestBehavior.AllowGet);            }            else            {                return Json("Failed", JsonRequestBehavior.AllowGet);            }        }


        public ActionResult RentedItems()
        {
            int userId = 0;
            if (UserId.HasValue)
            {
                int.TryParse(Convert.ToString(UserId.Value), out userId);
            }
            var requests = _accountDataService.GetRentedItems(userId);
            return View(requests);
        }

        //public ActionResult RentedItemDetail(int requestId)
        //{
        //    int userId = 0;
        //    if (UserId.HasValue)
        //    {
        //        int.TryParse(Convert.ToString(UserId.Value), out userId);
        //    }
        //    var requests = _accountDataService.GetRentedItems(userId);
        //    return View(requests);
        //}


        public string getThankyouCoupon()
        {
           
            var cuoponDetail = _accountDataService.GetDetailThankYouCoupon();
            string couponCode = string.Empty;
            double discountAmount = 0;
            string discountType = string.Empty;
            if (cuoponDetail != null)
            {
                couponCode = cuoponDetail.CouponCode;
                discountAmount = cuoponDetail.CouponDiscount;
                discountType = cuoponDetail.CouponDiscountType == 1 ? "%" : "$";
            }
            else
            {
                couponCode = "XXXXXXX";
                discountAmount = 0;
                discountType = "";
            }
            return couponCode;
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
                    if (checkRequest != true)
                    {
                        var userNotificationCreateModel = new UserNotificationCreateModel
                        {
                            UserId = getUserRequestResult.Obj.UserId,
                            Text = $"Sharer has started the ride.Click here to track location.",
                            Url = $"{HttpContext.Request.Url.GetLeftPart(UriPartial.Authority)}/Booking/Location/{id}"
                        };
                        _userNotificationService.AddNotification(userNotificationCreateModel);

                        var borrowerCoordinates = await GetCoordinatesAsync(getUserRequestResult.Obj.ShippingAddress);

                        Obj = _liveLocationService.AddLocation(getUserRequestResult.Obj.UserId, id, getUserRequestResult.Obj.OwnerId, momentarilyItem.Obj.Latitude, momentarilyItem.Obj.Longitude, borrowerCoordinates.Item1, borrowerCoordinates.Item2, DeliverBy.SHARER);
                    }
                    else
                    {
                        Obj = _liveLocationService.GetByRequestId(id);
                    }
                }
                else
                {
                    Obj = _liveLocationService.GetByRequestId(id);
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
                if (DateTime.Now > getUserRequestResult.Obj.EndDate)
                {
                    var userNotificationCreateModel = new UserNotificationCreateModel
                    {
                        UserId = UserId.Value,
                        Text = $"Rental time has been completed.",
                        Url = $"{HttpContext.Request.Url.GetLeftPart(UriPartial.Authority)}/Listing/Booking/{id}"
                    };
                    _userNotificationService.AddNotification(userNotificationCreateModel);
                } else
                {
                    var userNotificationCreateModel = new UserNotificationCreateModel
                    {
                        UserId = UserId.Value,
                        Text = $"Delivery time has not started yet.",
                        Url = $"{HttpContext.Request.Url.GetLeftPart(UriPartial.Authority)}/Listing/Booking/{id}"
                    };
                    _userNotificationService.AddNotification(userNotificationCreateModel);
                }
                return RedirectToAction("Booking", new { id = id });
            }
        }

        [HttpPost, ActionName("Location")]
        public JsonResult PostLocation(string locationId)
        {
            var liveLocation = _liveLocationService.fetchLocation(locationId);
            return Json(new { lat = liveLocation.BorrowerLatitude, lng = liveLocation.BorrowerLongitude });
        }

        [HttpPost, ActionName("UpdateLocation")]
        public JsonResult UpdateLocation(string locationId, double lat, double lng)
        {
            var liveLocation = _liveLocationService.UpdateSharerLocation(locationId, UserId.Value, Convert.ToDouble(lat), Convert.ToDouble(lng));
            return Json(new { status = "success" });
        }

        [HttpGet]
        public ActionResult ConfirmDelivery(int id)
        {
            var liveLocation = _liveLocationService.ConfirmDelivery(id);
            return RedirectToAction("Booking", new { id = id });
        }

        [HttpGet]
        public ActionResult ReturnConfirm(int id)
        {
            var liveLocation = _liveLocationService.ReturnConfirm(id);
            return RedirectToAction("Booking", new { id = id });
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

            var checkRequest = _liveLocationService.checkRequest(id);

            if (liveLocation.ReturnConfirm == false && liveLocation.DeliveryConfirm == true)
            {
                if (DateTime.Now <= getUserRequestResult.Obj.EndDate && DateTime.Now >= getUserRequestResult.Obj.StartDate && yesterday < DateTime.Now)
                {
                    if (getUserRequestResult.Obj.ApplyForDelivery)
                    {
                        if (checkRequest != true)
                        {
                            var userNotificationCreateModel = new UserNotificationCreateModel
                            {
                                UserId = getUserRequestResult.Obj.UserId,
                                Text = $"Sharer has started the ride.Click here to track location.",
                                Url = $"{HttpContext.Request.Url.GetLeftPart(UriPartial.Authority)}/Booking/Return/{id}"
                            };
                            _userNotificationService.AddNotification(userNotificationCreateModel);

                            var borrowerCoordinates = await GetCoordinatesAsync(getUserRequestResult.Obj.ShippingAddress);

                            Obj = _liveLocationService.AddLocation(getUserRequestResult.Obj.UserId, id, getUserRequestResult.Obj.OwnerId, momentarilyItem.Obj.Latitude, momentarilyItem.Obj.Longitude, borrowerCoordinates.Item1, borrowerCoordinates.Item2, DeliverBy.SHARER);
                        }
                        else
                        {
                            var userNotificationCreateModel = new UserNotificationCreateModel
                            {
                                UserId = getUserRequestResult.Obj.UserId,
                                Text = $"Sharer has started the ride.Click here to track location.",
                                Url = $"{HttpContext.Request.Url.GetLeftPart(UriPartial.Authority)}/Booking/Return/{id}"
                            };
                            _userNotificationService.AddNotification(userNotificationCreateModel);

                            Obj = _liveLocationService.GetByRequestId(id);
                        }
                    } else
                    {
                        Obj = _liveLocationService.GetByRequestId(id);
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
                            Url = $"{HttpContext.Request.Url.GetLeftPart(UriPartial.Authority)}/Listing/Booking/{id}"
                        };
                        _userNotificationService.AddNotification(userNotificationCreateModel);
                    }
                    else
                    {
                        var userNotificationCreateModel = new UserNotificationCreateModel
                        {
                            UserId = UserId.Value,
                            Text = $"Return time has not started yet.",
                            Url = $"{HttpContext.Request.Url.GetLeftPart(UriPartial.Authority)}/Listing/Booking/{id}"
                        };
                        _userNotificationService.AddNotification(userNotificationCreateModel);
                    }
                    return RedirectToAction("Booking", new { id = id });
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
                return RedirectToAction("Booking", new { id = id });
            }
            else 
            {
                var userNotificationCreateModel = new UserNotificationCreateModel
                {
                    UserId = UserId.Value,
                    Text = $"You cannot start return. The Item is not delivered yet.",
                    Url = $"{HttpContext.Request.Url.GetLeftPart(UriPartial.Authority)}/Listing/Booking/{id}"
                };
                _userNotificationService.AddNotification(userNotificationCreateModel);
                return RedirectToAction("Booking", new { id = id });
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