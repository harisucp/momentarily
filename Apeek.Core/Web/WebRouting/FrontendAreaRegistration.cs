using System.Web.Mvc;
using System.Web.Routing;
using Apeek.Core.Web.WebRouting.Infrastructure;
namespace Apeek.Core.Web.WebRouting
{
    public class FrontendAreaRegistration : AreaLangRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Frontend";
            }
        }
        public RouteCollection RegisterTable()
        {
            return RegisterRoutes(RouteTable.Routes);
        }
        protected override RouteCollection RegisterRoutes()
        {
            return RegisterRoutes(new RouteCollection());
        }
        protected RouteCollection RegisterRoutes(RouteCollection routesTemp)
        {
            //Ignoring routes in MVC will tell the MVC framework not to pick up those URLs.
            //This means that it will let the underlying ASP.NET handle the request, which will happily show you a static file.
            routesTemp.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routesTemp.IgnoreRoute("favicon.ico");
            //allow routing all existing files
            routesTemp.RouteExistingFiles = true;
            //redirect all requests to not existing files to error page
            routesTemp.MapRoute(
                name: "",
                url: "{*src}",
                defaults: new { Controller = "Error", action = "NotFound" },
                constraints: new { src = @"(.*?)\.(html|htm|aspx|asp|php)" } // URL constraints 
            );
            MapRoute(
                routeCollection: routesTemp,
                name: "",
                url: "{*sitemap}",
                defaults: new { controller = "Sitemap", action = "Index" },
                constraints: new { sitemap = @"sitemap(.*?)\.xml" },
                routeHandler: new SingleCultureMvcRouteHandler()
            );
            MapSubDomainRoute(
                routeCollection: routesTemp,
                name: "",
                url: "robots.txt",
                defaults: new { controller = "Robots", action = "Index" },
                routeHandler: new SingleCultureMvcRouteHandler()
            );
            MapRoute(
                routeCollection: routesTemp,
                name: "",
                url: "robots.txt",
                defaults: new { controller = "Robots", action = "Index" },
                routeHandler: new SingleCultureMvcRouteHandler()
            );
            MapRoute(
                routeCollection: routesTemp,
                name: "",
                url: "account/{action}",
                defaults: new { controller = "Account", action = "Login" },
                routeHandler: new SingleCultureMvcRouteHandler()
            );
            //MapRoute(
            //    routeCollection: routesTemp,
            //    name: "",
            //    url: "acc/{action}",
            //    defaults: new { controller = "User", action = "SendVerificationEmail" },
            //    routeHandler: new SingleCultureMvcRouteHandler()
            //);
            MapDomainRoute(
                routeCollection: routesTemp,
                name: "",
                url: "error/{action}",
                defaults: new { controller = "Error", action = "NotFound2" },
                routeHandler: new SingleCultureMvcRouteHandler()
            );
            MapRoute(
                routeCollection: routesTemp,
                name: "",
                url: "contactus",
                defaults: new { controller = "Home", action = "ContactUs" },
                routeHandler: new SingleCultureMvcRouteHandler()
            );
            MapRoute(
                routeCollection: routesTemp,
                name: "",
                url: "termsofuse",
                defaults: new { controller = "Home", action = "Terms" },
                routeHandler: new SingleCultureMvcRouteHandler()
            );
            MapRoute(
                routeCollection: routesTemp,
                name: "",
                url: "privacypolicy",
                defaults: new { controller = "Home", action = "PrivacyPolicy" },
                routeHandler: new SingleCultureMvcRouteHandler()
            );
            MapSubDomainRoute(
                routeCollection: routesTemp,
                name: "",
                url: "user/{userUrl}",
                defaults: new { controller = "Home", action = "User", userUrl = UrlParameter.Optional },
                routeHandler: new SingleCultureMvcRouteHandler()
            );
            //sub-domain location page
            MapSubDomainRoute(
                routeCollection: routesTemp,
                name: "",
                url: "",
                defaults: new { controller = "Home", action = "Location" },
                routeHandler: new SingleCultureMvcRouteHandler()
            );
          //  MapDomainRoute(
          //      routeCollection: routesTemp,
          //    name: "Frontend_default",
          //    url: "{controller}/{action}/{id}",
          //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
          //    routeHandler: new SingleCultureMvcRouteHandler()
          //);
            MapDomainRoute(
                routeCollection: routesTemp,
                name: "",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                routeHandler: new SingleCultureMvcRouteHandler()
            );
            return routesTemp;
        }
    }
}
