using System;
using System.Net.Http;
using System.Web.Routing;
namespace Apeek.Common.UrlHelpers
{
    public class HttpUrlHelper : IUrlHelper
    {
        private System.Web.Http.Routing.UrlHelper _urlHelper;
        public HttpUrlHelper(System.Web.Http.Routing.UrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
        }
        public string Action(string index, string controller, object routeValues)
        {
            throw new NotImplementedException();
        }
        public string HttpRouteUrl(string defaultapi, RouteValueDictionary routeApi)
        {
            return _urlHelper.Route(defaultapi, routeApi);
        }
        public string RouteUrl(RouteValueDictionary routeLocations)
        {
            return _urlHelper.Route("Default", routeLocations);
        }
        public string RouteUrl(object routeLocations)
        {
            throw new NotImplementedException();
        }
        public string Action(string action, string controller, object routeValues, string protocol)
        {
            throw new NotImplementedException();
        }
        public string Action(string action, string controller)
        {
            throw new NotImplementedException();
        }
        public HttpRequestMessage Request { get { return _urlHelper.Request; } }
        public RequestContext RequestContext { get; private set; }
    }
}