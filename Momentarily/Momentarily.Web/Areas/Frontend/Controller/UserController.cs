using System.Collections.Generic;
using System.Web.Mvc;
using Apeek.Common;
using Apeek.Common.Controllers;
using Apeek.Core.Services;
using Apeek.Entities.Web;
using Apeek.ViewModels.Models;
using Apeek.ViewModels.Models.Impl;
using Apeek.Web.Framework.ControllerHelpers;
using Apeek.Web.Framework.Controllers;
using Momentarily.Entities.Entities;
using Momentarily.UI.Service.Services;
using Momentarily.UI.Service.Services.Impl;
using Momentarily.Core.Services.Impl;
using BraintreePayments.Models;
using Momentarily.ViewModels.Models.Braintree;
using Apeek.Entities.Entities;
using Momentarily.Test.Core.Service.Impl;
using Apeek.Common.Definitions;
using System;
using Apeek.Core.Services.Impl;

namespace Momentarily.Web.Areas.Frontend.Controller
{
    public class UserController : FrontendController
    {
        private readonly UserControllerHelper _helper;
        private readonly IPinPaymentStoreDataService _pinPaymentStoreDataService;
        private readonly PinPaymentService _pinPaymentService;
        private readonly IUserDataService<MomentarilyItem> _userService;
        private readonly BraintreePaymentsService _braintreePaymentsService;
        private string SubscriberListId = "hMFdFOy89j4lo14A6v5zaw";
        private readonly AccountControllerHelper<IRegisterModel> _accountHelper;
        public UserController()
        {
            _helper = new UserControllerHelper();
            _pinPaymentService = new PinPaymentService();
            _pinPaymentStoreDataService = Ioc.Get<IPinPaymentStoreDataService>();
            _userService = Ioc.Get<IUserDataService<MomentarilyItem>>();
            _braintreePaymentsService = new BraintreePaymentsService();

            _accountHelper = new AccountControllerHelper<IRegisterModel>();
        }
        [Authorize]
        [HttpGet]
        public ActionResult UserProfile()
        {
            if (!_helper.UserHasAccess())
                return RedirectToHome();
            var model = _helper.GetUserUpdateModel();

            if (model == null) return RedirectToHome();
            var CountriesyList = new SelectList(_helper.getAllCountries(), "PhoneCode", "Name");
            ViewBag.Countries = CountriesyList;
            model.CountryCode = model.CountryCode.Trim();
            var shape = _shapeFactory.BuildShape(null, model, PageName.UserProfile.ToString());
            return DisplayShape(shape);
        }
        [Authorize]
        [HttpPost]
        public ActionResult UserProfile(Shape<UserUpdateModel> shape)
        {
            ////if(shape.ViewModel.UserImage != null)
            ////    shape.ViewModel.UserImage.ImgSettings = MomentarilyImageSettings.UserImageSizes;
            var requestUser = _helper.GetUser();
            var CountriesyList = new SelectList(_helper.getAllCountries(), "PhoneCode", "Name");
            ViewBag.Countries = CountriesyList;
            if (!string.IsNullOrWhiteSpace(shape.ViewModel.FirstName) &&
                !string.IsNullOrWhiteSpace(shape.ViewModel.LastName))
            {
                var viewShape = _helper.UserProfilePost(shape, ModelState);
                if (viewShape.IsError == false)
                {
                    bool checkExsistSubscriber = _accountHelper.ExsistSubscriberOnlyEmail(requestUser.Email);
                    if (shape.ViewModel.IgnoreMarketingEmails == false)
                    {
                        _accountHelper.subscribeEmail(SubscriberListId, requestUser.Email, requestUser.FirstName + " " + requestUser.LastName, true);
                        if (!checkExsistSubscriber)
                        {
                            bool saveSubscriber = _accountHelper.saveSubscriber(shape.ViewModel.Email);
                        }
                        else
                        {
                            _accountHelper.UpdateSubscriberUnsubscriber(requestUser.Email, shape.ViewModel.IgnoreMarketingEmails);
                        }
                    }
                    else
                    {
                        _accountHelper.unsubscribeEmail(SubscriberListId, requestUser.Email, "", true);
                        _accountHelper.UpdateSubscriberUnsubscriber(requestUser.Email,shape.ViewModel.IgnoreMarketingEmails);
                    }
                }
                return DisplayShape(viewShape);
            }
            shape.IsError = true;
            shape.Message = ViewErrorText.UserProfileNoChanged;
            return DisplayShape(shape);
        }
        [Authorize]
        [HttpGet]
        public ActionResult UserEmail(CreateResult? result = null)
        {
            if (!_helper.UserHasAccess())
                return RedirectToHome();
            UserEmailModel userEmailModel = _helper.GetUserEmailModel();
            var shape = _shapeFactory.BuildShape(null, userEmailModel, PageName.UserEmail.ToString());
            if (result == null) return DisplayShape(shape);
            switch (result)
            {
                case CreateResult.EmailChangeError:
                    shape.IsError = true;
                    shape.Message = ViewErrorText.UserEmailNoChanged;
                    break;
                case CreateResult.EmailChangeSuccess:
                    shape.IsError = false;
                    shape.Message = ViewErrorText.UserEmailChanged;
                    break;
            }
            return DisplayShape(shape);
        }
        [Authorize]
        [HttpPost]
        public ActionResult UserEmail(Shape<UserEmailModel> shape)
        {
            return !_helper.UserHasAccess() ? RedirectToHome() : DisplayShape(_helper.UserEmail(shape, ModelState));
        }
        [Authorize]
        [HttpGet]
        public ActionResult UserEmailConfirm()
        {
            var user = _helper.GetUser();
            if (user != null)
            {
                if(user.Verified && user.IsMobileVerified)
                {
                    return RedirectToAction("Index","Listing");
                }
                return View(user);
            }
            return RedirectToHome();
        }

