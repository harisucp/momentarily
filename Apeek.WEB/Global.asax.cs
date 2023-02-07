using System;
using System.Web.Http;
using System.Web.Mvc;
using Apeek.Common;
using Apeek.Common.Interfaces;
using Apeek.Common.Validation;
using Apeek.Core;
using Apeek.Web.API;
namespace Apeek.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            ApeekController.DoPreStartActions();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteRegistration.RegisterAllRoutes();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            ModelBinders.Binders.Add(typeof(double), new DoubleModelBinder());
            AuthConfig.RegisterAuth();
        }
        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            var userAccessController = Ioc.Get<IUserAccessController>();
            userAccessController.PostAuthRequest();
        }
    }
}
