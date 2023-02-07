using System.Collections.Generic;
using System.Web.Mvc;
using Apeek.Common;
using Apeek.Common.Controllers;
using Apeek.ViewModels.Models;
using Apeek.ViewModels.Models.Impl;
using Apeek.Web.Framework.ControllerHelpers;
using Apeek.Web.Framework.Controllers;
namespace Apeek.Web.Areas.Frontend.Controllers
{
    public class UserController: FrontendController
    {
        private readonly UserControllerHelper _helper;
        public UserController()
        {
            _helper = new UserControllerHelper();
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
            return !_helper.UserHasAccess() 
                ? RedirectToHome() 
                : DisplayShape(_helper.UserPwd(shape, ModelState));
        }
        [Authorize]
        [HttpGet]
        public ActionResult UserProfile()
        {
            if (!_helper.UserHasAccess())
                return RedirectToHome();
            var model = _helper.GetUserUpdateModel();
            if (model == null) return RedirectToHome();
            var shape = _shapeFactory.BuildShape(null, model, PageName.UserProfile.ToString());
            return DisplayShape(shape);
        }
        [Authorize]
        [HttpPost]
        public ActionResult UserProfilePost(Shape<UserUpdateModel> shape)
        {
            return !_helper.UserHasAccess() 
                ? RedirectToHome() 
                : DisplayShape(_helper.UserProfilePost(shape, ModelState));
        }
        [HttpGet]
        public ActionResult VerifyUser(string vc)
        {
            var user = _helper.VerifyUser(vc);
            return user != null 
                ? AuthenticateUserWithPrivilagesAndRedirect(user.Id, QuickUrl.UserProfileUrl()) 
                : RedirectToHome();
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
    }
}