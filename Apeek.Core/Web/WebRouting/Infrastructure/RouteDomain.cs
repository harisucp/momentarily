using System.Text.RegularExpressions;
using System.Web.Routing;
namespace Apeek.Core.Web.WebRouting.Infrastructure
{
    public class RouteDomain : Route
    {
        public RouteDomain(string url, IRouteHandler routeHandler) : base(url, routeHandler) {}
        public RouteDomain(string url, RouteValueDictionary routeHandler, RouteValueDictionary constraints,
            RouteValueDictionary routeValueDictionary, IRouteHandler routeHandler1)
            : base(url, routeHandler, constraints, routeValueDictionary, routeHandler1) {}
        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            VirtualPathData vpd = null;
            vpd = base.GetVirtualPath(requestContext, values);
            if (vpd != null)
            {
                var dns = ApeekController.CurrentDns;
                if (dns != null)
                {
                    if (Regex.IsMatch(requestContext.HttpContext.Request.Url.Authority, string.Format("(?=.){0}$", dns.Name)))
                        vpd.VirtualPath = dns.Name;
                    else if (Regex.IsMatch(requestContext.HttpContext.Request.Url.Authority, string.Format("(?=.){0}:{1}$", dns.Name, requestContext.HttpContext.Request.Url.Port)))
                        vpd.VirtualPath = string.Format("{0}:{1}", dns.Name, requestContext.HttpContext.Request.Url.Port);
                }
            }
            return vpd;
        }
    }
}