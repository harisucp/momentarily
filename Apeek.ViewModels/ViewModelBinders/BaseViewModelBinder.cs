using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
namespace Apeek.ViewModels.ViewModelBinders
{
    public abstract class BaseViewModelBinder : IModelBinder
    {
        protected bool ParseBool(NameValueCollection form, string keyShowInMenu)
        {
            if (form.AllKeys.Contains(keyShowInMenu))
            {
                switch (form[keyShowInMenu])
                {
                    case "true,false":
                        return true;
                    case "false":
                        return false;
                    default:
                        throw new InvalidEnumArgumentException("LangViewModelBinder.ParseBool: cannot parse bool values");
                }
            }
            return false;
        }
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            return BindModelExact(controllerContext, bindingContext);
        }
        protected abstract object BindModelExact(ControllerContext controllerContext, ModelBindingContext bindingContext);
    }
}