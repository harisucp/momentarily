using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Apeek.Common;
using Apeek.Common.Interfaces;
using Apeek.Common.Validation;
using Apeek.Core;
using Apeek.Web.API;
using Momentarily.Common.Definitions;
using Momentarily.Web.Models;

namespace Momentarily.Web
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            Ioc.Add(a => a.For<IImageSettings>().Use<MomentarilyImageSettings>());
            ApeekController.DoPreStartActions();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteRegistration.RegisterAllRoutes();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
            ModelBinders.Binders.Add(typeof(double), new DoubleModelBinder());
            ModelBinders.Binders.Add(typeof(DateTime), new AppekDateTimeModelBinder());
            JobScheduler.Start();
        }
        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            var userAccessController =  Ioc.Get<IUserAccessController>();
            userAccessController.PostAuthRequest();
        }
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new OutputCacheAttribute
            {
                VaryByParam = "*",
                Duration = 0,
                NoStore = true,
            });
            // the rest of your global filters here
        }
    }
}