        [Authorize]        [HttpGet]        public ActionResult BlockedUser()        {            var user = _helper.GetUser();            if (user != null)            {                return View(user);            }            return RedirectToHome();        }
        [Authorize]
        [HttpPost]
        public ActionResult UserEmailConfirmResend()
        {
            var user = _helper.GetUser();
            if (user != null)
            {
                var result = _helper.ResendActivation(UserId.Value, QuickUrl, ControllerContext.HttpContext.Request);
                if (result != null)
                {
                    if (result.IsMobileVerified == false)
                    {
                        TwilioSmsSendProviderTest test = new TwilioSmsSendProviderTest();
                        string OTP = test.GenerateOTP();
                        var code = _userService.GetCountryCodeByPhoneNumber(result.PhoneNumber);                        bool sent = test.SendOTP(OTP, result.PhoneNumber, result.VC, code);
                        AccountController.OTP = OTP;
                       return RedirectToAction("OTPMessageSent", "Account", result);
                    }
                    else
                    {
                        TempData["confirmemailsent"]="sent";
                        return RedirectToHome();
                    }
                }
            }
            return RedirectToHome();
        }
        public ActionResult UserPwd()
        {

            if (!_helper.UserHasAccess())
                return RedirectToHome();
            var shape = _shapeFactory.BuildShape(null, new UserPwdModel(), PageName.UserPwd.ToString());
            return DisplayShape(shape);
        }
        [Authorize]
        [HttpPost]
        public ActionResult UserPwd(Shape<UserPwdModel> shape)
        {
            if (!_helper.UserHasAccess())
            {
                return RedirectToHome();
            }
          else
            {
                _helper.LogOff();
                Response.Cookies.Remove("Login");
                DisplayShape(_helper.UserPwd(shape, ModelState));
                TempData["isPasswordChangeSuccessfull"] = "True";
                return Redirect(QuickUrl.HomeUrl());
            }

           
        }

        public ActionResult LogOff()
        {
            _helper.LogOff();
            Response.Cookies.Remove("Login");
            return Redirect(QuickUrl.HomeUrl());
        }

        public ActionResult ResetPassword()
        {
            TempData["NoAccessFooter"] = "NoAccess";
            if (!_helper.UserHasAccess())
                return RedirectToHome();
            var shape = _shapeFactory.BuildShape(null, new UserPwdModel(), PageName.UserPwd.ToString());
            return DisplayShape(shape);
        }
        [Authorize]
        [HttpPost]
        public ActionResult ResetPassword(Shape<UserPwdModel> shape)
        {
            TempData["NoAccessFooter"] = "NoAccess";
            return !_helper.UserHasAccess()
                ? RedirectToHome()
                : DisplayShape2(_helper.UserPwd(shape, ModelState));
        }
        [HttpGet]
        public ActionResult VerifyUser(string vc)
        {
            var user = _helper.VerifyUser(vc);
            return user != null
                ? AuthenticateUserWithPrivilagesAndRedirect(user.Id, QuickUrl.UserProfileUrl())
                : RedirectToHome();
        }
        [HttpGet]
        public ActionResult WelcomeUser(string vc)
        {
            TempData["NoAccessFooter"] = "NoAccess";
            var user = _helper.VerifyUser(vc);
            if (user != null)
            {
                AuthenticateUserconfirm(user.Id);
                return View(user);
            }
            return RedirectToHome();
        }
        [HttpPost]
        public ActionResult WelcomeUser(User user)
        {
            TempData["NoAccessFooter"] = "NoAccess";
            return user != null
               ? AuthenticateUserWithPrivilagesAndRedirect(user.Id, QuickUrl.CreateListUrl())
               : RedirectToHome();
        }
        [HttpGet]
        public bool VerifyMobileNumber(string otp, string vc)
        {
            bool result = false;
            result = _helper.VerifyMobile(otp, vc);
            return result;
        }

