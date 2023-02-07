using System.Web;
using System.Web.Routing;
using Apeek.Common;
namespace Apeek.Core.Web.WebRouting.Infrastructure
{
    public class RouteSubDomain : Route
    {
        public RouteSubDomain(string url, IRouteHandler routeHandler) : base(url, routeHandler) {}
        public RouteSubDomain(string url, RouteValueDictionary routeHandler, RouteValueDictionary constraints,
            RouteValueDictionary routeValueDictionary, IRouteHandler routeHandler1)
            : base(url, routeHandler, constraints, routeValueDictionary, routeHandler1) {}
        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            RouteData routeData = null;
            var url = httpContext.Request.Headers["HOST"];
            var currentDomain = string.Format(".{0}", ApeekController.GetCurrentDnsWithPort(httpContext.Request.Url));
            if (url.ToLower().Contains(currentDomain))
            {
                var subdomain = url.Replace(currentDomain, string.Empty);
                routeData = base.GetRouteData(httpContext);
                if (routeData != null)
                {
                    routeData.Values[QS.subDomain] = subdomain;
                }
            }
            return routeData;
        }
        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            var subDomain = string.Empty;
            if (values.ContainsKey(QS.subDomain))
            {
                subDomain = values[QS.subDomain].ToString();
                values.Remove(QS.subDomain);
            }
            VirtualPathData vpd = null;
            vpd = base.GetVirtualPath(requestContext, values);
            if (!string.IsNullOrWhiteSpace(subDomain))
            {
                if (vpd != null)
                {
                    vpd.VirtualPath = string.Format("{0}.{1}/{2}", subDomain, ApeekController.GetCurrentDnsWithPort(requestContext.HttpContext.Request.Url), vpd.VirtualPath);
                }
                else
                {
                    //adding back subdomain value to have it in another route
                    values.Add(QS.subDomain, subDomain);
                }
            }
            return vpd;
        }
    }
}