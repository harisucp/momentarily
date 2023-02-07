using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Apeek.Entities.Interfaces;
using Apeek.ViewModels.BaseViewModel;
namespace Apeek.Core.Validators
{
    public class LangViewModelValidator<TEntity, TLang> : ViewModelValidatorBase
        where TEntity : IEntity, new()
        where TLang : IEntityLang, new()
    {
        private readonly LangViewModel<TEntity, TLang> _langViewModel;
        public LangViewModelValidator(LangViewModel<TEntity, TLang> langViewModel, ModelStateDictionary modelState)
            : base(modelState)
        {
            _langViewModel = langViewModel;
        }
        public override void Validate()
        {
            foreach (var cityLang in _langViewModel.EntityLang)
            {
                var context = new ValidationContext(cityLang.Value);
                var results = new List<ValidationResult>();
                var isvalid = Validator.TryValidateObject(cityLang.Value, context, results, true);
                if (!isvalid)
                {
                    foreach (var result in results)
                    {
                        AddError("Property", result.ErrorMessage);
                    }
                }
            }
        }
    }
}