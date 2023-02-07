using System.ComponentModel;
using System.Web.Mvc;
namespace Apeek.Common.Validation
{
    public class StringPropertyBindAttribute : PropertyBindAttribute
    {
        public override bool BindProperty(ControllerContext controllerContext,
            ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor)
        {
            if (propertyDescriptor.PropertyType == typeof(string))
            {
                var value = bindingContext.ValueProvider.GetValue(propertyDescriptor.Name);
                if (!string.IsNullOrWhiteSpace(value.AttemptedValue))
                {
                    propertyDescriptor.SetValue(bindingContext.Model,
                        value.AttemptedValue.Replace(@"\", @"\\"));
                }
                return true;
            }
            return false;
        }
    }
}
