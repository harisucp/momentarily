using System.Web.Mvc;
using System.Web.Routing;
namespace Apeek.Core.Web.WebRouting
{
    public class ErrorRouteRegistration
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute(
                name: "",
                url: "{*query}",
                defaults: new { controller = "Error", action = "NotFound" }
            );
        }
    }
}