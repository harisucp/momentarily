using System.ComponentModel.DataAnnotations;
namespace Apeek.Entities.Validators
{
    public class ValidatorCity : ValidatorBase
    {
        public override bool IsValid(object value)
        {
            var city = value as string;
            if (string.IsNullOrWhiteSpace(city))
            {
                ErrorMessage = ValidationErrors.SelectCity;
                return false;
            }
            return true;
        }
    }
}