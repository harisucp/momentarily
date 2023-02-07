using System.Net.Http;
using System.Web.Mvc;
using System.Web.Routing;
namespace Apeek.Common.UrlHelpers
{
    public class MvcUrlHelper : IUrlHelper
    {
        private UrlHelper _urlHelper;
        public MvcUrlHelper(UrlHelper urlHelper)
        {
            this._urlHelper = urlHelper;
        }
        public string Action(string index, string controller, object routeValues)
        {
            return _urlHelper.Action(index, controller, routeValues);
        }
        public string HttpRouteUrl(string defaultapi, RouteValueDictionary routeApi)
        {
            return _urlHelper.HttpRouteUrl(defaultapi, routeApi);
        }
        public string RouteUrl(RouteValueDictionary routeLocations)
        {
            return _urlHelper.RouteUrl(routeLocations);
        }
        public string RouteUrl(object routeLocations)
        {
            return _urlHelper.RouteUrl(routeLocations);
        }
        public string Action(string action, string controller, object routeValues, string protocol)
        {
            return _urlHelper.Action(action, controller, routeValues, protocol);
        }
        public string Action(string action, string controller)
        {
            return _urlHelper.Action(action, controller);
        }
        public HttpRequestMessage Request { get; set; }
        public RequestContext RequestContext { get { return _urlHelper.RequestContext; } }
    }
}