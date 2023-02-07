using Momentarily.Web.Areas.Frontend;
using System.Web.Mvc;
using System.Web.Routing;
using Apeek.Core.Web.WebRouting;
namespace Momentarily.Web
{
    public class RouteRegistration
    {
        public static void RegisterAllRoutes()
        {
            RegisterArea(new FrontendAreaRegistration());
        }
        private static void RegisterArea(AreaRegistration areaRegistration)
        {
            var adminAreaContext = new AreaRegistrationContext(areaRegistration.AreaName, RouteTable.Routes);
            areaRegistration.RegisterArea(adminAreaContext);
        }
    }
}