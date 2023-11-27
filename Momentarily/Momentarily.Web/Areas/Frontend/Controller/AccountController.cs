
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Apeek.Common;
using Apeek.Common.Controllers;
using Apeek.Entities.Web;
using Apeek.ViewModels.Models;
using Apeek.Web.Framework.Auth;
using Apeek.Web.Framework.ControllerHelpers;
using Apeek.Web.Framework.Controllers;
using Momentarily.ViewModels.Models;
using Microsoft.Web.WebPages.OAuth;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Facebook;
using System.Web;
using Momentarily.Test.Core.Service.Impl;
using Apeek.NH.Repository.Repositories;
using Apeek.Core.Services;
using System.Linq;
using Apeek.Common.Models;
using Apeek.Entities.Entities;
using System.Globalization;
using Momentarily.Entities.Entities;

namespace Momentarily.Web.Areas.Frontend.Controller
{
    public class AccountController : FrontendController// BaseAccountController<MomentarilyRegisterModel>
    {
        private readonly AccountControllerHelper<MomentarilyRegisterModel> _helper;
        private readonly ExternalLoginControllerHelper _externalLoginHelper;
        private string ClientId = ConfigurationManager.AppSettings["Google.ClientID"];
        private string SecretKey = ConfigurationManager.AppSettings["Google.SecretKey"];
        //private string RedirectUrl = ConfigurationManager.AppSettings["Google.RedirectUrl"];
        //private string RedirectUrlForLogin = ConfigurationManager.AppSettings["Google.RedirectUrlForLogin"];
        private string SubscriberListId = "hMFdFOy89j4lo14A6v5zaw";
        private string SignUpListId = "vt9kjw5GnqLH2iyCPZRZFA";
        private string SendybrandEmailAddress = "hello@momentarily.com";
        public static string OTP = "None";
        private readonly IAccountDataService _accountDataService;
        private readonly ISendMessageService _sendMessageService;
        private readonly IUserDataService<MomentarilyItem> _userDataService;
        private readonly ITwilioNotificationService _twilioNotificationService;
        private string RedirectUrl = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + "/Account/SaveUser";
        private string RedirectUrlForLogin = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + "/Account/LoginUser";
        public AccountController(IAccountDataService accountDataService, ISendMessageService sendMessageService, ITwilioNotificationService twilioNotificationService, IUserDataService<MomentarilyItem> userDataService)
        {
            _helper = new AccountControllerHelper<MomentarilyRegisterModel>();
            _externalLoginHelper = new ExternalLoginControllerHelper();
            _accountDataService = accountDataService;
            _sendMessageService = sendMessageService;
            _twilioNotificationService = twilioNotificationService;
            _userDataService = userDataService;
        }



