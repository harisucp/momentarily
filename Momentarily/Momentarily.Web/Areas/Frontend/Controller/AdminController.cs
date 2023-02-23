using Apeek.Common;  using Apeek.Common.Controllers;using Apeek.Entities.Web;using Apeek.ViewModels.Models;using Apeek.Web.Framework.ControllerHelpers;using Apeek.Web.Framework.Controllers;using Momentarily.ViewModels.Models;using System;using System.Collections.Generic;using System.Linq;using System.Web;using System.Web.Mvc;namespace Momentarily.Web.Areas.Frontend.Controller{    public class AdminController : FrontendController    {
        // GET: Frontend/Admin
        private readonly AccountControllerHelper<MomentarilyRegisterModel> _helper;        public AdminController()        {            _helper = new AccountControllerHelper<MomentarilyRegisterModel>();        }        [AllowAnonymous]        public ActionResult Index()        {
            TempData["NoAccessFooter"] = "NoAccess";
            //if (Convert.ToBoolean(Session["IsAdmin"]) == false)
            //{
            _helper.LogOff();
            //Session["IsAdmin"] = true;
            Response.Cookies.Remove("AdminLogin");
            //    return RedirectToAction("Index");
            //}
            //if (ContextService.IsUserAuthenticated)
            //    return Redirect(QuickUrl.UserProfileUrl());
            //ViewBag.RedirectTo = ReturnUrl;
            var shape = _shapeFactory.BuildShape(null, new LoginModel(), PageName.Admin.ToString());
            if (Request.Cookies["AdminLogin"] != null)
            {
                shape.ViewModel.RememberMe = true;
            }

            //if (!_helper.ExternalLoginError) return DisplayShape(shape);
            //shape.IsError = _helper.ExternalLoginError;
            // shape.Message = "Failure to login using external system.";
            Session["IsAdmin"] = true;            Session["IsAutherised"] = false;            return DisplayShape(shape);        }        [HttpPost]        [AllowAnonymous]        public ActionResult Index(Shape<LoginModel> shape)        {
            //var lists=_helper.getCampaignList("hello@momentarily.com");
            TempData["NoAccessFooter"] = "NoAccess";
            if (ContextService.IsUserAuthenticated)                return Redirect(QuickUrl.UserProfileUrl());            var loginResult = _helper.AdminLogin(shape, ReturnUrl, ModelState);            if (loginResult != null && loginResult.LoginStatus != LoginStatus.Fail)            {                if (shape.ViewModel.RememberMe)                {                    HttpCookie cookie = new HttpCookie("AdminLogin");                    cookie.Values.Add("AdminEmailID", shape.ViewModel.EmailAddressOrPhoneNum);                    cookie.Values.Add("AdminPassword", shape.ViewModel.Password);                    cookie.Expires = DateTime.Now.AddDays(15);                    Response.Cookies.Add(cookie);                }                else
                {
                    shape.ViewModel.RememberMe = false;
                    Response.Cookies["AdminLogin"].Expires = DateTime.Now.AddDays(-1);
                }                var redirectTo = ReturnUrl;                if (loginResult.LoginStatus != LoginStatus.SuccessWithTempPwd)                {                    Session["IsAdmin"] = true;                    Session["IsAutherised"] = true;                    return AuthenticateAdminWithPrivilagesAndRedirect(loginResult.User.Id, redirectTo);                }

                //redirectTo = QuickUrl.UserPwdUrl();
                //if (string.IsNullOrWhiteSpace(loginResult.User.Email))
                //    redirectTo = QuickUrl.UserEmailUrl(new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>(QS.source, "tmp_pwd") });
                //return AuthenticateUserWithPrivilagesAndRedirect(loginResult.User.Id, redirectTo);
            }            Session["IsAdmin"] = true;            Session["IsAutherised"] = false;            ViewBag.RedirectTo = ReturnUrl;            shape.IsError = true;            shape.Message = "Email and password do not match.";            return View(shape);        }        [AllowAnonymous]        public ActionResult LogOut()        {            _helper.LogOff();            Session["IsAdmin"] = false;            Session["IsAutherised"] = false;            Response.Cookies.Remove("AdminLogin");            return RedirectToAction("Index");        }
    }}