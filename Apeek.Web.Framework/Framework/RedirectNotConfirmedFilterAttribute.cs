using System.Web.Mvc;
using Apeek.Common;
using Apeek.Common.Controllers;
using Apeek.Common.HttpContextImpl;
using Apeek.Web.Framework.Controllers;
using Apeek.Common.UrlHelpers;
namespace Apeek.Web.Framework
{
    public class RedirectNotConfirmedFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (ContextService.IsUserAuthenticated)
            {
                if (!ContextService.AuthenticatedUser.IsUserConfirmed)
                {
                    var controller = filterContext.Controller as BaseController;
                    if (controller != null)
                    {
                        var urlGenerator = Ioc.Get<IUrlGenerator>();
                        var quickUrl = new QuickUrl(new MvcUrlHelper(controller.Url), urlGenerator);
                        filterContext.Result = new RedirectResult(quickUrl.VerifyEmail());
                    }
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }

    public class RedirectUserBlockedFilterAttribute : ActionFilterAttribute    {        public override void OnActionExecuting(ActionExecutingContext filterContext)        {            if (ContextService.IsUserAuthenticated)            {                if (ContextService.AuthenticatedUser.IsBlocked)                {                    var controller = filterContext.Controller as BaseController;                    if (controller != null)                    {                        var urlGenerator = Ioc.Get<IUrlGenerator>();                        var quickUrl = new QuickUrl(new MvcUrlHelper(controller.Url), urlGenerator);                        filterContext.Result = new RedirectResult(quickUrl.UserBlocked());                    }                }            }            base.OnActionExecuting(filterContext);        }    }


    public class RedirectNotIsAdminFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)

        {
            if (!ContextService.IsUserAuthenticated)
            {
                if (!ContextService.AuthenticatedUser.IsUserConfirmed && !ContextService.AuthenticatedUser.IsAdmin)
                {
                    var controller = filterContext.Controller as BaseController;
                    if (controller != null)
                    {
                        var urlGenerator = Ioc.Get<IUrlGenerator>();
                        var quickUrl = new QuickUrl(new MvcUrlHelper(controller.Url), urlGenerator);
                        filterContext.Result = new RedirectResult(quickUrl.VerifyIsAdmin());
                    }
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }
}