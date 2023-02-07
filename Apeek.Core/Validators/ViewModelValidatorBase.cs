using System;
using System.Web.Mvc;
namespace Apeek.Core.Validators
{
    public class ViewModelValidatorBase
    {
        protected ModelStateDictionary ModelState { get; set; }
        public virtual void Validate()
        {
            throw new NotImplementedException();
        }
        protected ViewModelValidatorBase(ModelStateDictionary modelState)
        {
            ModelState = modelState;
        }
        public bool IsValid
        {
            get
            {
                Validate();
                return ModelState.IsValid;
            }
        }
        protected void AddError(string propertyName, string error)
        {
            ModelState.AddModelError(propertyName, error);
        }
    }
}