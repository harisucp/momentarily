using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Apeek.Common;
using Apeek.Common.Controllers;
using Apeek.Common.UrlHelpers;
namespace Apeek.Web.Areas.Frontend.Controllers
{
    public class BaseController : Controller
    {
        protected readonly IUrlGenerator _urlGenerator;
        private QuickUrl _quickUrl;
        protected QuickUrl QuickUrl { get { return _quickUrl; } }
        public BaseController()
        {
            _urlGenerator = Ioc.Get<IUrlGenerator>();
        }
        protected override void OnActionExecuting(ActionExecutingContext httpContext)
        {
            LanguageController.UpdateCurLang();
            _quickUrl = new QuickUrl(new MvcUrlHelper(Url), _urlGenerator);
        }
        #region Http404 handling
        public ActionResult InvokeHttp404(HttpContextBase httpContext)
        {
            IController errorController = Ioc.Get<ErrorController>();
            var errorRoute = new RouteData();
            if (QuickUrl == null)
                _quickUrl = new QuickUrl(new MvcUrlHelper(Url), _urlGenerator);
            foreach (var v in QuickUrl.RouteNotFound(httpContext.Request.Url.OriginalString))
            {
                errorRoute.Values.Add(v.Key, v.Value);
            }
            errorController.Execute(new RequestContext(httpContext, errorRoute));
            return new EmptyResult();
        }
        protected override void HandleUnknownAction(string actionName)
        {
            if (this.GetType() != typeof(ErrorController))
                InvokeHttp404(HttpContext);
        }
        #endregion
    }
}