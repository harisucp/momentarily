using System.Net.Http;
using System.Web.Routing;
namespace Apeek.Common.UrlHelpers
{
    public interface IUrlHelper
    {
        string Action(string index, string controller, object routeValues);
        string HttpRouteUrl(string defaultapi, RouteValueDictionary routeApi);
        string RouteUrl(RouteValueDictionary routeLocations);
        string RouteUrl(object routeLocations);
        string Action(string action, string controller, object routeValues, string protocol);
        string Action(string action, string controller);
        HttpRequestMessage Request { get; }
        RequestContext RequestContext { get; }
    }
}