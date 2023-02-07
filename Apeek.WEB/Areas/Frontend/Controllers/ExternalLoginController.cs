using System.Web.Mvc;
using Apeek.Common.Controllers;
using Apeek.Web.Framework.Auth;
using Apeek.Web.Framework.ControllerHelpers;
using Apeek.Web.Framework.Controllers;
using Microsoft.Web.WebPages.OAuth;
namespace Apeek.Web.Areas.Frontend.Controllers
{
    public class ExternalLoginController : FrontendController
    {
        private readonly ExternalLoginControllerHelper _helper;
        public ExternalLoginController()
        {
            _helper = new ExternalLoginControllerHelper();
        }
        [AllowAnonymous]
        public ActionResult ExternalLogin(string provider, string redirectTo)
        {
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
            var result = _helper.VerifyAuthentication(redirectTo, Url.Action("ExternalLoginCallback", "ExternalLogin", new { redirectTo = redirectTo }));
            if (!result.IsSuccessful) return Redirect(QuickUrl.LoginUrl(redirectTo, true));
            var user = _helper.AuthenticateUser(result);
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
        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult ExternalLoginsList(string redirectTo)
        {
            ViewBag.RedirectTo = redirectTo;
            return PartialView("_ExternalLoginsListPartial", OAuthWebSecurity.RegisteredClientData);
        }
    }
}