        [HttpGet]
        public ActionResult VerifyMobileLink(string vc)
        {
            bool result = false;
            result = _helper.VerifyMobileLink(vc);
            return RedirectToAction("RedirectUser", new { vcode = vc });
        }

        [HttpGet]
        public ActionResult RedirectUser(string vcode)
        {
            var user = _helper.GetUserByVC(vcode);
            if (user != null)
            {
                return RedirectToAction("Login", "Account");
            }
            return RedirectToHome();
        }

        [HttpGet]
        public ActionResult VerifySecurityRequest(string vc)
        {
            if (string.IsNullOrWhiteSpace(vc))
                return RedirectToHome();
            var verifyResult = _helper.VerifySecurityRequest(vc);
            if (verifyResult.Success)
            {
                string redirectTo = null;
                if (verifyResult.RedirectTo == PageName.UserEmail.ToString())
                    redirectTo = QuickUrl.UserEmailUrl(new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>(QS.result, CreateResult.EmailChangeSuccess.ToString()) });
                if (ContextService.IsUserAuthenticated)
                {
                    if (verifyResult.Request.DataType == UserSecurityDataType.Email)
                    {
                        UserAccess.UpdateUser(verifyResult.Request.NewValue);
                        if (string.IsNullOrEmpty(verifyResult.Request.OldValue))
                            return RedirectToLocal(QuickUrl.UserInfoUrl());
                    }
                    return RedirectToLocal(redirectTo);
                }
                return AuthenticateUserWithPrivilagesAndRedirect(verifyResult.UserId, redirectTo);
            }
            return Redirect(QuickUrl.CreateResultUrl(CreateResult.VerifySecurityRequestError));
        }
        [Authorize]        [HttpGet]        public ActionResult BankInfo()        {            if (!_helper.UserHasAccess()) return RedirectToHome();            int _userid = 0;            _userid = UserId.Value;
            //var getRecipientResult = _braintreePaymentsService.GetRecipientByUserId(UserId.Value);
            var getcurrentuserPaypalInfo = _helper.GetCurrentPaypalInfoPaymentDetail(_userid);            PayoutDetailsModel model = new PayoutDetailsModel            {                UserId = UserId.Value            };            if (getcurrentuserPaypalInfo.CreateResult == CreateResult.Success)            {                var recipient = getcurrentuserPaypalInfo.Obj;                model.AccountNumber = recipient.AccountNumber;                model.RoutingNumber = recipient.RoutingNumber;                model.Locality = recipient.Locality;                model.PostalCode = recipient.PostalCode;                model.Region = recipient.Region;                model.StreetAddress = recipient.StreetAddress;                model.PaypalBusinessEmail = recipient.PaypalBusinessEmail;                model.PaymentType = recipient.PaymentType;                model.PhoneNumber = recipient.PhoneNumber;                var PaymentType = new SelectList(_helper.getPaymentTypes(), "Id", "GlobalCodeName", recipient.PaymentType);                ViewBag.PaymentType = PaymentType;            }            else            {                var PaymentTypeList = new SelectList(_helper.getPaymentTypes(), "Id", "GlobalCodeName");                ViewBag.PaymentType = PaymentTypeList;            }








            #region Old Code            //if (getRecipientResult.CreateResult == CreateResult.Success)
                                         //{
                                         //    var recipient = getRecipientResult.Obj;
                                         //    model.AccountNumber = recipient.Funding.AccountNumber;
                                         //    model.RoutingNumber = recipient.Funding.RoutingNumber;
                                         //    model.Locality = recipient.Individual.Locality;
                                         //    model.PostalCode = recipient.Individual.PostalCode;
                                         //    model.Region = recipient.Individual.Region;
                                         //    model.StreetAddress = recipient.Individual.StreetAddress;
                                         //}
            #endregion
            var shape = _shapeFactory.BuildShape(null, model, PageName.UserPwd.ToString());            shape.IsError = getcurrentuserPaypalInfo.CreateResult == CreateResult.Error;            shape.Message = getcurrentuserPaypalInfo.Message;            return DisplayShape(shape);        }        [Authorize]        [HttpPost]        public ActionResult BankInfo(Shape<PayoutDetailsModel> shape)        {            if (!_helper.UserHasAccess()) return RedirectToHome();            if (shape.ViewModel.PaymentType == (int)GlobalCode.Phone)            {                shape.ViewModel.PaypalBusinessEmail = "";                ModelState.Remove("ViewModel.PaypalBusinessEmail");            }            else            {                shape.ViewModel.PhoneNumber = "";                ModelState.Remove("ViewModel.PhoneNumber");            }            ModelState.Remove("ViewModel.Locality");            ModelState.Remove("ViewModel.PostalCode");            ModelState.Remove("ViewModel.Region");            ModelState.Remove("ViewModel.StreetAddress");            if (ModelState.IsValid && shape.ViewModel.UserId == UserId.Value)            {                var user = _helper.GetUser();                var savePaypalInfo = _helper.CreatePaypalInfoPaymentDetail(shape);                if (savePaypalInfo.CreateResult == CreateResult.Success && savePaypalInfo.Message == "Created")                {                    shape.IsError = false;                    shape.Message = "Your information has been saved successfully";                }                else if (savePaypalInfo.CreateResult == CreateResult.Success && savePaypalInfo.Message == "Updated")                {                    shape.IsError = false;                    shape.Message = "Your information has been updated successfully";                }                else                {                    shape.IsError = true;                    shape.Message = savePaypalInfo.Message;                }










                #region Old Implemented Code                //var createRecipientResult = _braintreePaymentsService.CreateOrUpdateRecipient(
                                                             //    UserId: user.Id,
                                                             //    Individual: new MerchantAccountIndividual
                                                             //    {
                                                             //        FirstName = user.FirstName,
                                                             //        LastName = user.LastName,
                                                             //        Email = user.Email,
                                                             //        DateOfBirth = user.DateOfBirth.HasValue ? user.DateOfBirth.Value.ToShortDateString() : "",
                                                             //        Locality = shape.ViewModel.Locality,
                                                             //        PostalCode = shape.ViewModel.PostalCode,
                                                             //        Region = shape.ViewModel.Region,
                                                             //        StreetAddress = shape.ViewModel.StreetAddress
                                                             //    },
                                                             //    Funding: new MerchantAccountFunding
                                                             //    {
                                                             //        AccountNumber = shape.ViewModel.AccountNumber,
                                                             //        RoutingNumber = shape.ViewModel.RoutingNumber
                                                             //    }
                                                             //);
                                                             //if (createRecipientResult.CreateResult == CreateResult.Success)
                                                             //{
                                                             //    shape.IsError = false;
                                                             //    shape.Message = "Your information has been saved successfully";
                                                             //}
                                                             //else
                                                             //{
                                                             //    shape.IsError = true;
                                                             //    shape.Message = createRecipientResult.Message;
                                                             //}
                #endregion
            }
            //int selectPaymentTpe = 0;
            //int.TryParse(shape.ViewModel.PaymentType, out selectPaymentTpe);
            var PaymentTypeListNew = new SelectList(_helper.getPaymentTypes(), "Id", "GlobalCodeName", shape.ViewModel.PaymentType);            ViewBag.PaymentType = PaymentTypeListNew;            return DisplayShape(shape);        }
        [Authorize]
        public ActionResult UserPublicProfile(int id)
        {
            if (!_helper.UserHasAccess())
                return RedirectToHome();
            var result = _userService.GetPublicUserProfile(id);
            if (result.CreateResult == CreateResult.Success)
            {
                var shape = _shapeFactory.BuildShape(null, result.Obj, PageName.UserPwd.ToString());
                return DisplayShape(shape);
            }
            return RedirectToHome();
        }

        [HttpGet]        public bool DeleteAccount()        {            bool result = false;            result = _helper.DeleteAccount();            return result;        }        [HttpGet]        public bool DeleteUserAccount(int userId)        {            bool result = false;            if (UserId > 0)            {                result = _helper.DeleteUserAccount(userId);            }
            return result;        }
    }
}