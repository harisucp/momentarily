using System;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Apeek.Common;
using Apeek.Web.Areas.Frontend.Controllers;

namespace Apeek.Web.DependencyResolution
{
    public class StructureMapControllerFactory : DefaultControllerFactory
    {
        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            try
            {
                if (controllerType == null)
                    return base.GetControllerInstance(requestContext, controllerType);
            }
            catch (HttpException ex)
            {
                if (ex.GetHttpCode() == (int)HttpStatusCode.NotFound)
                {
                    IController errorController = Ioc.Get<ErrorController>();
                    ((ErrorController)errorController).InvokeHttp404(requestContext.HttpContext);

                    return errorController;
                }
                else
                    throw ex;
            }

            return Ioc.Get(controllerType) as Controller;
        }
    }
}