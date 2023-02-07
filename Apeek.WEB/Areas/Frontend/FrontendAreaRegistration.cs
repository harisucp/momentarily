using System.Web.Mvc;
namespace Apeek.Web.Areas.Frontend
{
    public class FrontendAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Frontend";
            }
        }
        public override void RegisterArea(AreaRegistrationContext context) 
        {
            //context.Routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            context.MapRoute(
                "Frontend_default",
                "{controller}/{action}/{id}",
                new {controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}