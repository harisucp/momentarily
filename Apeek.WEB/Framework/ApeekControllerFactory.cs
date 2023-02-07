using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Apeek.Web.Framework
{
    public class ApeekControllerFactory : DefaultControllerFactory
    {

        protected override Type GetControllerType(System.Web.Routing.RequestContext requestContext, string controllerName)
        {

            var type = base.GetControllerType(requestContext, controllerName);
            if (type == null)
            {
                string[] splits = controllerName.Split('.');
                Type objectType = (
                   from t in this.GetType().Assembly.GetTypes()
                   where t.IsClass && t.Name == (controllerName + "Controller")
                   select t).Single();
                return objectType;
            }
            return type;

        }

    }
}