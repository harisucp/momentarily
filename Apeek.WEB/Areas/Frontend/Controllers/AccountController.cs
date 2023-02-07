using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Apeek.Common;
using Apeek.Common.Controllers;
using Apeek.Entities.Web;
using Apeek.ViewModels.Models;
using Apeek.Web.Framework.ControllerHelpers;
using Apeek.Web.Framework.Controllers;
namespace Apeek.Web.Areas.Frontend.Controllers
{
    [Authorize]
    public class AccountController : FrontendController
    {
        private readonly AccountControllerHelper<IRegisterModel> _helper;
        public AccountController()
        {
            _helper = new AccountControllerHelper<IRegisterModel>();
        }
        [AllowAnonymous]
        public ActionResult Register()
        {
            if (ContextService.IsUserAuthenticated)
                return Redirect(QuickUrl.UserInfoPreviewUrl());
            var model = Activator.CreateInstance<IRegisterModel>();
            var shape = _shapeFactory.BuildShape(null, model, PageName.Register.ToString());
            return DisplayShape(shape);
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Register(Shape<IRegisterModel> shape)
        {
            if (ContextService.IsUserAuthenticated)
                return Redirect(QuickUrl.UserProfileUrl());
            var user = _helper.Register(shape, QuickUrl, ModelState, ControllerContext.HttpContext.Request);
            if (user != null)
            {
                var redirectTo = QuickUrl.RegisterMessageSentUrl(user.Email);
                return AuthenticateUserWithPrivilagesAndRedirect(user.Id, redirectTo);
            }
            shape.IsError = true;
            shape.Message = "Error.";
            return View(shape);
        }
        [AllowAnonymous]
        public ActionResult RestorePwd()
        {
            var shape = _shapeFactory.BuildShape(null, new UserRestorePwd());
            return DisplayShape(shape);
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult RestorePwd(Shape<UserRestorePwd> shape)
        {
            shape = _helper.RestorePwd(shape, QuickUrl, ModelState);
            return DisplayShape(shape);
        }
        [AllowAnonymous]
        public ActionResult Login()
        {
            if (ContextService.IsUserAuthenticated)
                return Redirect(QuickUrl.UserProfileUrl());
            ViewBag.RedirectTo = ReturnUrl;
            var shape = _shapeFactory.BuildShape(null, new LoginModel(), PageName.Login.ToString());
            if (!_helper.ExternalLoginError) return DisplayShape(shape);
            shape.IsError = _helper.ExternalLoginError;
            shape.Message = "Failure to login using external system.";
            return DisplayShape(shape);
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(Shape<LoginModel> shape)
        {
            if (ContextService.IsUserAuthenticated)
                return Redirect(QuickUrl.UserProfileUrl());
            var loginResult = _helper.Login(shape, ReturnUrl, ModelState);
            if (loginResult != null && loginResult.LoginStatus != LoginStatus.Fail)
            {
                var redirectTo = ReturnUrl;
                if (loginResult.LoginStatus != LoginStatus.SuccessWithTempPwd)
                    return AuthenticateUserWithPrivilagesAndRedirect(loginResult.User.Id, redirectTo);
                redirectTo = QuickUrl.UserPwdUrl();
                if (string.IsNullOrWhiteSpace(loginResult.User.Email))
                    redirectTo = QuickUrl.UserEmailUrl(new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>(QS.source, "tmp_pwd") });
                return AuthenticateUserWithPrivilagesAndRedirect(loginResult.User.Id, redirectTo);
            }
            ViewBag.RedirectTo = ReturnUrl;
            shape.IsError = true;
            shape.Message = "Email and password do not match.";
            return View(shape);
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
        public ActionResult LogOff()
        {
            _helper.LogOff();
            return Redirect(QuickUrl.HomeUrl());
        }
        [Authorize]
        [HttpGet]
        public ActionResult RegisterMessageSent(string email)
        {
            MessageSentModel model = new MessageSentModel
            {
                Email = email
            };
            var shape = _shapeFactory.BuildShape(null, model, PageName.LoginMessageSent.ToString());
            return DisplayShape(shape);
        }
    }
}