        [AllowAnonymous]
        public ActionResult Register()
        {
            // var validateOTP = _helper.ManageOTPRequests(6364, 3);
            TempData["NoAccessFooter"] = "NoAccess";
            TempData["password"] = "";
            TempData["confirm"] = "";
            TempData["date"] = "empty";
            ViewBag.SiteKey = ConfigurationManager.AppSettings["CaptchaSitekey"];
            var model = Activator.CreateInstance<MomentarilyRegisterModel>();
            model.CountryId = 1;
            var CountriesyList = new SelectList(_helper.getAllCountries(), "PhoneCode", "Name");
            ViewBag.Countries = CountriesyList;
            var shape = _shapeFactory.BuildShape(null, model, PageName.Register.ToString());
            shape.ViewModel.IsExternal = false;
            return DisplayShape(shape);
            //return base.RegisterGet();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Register(Shape<MomentarilyRegisterModel> shape)
        {
            string EncodedResponse = Request.Form["g-Recaptcha-Response"];
            string SecretKey = ConfigurationManager.AppSettings["CaptchaSecretKey"].ToString();
            int OTPAllowed = Convert.ToInt32(ConfigurationManager.AppSettings["OTPRequestAllowed"]);
            bool IsCaptchaValid = (ReCaptchaClass.Validate(EncodedResponse, SecretKey) == "true" ? true : false);

            if (!IsCaptchaValid)
            {
                //Not-Valid Request
                shape.IsError = true;
                TempData["NoAccessFooter"] = "NoAccess";
                ModelState.AddModelError("ViewModel.RecaptchaMessage", "Please verify Google Recaptcha");

                ModelState.SetModelValue("g-Recaptcha-Response", new ValueProviderResult(string.Empty, string.Empty, CultureInfo.InvariantCulture));

                ViewBag.SiteKey = ConfigurationManager.AppSettings["CaptchaSitekey"];
                // return View(shape);
            }
            var yearDiff = 0;
            int _year = 0;
            int _month = 0;
            int _day = 0;

            if (shape.ViewModel.FirstName != null)
            {
                shape.ViewModel.FirstName = FirstCharToUpper(shape.ViewModel.FirstName);
            }
            if (shape.ViewModel.LastName != null)
            {
                shape.ViewModel.LastName = FirstCharToUpper(shape.ViewModel.LastName);
            }

            var CountriesyList = new SelectList(_helper.getAllCountries(), "PhoneCode", "Name");
            ViewBag.Countries = CountriesyList;
            if (shape.ViewModel.Password != shape.ViewModel.ConfirmPassword)
            {
                shape.ViewModel.ConfirmPassword = "";
            }
            TempData["password"] = shape.ViewModel.Password;
            TempData["confirm"] = shape.ViewModel.ConfirmPassword;

            if (shape.ViewModel.DateOfBirthday.Year != 1)
            {
                shape.ViewModel.DateOfBirthday = DateTime.Parse(shape.ViewModel.DateOfBirthday.ToString("MM/dd/yyyy"));
                var date = shape.ViewModel.DateOfBirthday.ToShortDateString();
                //yearDiff = DateTime.Now.Year - shape.ViewModel.DateOfBirthday.Year;
                int[] aa = CalculateYourAge(shape.ViewModel.DateOfBirthday);
                _year = aa[0];
                _month = aa[1];
                _day = aa[2];
                yearDiff = _year;
            }
            if (yearDiff >= 18)
            {
                ViewBag.first_name = shape.ViewModel.FirstName;

                var user = _helper.Register(shape, QuickUrl, ModelState, ControllerContext.HttpContext.Request);

                if (user != null)
                {
                    // var validateOTP = _helper.ManageOTPRequests(user.Id, OTPAllowed);
                    TwilioSmsSendProviderTest test = new TwilioSmsSendProviderTest();
                    //OTP = test.GenerateOTP();
                    //bool sent = test.SendOTP(OTP, shape.ViewModel.PhoneNumber, user.VerificationCode, Convert.ToString(shape.ViewModel.CountryId));
                    OTP = _twilioNotificationService.GenerateOTP();
                    bool sent = _twilioNotificationService.SendOTPVerificationCode(shape.ViewModel.PhoneNumber, Convert.ToString(shape.ViewModel.CountryId), user.VerificationCode, OTP, user.Id);

                    bool isUpdated = _accountDataService.UpdateOTPRequests(user.Id);
                    if (shape.ViewModel.IgnoreMarketingEmails != false)
                    {
                        _helper.subscribeEmail(SubscriberListId, user.Email, user.FirstName + " " + user.LastName, true);
                        bool saveSubscriber = _helper.saveSubscriber(shape.ViewModel.Email);
                    }

                    int userId = user.Id;
                    User retrievedUser = _accountDataService.GetUser(userId);
                    if (retrievedUser != null)
                    {
                        bool isGeneralUpdatesEnabled = shape.ViewModel.GeneralUpdate;
                        //Unsubscribe(userId);
                    }

                    //if (shape.ViewModel.IsExternal == true)
                    //{
                    //    var redirectTo = QuickUrl.RegisterMessageSentUrl(user.Email);
                    //    return AuthenticateUserWithPrivilagesAndRedirect(user.Id, redirectTo);
                    //}
                    //else
                    //{
                    //string ItemListURL = "/Search/Map?Location=&Latitude=0&Longitude=0&SearchByMap=false&NeLatitude=0&NeLongitude=0&SwLatitude=0&SwLongitude=0&DateStart=%2FDate(1575969961000)%2F&DateEnd=%2FDate(1577179561000)%2F&Page=1&PageSize=21&Radius=25&Keyword=&RentPeriod=1&SortBy=1";
                    //var sendMsgWelcomeUser = _sendMessageService.SendEmailWelcomeTemplate(user.Email, user.FullName, ItemListURL);
                    MessageSentModel sentmodel = new MessageSentModel();
                    sentmodel.IsExternal = false;
                    sentmodel.VC = user.VerificationCode;
                    sentmodel.PhoneNumber = shape.ViewModel.PhoneNumber;
                    sentmodel.Email = user.Email;
                    sentmodel.IsVerified = false;
                    sentmodel.IsMobileVerified = false;
                    return RedirectToAction("OTPMessageSent", sentmodel);
                }
            }
            else
            {
                TempData["NoAccessFooter"] = "NoAccess";
                shape.IsError = true;
                ModelState.AddModelError("ViewModel.DateOfBirthday", "You need to be at least 18 years old to create an account");
            }
            return View(shape);
        }

        public ActionResult Unsubscribe(int userId)
        {
            var user = _accountDataService.GetUser(userId);
            if (user != null)
            {
                _accountDataService.UpdateGeneralUpdateColumn(userId);
            }
            return View();
        }


        private string FirstCharToUpper(string input)
        {
            switch (input)
            {
                case null: throw new ArgumentNullException(nameof(input));
                case "": throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
                default: return input.First().ToString().ToUpper() + input.Substring(1);
            }
        }


        public void GoogleSignUp()
        {
            //string ss = domainName;
            Response.Redirect($"https://accounts.google.com/o/oauth2/v2/auth?client_id={"" + ClientId + ""}&response_type=code&scope=openid%20email%20profile&redirect_uri={"" + RedirectUrl + ""}&state=abcdef&prompt=consent");
        }
        public static int[] CalculateYourAge(DateTime Dob)
        {
            int[] _data = new int[5];
            DateTime Now = DateTime.Now;
            int Years = new DateTime(DateTime.Now.Subtract(Dob).Ticks).Year - 1;
            DateTime PastYearDate = Dob.AddYears(Years);
            int Months = 0;
            for (int i = 1; i <= 12; i++)
            {
                if (PastYearDate.AddMonths(i) == Now)
                {
                    Months = i;
                    break;
                }
                else if (PastYearDate.AddMonths(i) >= Now)
                {
                    Months = i - 1;
                    break;
                }
            }
            int Days = Now.Subtract(PastYearDate.AddMonths(Months)).Days;
            int Hours = Now.Subtract(PastYearDate).Hours;
            int Minutes = Now.Subtract(PastYearDate).Minutes;
            int Seconds = Now.Subtract(PastYearDate).Seconds;
            _data[0] = Years;
            _data[1] = Months;
            _data[2] = Days;
            _data[3] = Minutes;
            _data[4] = Seconds;
            return _data;
            //return String.Format("Age: {0} Year(s) {1} Month(s) {2} Day(s) {3} Hour(s) {4} Second(s)",
            //Years, Months, Days, Hours, Seconds);
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> SaveUser(string code, string state, string session_state)
        {
            if (string.IsNullOrEmpty(code))
            {
                return View("Error");
            }
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://www.googleapis.com")
            };
            var requestUrl = $"oauth2/v4/token?code={code}&client_id={"" + ClientId + ""}&client_secret={"" + SecretKey + ""}&redirect_uri={"" + RedirectUrl + ""}&grant_type=authorization_code";
            var dict = new Dictionary<string, string>
            {
                { "Content-Type", "application/x-www-form-urlencoded" }
            };
            var req = new HttpRequestMessage(System.Net.Http.HttpMethod.Post, requestUrl) { Content = new FormUrlEncodedContent(dict) };
            var response = await httpClient.SendAsync(req);
            var token = JsonConvert.DeserializeObject<GmailToken>(await response.Content.ReadAsStringAsync());
            Session["user"] = token.AccessToken;
            var obj = await GetuserProfile(token.AccessToken);
            if (obj.Id == null || obj.Email == null)
            {
                return RedirectToAction("Register");
            }
            //IdToken property stores user data in Base64Encoded form  
            //var data = Convert.FromBase64String(token.IdToken.Split('.')[1]);  
            //var base64Decoded = System.Text.ASCIIEncoding.ASCII.GetString(data);  
            var model = Activator.CreateInstance<MomentarilyRegisterModel>();
            model.Email = obj.Email;
            model.FirstName = obj.GivenName;
            model.LastName = obj.FamilyName;
            model.GoogleId = Convert.ToString(obj.Id);
            model.IsExternal = true;
            model.CountryId = 1;
            var CountriesyList = new SelectList(_helper.getAllCountries(), "PhoneCode", "Name");
            ViewBag.Countries = CountriesyList;
            var shape = _shapeFactory.BuildShape(null, model, PageName.Register.ToString());
            //DisplayShape(shape);
            return View("Register", shape);
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult SaveUser(Shape<MomentarilyRegisterModel> shape)
        {
            var yearDiff = 0;
            int _year = 0;
            int _month = 0;
            int _day = 0;
            //if (shape.ViewModel.DateOfBirthday.Year != 1)
            //{
            //    shape.ViewModel.DateOfBirthday = DateTime.Parse(shape.ViewModel.DateOfBirthday.ToString("MM/dd/yyyy"));
            //    yearDiff = DateTime.Now.Year - shape.ViewModel.DateOfBirthday.Year;
            //}
            if (shape.ViewModel.DateOfBirthday.Year != 1)
            {
                shape.ViewModel.DateOfBirthday = DateTime.Parse(shape.ViewModel.DateOfBirthday.ToString("MM/dd/yyyy"));
                var date = shape.ViewModel.DateOfBirthday.ToShortDateString();
                int[] aa = CalculateYourAge(shape.ViewModel.DateOfBirthday);
                _year = aa[0];
                _month = aa[1];
                _day = aa[2];
                yearDiff = _year;
            }
            var CountriesyList = new SelectList(_helper.getAllCountries(), "PhoneCode", "Name");
            ViewBag.Countries = CountriesyList;
            if (shape.ViewModel.Password != shape.ViewModel.ConfirmPassword)
            {
                shape.ViewModel.ConfirmPassword = "";
            }
            TempData["password"] = shape.ViewModel.Password;
            TempData["confirm"] = shape.ViewModel.ConfirmPassword;

            if (yearDiff >= 18)
            {
                var user = _helper.Register(shape, QuickUrl, ModelState, ControllerContext.HttpContext.Request);
                if (user != null)
                {
                    //TwilioSmsSendProviderTest test = new TwilioSmsSendProviderTest();
                    //OTP = test.GenerateOTP();
                    //bool sent = test.SendOTP(OTP, shape.ViewModel.PhoneNumber, user.VerificationCode, Convert.ToString(shape.ViewModel.CountryId));
                    OTP = _twilioNotificationService.GenerateOTP();
                    bool sent = _twilioNotificationService.SendOTPVerificationCode(shape.ViewModel.PhoneNumber, Convert.ToString(shape.ViewModel.CountryId), user.VerificationCode, OTP, user.Id);
                    bool isUpdated = _accountDataService.UpdateOTPRequests(user.Id);
                    MessageSentModel sentmodel = new MessageSentModel();
                    sentmodel.IsExternal = true;
                    sentmodel.VC = user.VerificationCode;
                    sentmodel.PhoneNumber = shape.ViewModel.PhoneNumber;
                    sentmodel.Email = user.Email;
                    sentmodel.IsVerified = false;
                    sentmodel.IsMobileVerified = false;
                    return RedirectToAction("OTPMessageSent", sentmodel);

                    //var redirectTo = QuickUrl.RegisterMessageSentUrl(user.Email);
                    //return AuthenticateUserWithPrivilagesAndRedirect(user.Id, redirectTo);
                }
            }
            else
            {
                shape.IsError = true;
                ModelState.AddModelError("ViewModel.DateOfBirthday", "You need to be at least 18 years old to create an account");
            }
            return View("Register", shape);
        }
        public async Task<UserProfiles> GetuserProfile(string accesstoken)
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://www.googleapis.com")
            };
            string url = $"https://www.googleapis.com/oauth2/v1/userinfo?alt=json&access_token={"" + accesstoken + ""}";
            var response = await httpClient.GetAsync(url);
            return JsonConvert.DeserializeObject<UserProfiles>(await response.Content.ReadAsStringAsync());
        }
        public void GoogleSignIn()
        {
            Response.Redirect($"https://accounts.google.com/o/oauth2/v2/auth?client_id={"" + ClientId + ""}&response_type=code&scope=openid%20email%20profile&redirect_uri={"" + RedirectUrlForLogin + ""}&state=abcdef&prompt=consent");
        }
        [AllowAnonymous]
        public async Task<ActionResult> LoginUser(string code, string state, string session_state)
        {
            if (string.IsNullOrEmpty(code))
            {
                return View("Error");
            }
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://www.googleapis.com")
            };
            var requestUrl = $"oauth2/v4/token?code={code}&client_id={"" + ClientId + ""}&client_secret={"" + SecretKey + ""}&redirect_uri={"" + RedirectUrlForLogin + ""}&grant_type=authorization_code";
            var dict = new Dictionary<string, string>
            {
                { "Content-Type", "application/x-www-form-urlencoded" }
            };
            var req = new HttpRequestMessage(System.Net.Http.HttpMethod.Post, requestUrl) { Content = new FormUrlEncodedContent(dict) };
            var response = await httpClient.SendAsync(req);
            var token = JsonConvert.DeserializeObject<GmailToken>(await response.Content.ReadAsStringAsync());
            Session["user"] = token.AccessToken;
            var obj = await GetuserProfile(token.AccessToken);
            if (obj.Id == null || obj.Email == null)
            {
                return RedirectToAction("Login");
            }
            var loginResult = _helper.GetGoogleUser(obj.Id, obj.Email);
            if (loginResult != null)
            {
                var redirectTo = ReturnUrl;
                return AuthenticateUserWithPrivilagesAndRedirect(loginResult.Id, redirectTo);
            }
            Shape<LoginModel> shape = new Shape<LoginModel>();
            LoginModel model = new LoginModel();
            shape.ViewModel = model;
            ViewBag.RedirectTo = ReturnUrl;
            shape.IsError = true;
            shape.Message = "User does not exist.";
            return View("Login", shape);
        }
        [AllowAnonymous]
        public ActionResult Login()
        {
            TempData["NoAccessFooter"] = "NoAccess";
            if (ContextService.IsUserAuthenticated)
            {
                if (Convert.ToBoolean(Session["IsAutherised"]) == true || Convert.ToBoolean(Session["IsAdmin"]) == true)
                {
                    _helper.LogOff();
                    Response.Cookies.Remove("Login");
                    Session["IsAutherised"] = false;
                    Session["IsAdmin"] = false;
                    return RedirectToAction("Login");
                }
                else if (ContextService.IsUserAuthenticated && (Convert.ToBoolean(Session["IsAutherised"]) == false || Convert.ToBoolean(Session["IsAdmin"]) == false))
                {
                    _helper.LogOff();
                    Response.Cookies.Remove("Login");
                    Session["IsAutherised"] = false;
                    Session["IsAdmin"] = false;
                    return RedirectToAction("Login");
                }
                return Redirect(QuickUrl.UserProfileUrl());
            }

            ViewBag.RedirectTo = ReturnUrl;
            var shape = _shapeFactory.BuildShape(null, new LoginModel(), PageName.Login.ToString());
            if (Request.Cookies["Login"] != null)
            {
                // shape.ViewModel.EmailAddressOrPhoneNum = Request.Cookies["Login"].Values["EmailID"];
                //shape.ViewModel.Password = Request.Cookies["Login"].Values["Password"];
                shape.ViewModel.RememberMe = true;
            }
            if (!_helper.ExternalLoginError) return DisplayShape(shape);
            shape.IsError = _helper.ExternalLoginError;
            shape.Message = "Failure to login using external system.";
            return DisplayShape(shape);
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(Shape<LoginModel> shape)
        {
            //var lists=_helper.getCampaignList("hello@momentarily.com");
            // shape.ViewModel.EmailAddressOrPhoneNum = shape.ViewModel.EmailAddressOrPhoneNum.Trim();
            if (ContextService.IsUserAuthenticated)
                return Redirect(QuickUrl.UserProfileUrl());
            var loginResult = _helper.Login(shape, ReturnUrl, ModelState);  
            if (loginResult != null && loginResult.LoginStatus != LoginStatus.Fail)
            {
                if (loginResult.User.IsBlocked !=true)
                {
                    if (shape.ViewModel.RememberMe)
                    {
                        HttpCookie cookie = new HttpCookie("Login");
                        cookie.Values.Add("EmailID", shape.ViewModel.EmailAddressOrPhoneNum);
                        cookie.Values.Add("Password", shape.ViewModel.Password);
                        cookie.Expires = DateTime.Now.AddDays(15);
                        Response.Cookies.Add(cookie);
                    }
                    var redirectTo = ReturnUrl;
                    if (loginResult.LoginStatus != LoginStatus.SuccessWithTempPwd)
                        return AuthenticateUserWithPrivilagesAndRedirect(loginResult.User.Id, redirectTo);
                    redirectTo = QuickUrl.UserPwdUrl();
                    if (string.IsNullOrWhiteSpace(loginResult.User.Email))
                        redirectTo = QuickUrl.UserEmailUrl(new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>(QS.source, "tmp_pwd") });
                    return AuthenticateUserWithPrivilagesAndRedirect(loginResult.User.Id, redirectTo);
                }
                else
                {
                    //TempData["NoAccessFooter"] = "NoAccess";
                    //shape.Message = "Email and password do not match.";.
                    TempData["NoAccessFooter"] = "NoAccess";
                    shape.Message = "Your account is blocked please contact hello@momentarily.com.";
                }
                
            }
            //var Result = _helper.GetBlocked(shape, ReturnUrl, ModelState);

            //if (Result != null && Result.LoginStatus != LoginStatus.Fail)
            //{
            //    TempData["NoAccessFooter"] = "NoAccess";
            //    shape.Message = "Your account has been blocked by admin";
            //}
            else
            {
                TempData["NoAccessFooter"] = "NoAccess";
                shape.Message = "Email and password do not match.";
            }
            ViewBag.RedirectTo = ReturnUrl;
            shape.IsError = true;
            return View(shape);
        }
        [AllowAnonymous]
        public ActionResult RestorePwd()
        {
            TempData["NoAccessFooter"] = "NoAccess";
            var shape = _shapeFactory.BuildShape(null, new UserRestorePwd());
            return DisplayShape(shape);
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult RestorePwd(Shape<UserRestorePwd> shape)
        {
            TempData["NoAccessFooter"] = "NoAccess";
            shape = _helper.RestorePwd(shape, QuickUrl, ModelState);
            return DisplayShape(shape);
        }
        [AllowAnonymous]
        public ActionResult LogOff()
        {
            _helper.LogOff();
            Response.Cookies.Remove("Login");
            Session["IsUserLogin"] = false;
            return Redirect(QuickUrl.HomeUrl());
        }
        [AllowAnonymous]
        public ActionResult QuickLogin(string ue, string vc, string redirectTo)
        {
            var user = _helper.QuickLogin(ue, vc, redirectTo);
            return user == null
                ? Redirect(QuickUrl.HomeUrl())
                : AuthenticateUserWithPrivilagesAndRedirect(user.Id, redirectTo);
        }
        [AllowAnonymous]
        public ActionResult QuickLogin2(string ue, string vc, string redirectTo)
        {
            var user = _helper.QuickLogin(ue, vc, redirectTo);
            if (user != null && user.SendLinkDate != null &&
                DateTime.Now.Subtract(Convert.ToDateTime(user.SendLinkDate)).TotalHours > 24)
            {
                return Redirect(QuickUrl.LinkExpireUrl());
            }
            return user == null
                ? Redirect(QuickUrl.HomeUrl())
                : AuthenticateUserWithPrivilagesAndRedirect(user.Id, redirectTo);
        }
        [Authorize]
        [HttpGet]
        public ActionResult RegisterMessageSent(string email)
        {
            MessageSentModel model = new MessageSentModel
            {
                Email = email,
                IsExternal = true

            };
            var shape = _shapeFactory.BuildShape(null, model, PageName.LoginMessageSent.ToString());
            return DisplayShape(shape);
        }

        [HttpGet]
        public ActionResult OTPMessageSent(MessageSentModel model)
        {
            ViewBag.AllowedOTP = Convert.ToInt32(ConfigurationManager.AppSettings["OTPRequestAllowed"]);
            TempData["NoAccessFooter"] = "NoAccess";
            var shape = _shapeFactory.BuildShape(null, model, PageName.LoginMessageSent.ToString());
            return View("RegisterMessageSent", shape);
        }

        public dynamic MatchOTP(string otp, string Vcode)
        {
            if (otp == OTP)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public JsonResult ResendOTP(string vc, string number)
        {
            Result<User> validateOTP = new Result<User>();
            try
            {
                int OTPAllowed = Convert.ToInt32(ConfigurationManager.AppSettings["OTPRequestAllowed"]);
                int userId = _accountDataService.GetUser(vc).Id;
                validateOTP = _helper.ManageOTPRequests(userId, OTPAllowed);
                if (validateOTP.Success)
                {
                    //TwilioSmsSendProviderTest test = new TwilioSmsSendProviderTest();
                    //OTP = test.GenerateOTP();
                    //var countrycode = _accountDataService.GetCountryCodeByPhoneNumber(number);
                    //bool sent = test.SendOTP(OTP, number, vc, countrycode);
                    OTP = _twilioNotificationService.GenerateOTP();
                    var countrycode = _accountDataService.GetCountryCodeByPhoneNumber(number);
                    bool sent = _twilioNotificationService.SendOTPVerificationCode(number, countrycode, vc, OTP, userId);
                    if (sent) _accountDataService.UpdateOTPRequests(userId);
                }
            }
            catch (Exception ex)
            {
                validateOTP.Success = false;
                validateOTP.StatusCode = "OTP_004";
                validateOTP.Message = validateOTP.StatusCode + "-Something went wrong ! Please contact to administrator";
            }
            return Json(validateOTP, JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult ExternalLoginsList(string redirectTo)
        {
            if (String.IsNullOrEmpty(redirectTo))
            {
                redirectTo = QuickUrl.UserProfileUrl();
                Response.Redirect($"https://accounts.google.com/o/oauth2/v2/auth?client_id={"" + ClientId + ""}&response_type=code&scope=openid%20email%20profile&redirect_uri={"" + RedirectUrl + ""}&state=abcdef");
            }
            ViewBag.RedirectTo = redirectTo;
            return PartialView("_ExternalLoginsListPartial", OAuthWebSecurity.RegisteredClientData);
        }
        [AllowAnonymous]
        public ActionResult ExternalLogin(string provider, string redirectTo)
        {
            if (String.IsNullOrEmpty(redirectTo))
            {
                redirectTo = QuickUrl.UserProfileUrl();
            }
            return new ExternalLoginResult(provider, Url.Action("ExternalLoginCallback", new { redirectTo = redirectTo }));
        }
        [AllowAnonymous]
        public ActionResult ExternalLoginCallback(string redirectTo)
        {
            if (string.IsNullOrWhiteSpace(redirectTo))
            {
                redirectTo = ContextService.IsUserAuthenticated
                    ? QuickUrl.UserAccountAssociacionsUrl()
                    : QuickUrl.UserProfileUrl();
            }
            var result = _externalLoginHelper.VerifyAuthentication(redirectTo, Url.Action("ExternalLoginCallback", "Account", new { redirectTo = redirectTo }));
            if (!result.IsSuccessful) return Redirect(QuickUrl.LoginUrl(redirectTo, true));
            var user = _externalLoginHelper.AuthenticateUser(result);
            if (user == null) return Redirect(QuickUrl.LoginUrl(redirectTo, true));
            return ContextService.IsUserAuthenticated
                ? RedirectToLocal(redirectTo)
                : AuthenticateUserWithPrivilagesAndRedirect(user.Id, redirectTo);
        }
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return DisplayShape(_shapeFactory.BuildShape(null, string.Empty));
        }
        [HttpGet]
        public ActionResult FacebookSignUp()
        {
            var fb = new FacebookClient();
            var loginUrl = fb.GetLoginUrl(new
            {
                client_id = "418392938876882",
                client_secret = "06b1b159a1d1b9461432fb537d2d1199",
                redirect_uri = FacebookRediredtUri.AbsoluteUri,
                response_type = "code",
                scope = "email"
            });
            return Redirect(loginUrl.AbsoluteUri);
        }
        [HttpGet]
        public ActionResult FacebookSignIn()
        {
            var fb = new FacebookClient();
            var loginUrl = fb.GetLoginUrl(new
            {
                client_id = "418392938876882",
                client_secret = "06b1b159a1d1b9461432fb537d2d1199",
                redirect_uri = FacebookRediredtUri1.AbsoluteUri,
                response_type = "code",
                scope = "email"
            });
            return Redirect(loginUrl.AbsoluteUri);
        }
        private Uri FacebookRediredtUri1
        {
            get
            {
                //var url = Convert.ToString(Request.Url);
                //if (!url.Contains("https"))
                //{
                //    url = url.Replace("http", "https");
                //}
                //var uriBuilder = new UriBuilder(url);
                var uriBuilder = new UriBuilder(Request.Url);
                uriBuilder.Query = null;
                uriBuilder.Fragment = null;
                uriBuilder.Path = Url.Action("FacebookCallback1");
                return uriBuilder.Uri;
            }
        }
        private Uri FacebookRediredtUri
        {
            get
            {
                //var url = Convert.ToString(Request.Url);
                ////if (!url.Contains("https"))
                ////{
                ////    url = url.Replace("http", "https");
                ////}
                //var uriBuilder = new UriBuilder(url);
                var uriBuilder = new UriBuilder(Request.Url);
                uriBuilder.Query = null;
                uriBuilder.Fragment = null;
                uriBuilder.Path = Url.Action("FacebookCallback");
                return uriBuilder.Uri;
            }
        }
        [HttpGet]
        public ActionResult FacebookCallback(string code)
        {
            try
            {
                if (code == null || code == string.Empty)
                {
                    return RedirectToAction("Register");
                }
                var fb = new FacebookClient();
                dynamic result = fb.Post("oauth/access_token", new
                {
                    client_id = "418392938876882",
                    client_secret = "06b1b159a1d1b9461432fb537d2d1199",
                    redirect_uri = FacebookRediredtUri.AbsoluteUri,
                    code = code
                });
                var accessToken = result.access_token;
                Session["AccessToken"] = accessToken;
                fb.AccessToken = accessToken;
                dynamic me = fb.Get("me?fields=link,first_name,currency,last_name,email,gender,locale,timezone,verified,picture,age_range");
                if (me != null)
                {
                    var model = Activator.CreateInstance<MomentarilyRegisterModel>();
                    model.Email = me.email;
                    model.FirstName = me.first_name;
                    model.LastName = me.last_name;
                    model.FacebookId = Convert.ToString(me.id);
                    model.IsExternal = true;
                    model.CountryId = 1;
                    var CountriesyList = new SelectList(_helper.getAllCountries(), "PhoneCode", "Name");
                    ViewBag.Countries = CountriesyList;
                    var shape = _shapeFactory.BuildShape(null, model, PageName.Register.ToString());
                    return View("Register", shape);
                }
                return RedirectToAction("Register");
            }
            catch
            {
                return RedirectToAction("Register");
            }
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult FacebookCallback(Shape<MomentarilyRegisterModel> shape)
        {
            var yearDiff = 0;
            int _year = 0;
            int _month = 0;
            int _day = 0;
            //if (shape.ViewModel.DateOfBirthday.Year != 1)
            //{
            //    shape.ViewModel.DateOfBirthday = DateTime.Parse(shape.ViewModel.DateOfBirthday.ToString("MM/dd/yyyy"));
            //    yearDiff = DateTime.Now.Year - shape.ViewModel.DateOfBirthday.Year;
            //}
            if (shape.ViewModel.DateOfBirthday.Year != 1)
            {
                shape.ViewModel.DateOfBirthday = DateTime.Parse(shape.ViewModel.DateOfBirthday.ToString("MM/dd/yyyy"));
                var date = shape.ViewModel.DateOfBirthday.ToShortDateString();
                int[] aa = CalculateYourAge(shape.ViewModel.DateOfBirthday);
                _year = aa[0];
                _month = aa[1];
                _day = aa[2];
                yearDiff = _year;
            }
            var CountriesyList = new SelectList(_helper.getAllCountries(), "PhoneCode", "Name");
            ViewBag.Countries = CountriesyList;
            if (shape.ViewModel.Password != shape.ViewModel.ConfirmPassword)
            {
                shape.ViewModel.ConfirmPassword = "";
            }
            TempData["password"] = shape.ViewModel.Password;
            TempData["confirm"] = shape.ViewModel.ConfirmPassword;
            if (yearDiff >= 18)
            {
                var user = _helper.Register(shape, QuickUrl, ModelState, ControllerContext.HttpContext.Request);
                if (user != null)
                {
                    //TwilioSmsSendProviderTest test = new TwilioSmsSendProviderTest();
                    //OTP = test.GenerateOTP();
                    //bool sent = test.SendOTP(OTP, shape.ViewModel.PhoneNumber, user.VerificationCode, Convert.ToString(shape.ViewModel.CountryId));
                    OTP = _twilioNotificationService.GenerateOTP();
                    bool sent = _twilioNotificationService.SendOTPVerificationCode(shape.ViewModel.PhoneNumber, Convert.ToString(shape.ViewModel.CountryId), user.VerificationCode, OTP, user.Id);

                    bool isUpdated = _accountDataService.UpdateOTPRequests(user.Id);
                    MessageSentModel sentmodel = new MessageSentModel();
                    sentmodel.IsExternal = true;
                    sentmodel.VC = user.VerificationCode;
                    sentmodel.PhoneNumber = shape.ViewModel.PhoneNumber;
                    sentmodel.Email = user.Email;
                    sentmodel.IsVerified = false;
                    sentmodel.IsMobileVerified = false;
                    return RedirectToAction("OTPMessageSent", sentmodel);
                    //var redirectTo = QuickUrl.RegisterMessageSentUrl(user.Email);
                    //return AuthenticateUserWithPrivilagesAndRedirect(user.Id, redirectTo);
                }
            }
            else
            {
                shape.IsError = true;
                ModelState.AddModelError("ViewModel.DateOfBirthday", "You need to have more than 18 years to create account");
            }
            return View("Register", shape);
        }
        [HttpGet]
        public ActionResult FacebookCallback1(string code)
        {
            try
            {
                if (code == null || code == string.Empty)
                {
                    return RedirectToAction("Login");
                }
                var fb = new FacebookClient();
                dynamic result = fb.Post("oauth/access_token", new
                {
                    client_id = "418392938876882",
                    client_secret = "06b1b159a1d1b9461432fb537d2d1199",
                    redirect_uri = FacebookRediredtUri1.AbsoluteUri,
                    code = code
                });
                var accessToken = result.access_token;
                Session["AccessToken"] = accessToken;
                fb.AccessToken = accessToken;
                dynamic me = fb.Get("me?fields=link,first_name,currency,last_name,email,gender,locale,timezone,verified,picture,age_range");
                if (me != null)
                {
                    var loginResult = _helper.GetFacebookUser(Convert.ToString(me.id), Convert.ToString(me.email));
                    if (loginResult != null)
                    {
                        var redirectTo = ReturnUrl;
                        return AuthenticateUserWithPrivilagesAndRedirect(loginResult.Id, redirectTo);
                    }
                    Shape<LoginModel> shape = new Shape<LoginModel>();
                    LoginModel model = new LoginModel();
                    shape.ViewModel = model;
                    ViewBag.RedirectTo = ReturnUrl;
                    shape.IsError = true;
                    shape.Message = "User does not exist.";
                    return View("Login", shape);
                }
                return RedirectToAction("Login");
            }
            catch
            {
                return RedirectToAction("Login");
            }
        }
        [HttpPost]
        public ActionResult Subscribe(string Email)
        {
            try
            {
                bool checkExsistSubscriber = _helper.ExsistSubscriber(Email);
                if (checkExsistSubscriber == false)
                {
                    string trimEmail = Email.Trim();
                    bool saveSubscriber = _helper.saveSubscriber(Email);
                    var message = _helper.subscribeEmail(SubscriberListId, Email, "", true);
                    //string promoCode = "PR0ABCD12";
                    string ItemListURL = "/Search/Map?Location=&Latitude=0&Longitude=0&SearchByMap=false&NeLatitude=0&NeLongitude=0&SwLatitude=0&SwLongitude=0&DateStart=%2FDate(1575969961000)%2F&DateEnd=%2FDate(1577179561000)%2F&Page=1&PageSize=21&Radius=25&Keyword=&RentPeriod=1&SortBy=1";
                    string _userName = _helper.ExsistSubscriberGetName(trimEmail);

                    var cuoponDetail = _accountDataService.GetDetailThankYouForSubscriberCoupon();
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
                    var sendMsgSubscriber = _sendMessageService.SendEmailThankYouTemplateForSubscribing(0, _userName, trimEmail, couponCode, ItemListURL, discountAmount, discountType);
                    //var sendlaunchsoon = _accountDataService.SendLaunchSoonEmail(trimEmail, new DateTime(2020, 2, 15));
                    //var sendlaunched = _accountDataService.SendLaunchedEmail(trimEmail);
                    return Json(message, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("Already subscribed.");
                }

            }
            catch
            {
                return Json("Something went wrong");
            }
        }

        [HttpGet]
        public ActionResult NewsLetterSubscribe(string Email)
        {
            TempData["SubscriberMsg"] = null;
            try
            {
                bool checkExsistSubscriber = _helper.ExsistSubscriber(Email);
                if (checkExsistSubscriber == false)
                {
                    string trimEmail = Email.Trim();
                    bool saveSubscriber = _helper.saveSubscriber(Email);
                    var message = _helper.subscribeEmail(SubscriberListId, Email, "", true);
                    //string promoCode = "PR0ABCD12";
                    string ItemListURL = "/Search/Map?Location=&Latitude=0&Longitude=0&SearchByMap=false&NeLatitude=0&NeLongitude=0&SwLatitude=0&SwLongitude=0&DateStart=%2FDate(1575969961000)%2F&DateEnd=%2FDate(1577179561000)%2F&Page=1&PageSize=21&Radius=25&Keyword=&RentPeriod=1&SortBy=1";
                    string _userName = _helper.ExsistSubscriberGetName(trimEmail);
                    var cuoponDetail = _accountDataService.GetDetailThankYouForSubscriberCoupon();
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

                    var sendMsgSubscriber = _sendMessageService.SendEmailThankYouTemplateForSubscribing(0, _userName, trimEmail, couponCode, ItemListURL, discountAmount, discountType);
                    //var sendlaunchsoon = _accountDataService.SendLaunchSoonEmail(trimEmail, new DateTime(2020, 2, 15));
                    TempData["SubscriberMsg"] = message;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["SubscriberMsg"] = "Already subscribed.";
                    return RedirectToAction("Index", "Home");
                }

            }
            catch
            {
                TempData["SubscriberMsg"] = "Something went wrong";
                return RedirectToAction("Index", "Home");
            }
        }


        [HttpGet]
        public ActionResult NewsLetterUnSubscribe(string Email)
        {
            TempData["UnSubscriberMsg"] = null;
            try
            {
                bool checkExsistSubscriber = _helper.ExsistSubscriber(Email);
                if (checkExsistSubscriber == true)
                {
                    string trimEmail = Email.Trim();
                    bool updateSubscriber = _helper.UpdateSubscriber(Email);
                    var message = _helper.unsubscribeEmail(SubscriberListId, Email, "", true);
                    if (updateSubscriber == true && message.Contains("Subscribed"))
                    {
                        TempData["UnSubscriberMsg"] = "Unsubscribe successfully.";
                    }
                    else
                    {
                        TempData["UnSubscriberMsg"] = "Already Unsubscribed.";
                    }
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["UnSubscriberMsg"] = "You can't subscribe until now.";
                    return RedirectToAction("Index", "Home");
                }

            }
            catch
            {
                TempData["UnSubscriberMsg"] = "Something went wrong";
                return RedirectToAction("Index", "Home");
            }
        }




    }


}