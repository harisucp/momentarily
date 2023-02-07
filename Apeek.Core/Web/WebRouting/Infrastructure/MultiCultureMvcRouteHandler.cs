using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Apeek.Common;
using Apeek.Common.Controllers;
namespace Apeek.Core.Web.WebRouting.Infrastructure
{
    public class MultiCultureMvcRouteHandler : MvcRouteHandler
    {
        protected override IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            var value = requestContext.RouteData.Values[QS.lang] == null
                ? LanguageController.DefLanguage.ToString()
                : requestContext.RouteData.Values[QS.lang].ToString();
            if (value == UrlParameter.Optional.ToString())
                value = LanguageController.DefLanguage.ToString();
            var ci = new CultureInfo(value);
            Thread.CurrentThread.CurrentUICulture = ci;
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(ci.Name);
            return base.GetHttpHandler(requestContext);
        }
    }
}