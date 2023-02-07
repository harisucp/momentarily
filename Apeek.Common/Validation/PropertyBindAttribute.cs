﻿using System;
using System.ComponentModel;
using System.Web.Mvc;
namespace Apeek.Common.Validation
{
    [AttributeUsage(AttributeTargets.Property)]
    public abstract class PropertyBindAttribute : Attribute
    {
        public abstract bool BindProperty(ControllerContext controllerContext,
            ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor);
    }
}
