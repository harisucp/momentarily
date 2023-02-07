using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Apeek.Web.Framework
{
    public class CustomViewEngine : RazorViewEngine
    {
        private const string name = "Doodja";
        public CustomViewEngine():base() {


            AreaViewLocationFormats = new[] { 
                "~/Areas/%1/{2}/Views/{1}/{0}.cshtml",
                "~/Areas/%1/{2}/Views/Shared/{0}.cshtml",
                "~/Areas/{2}/Views/{1}/{0}.cshtml",
                "~/Areas/{2}/Views/Shared/{0}.cshtml",
            };



            ViewLocationFormats = new[] {
            "~/Areas/%1/{2}/Views/{1}/{0}.cshtml",
            "~/Areas/%1/{2}/Views/Shared/{0}.cshtml",
            "~/Areas/{2}/Views/{1}/{0}.cshtml",
                "~/Areas/{2}/Views/Shared/{0}.cshtml",
        };

            MasterLocationFormats = new[] {
            "~/Areas/%1/{2}/Views/{1}/{0}.cshtml",
            "~/Areas/%1/{2}/Views/Shared/{0}.cshtml", 
            "~/Areas/{2}/Views/{1}/{0}.cshtml",
                "~/Areas/{2}/Views/Shared/{0}.cshtml",
        };

            PartialViewLocationFormats = new[] {
            "~/Areas/%1/{2}/Views/{1}/{0}.cshtml",
            "~/Areas/%1/{2}/Views/Shared/{0}.cshtml",
            "~/Areas/{2}/Views/{1}/{0}.cshtml",
                "~/Areas/{2}/Views/Shared/{0}.cshtml",
        };
        }
         
        protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
        {
            if (partialPath.Contains("%1"))
            {
                return base.CreatePartialView(controllerContext, partialPath.Replace("%1", name));
            }
            return base.CreatePartialView(controllerContext, partialPath);
        }

        protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
        {
            if (viewPath.Contains("%1"))
            {
                return base.CreateView(controllerContext, viewPath.Replace("%1", name), masterPath.Replace("%1", name));
            }
            return base.CreateView(controllerContext, viewPath, masterPath);
        }

        protected override bool FileExists(ControllerContext controllerContext, string virtualPath)
        {
            if (virtualPath.Contains("%1"))
            {
                return base.FileExists(controllerContext, virtualPath.Replace("%1", name));
            }
            return base.FileExists(controllerContext, virtualPath);
        }
    }
}