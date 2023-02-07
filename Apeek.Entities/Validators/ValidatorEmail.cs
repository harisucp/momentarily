using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
namespace Apeek.Entities.Validators
{
    public class ValidatorEmail : ValidatorBase
    {
        public override bool IsValid(object value)
        {
            var email = value as string;
            if (string.IsNullOrWhiteSpace(email))
            {
                ErrorMessage = "Please, enter email";
                return false;
            }
            else
            {
                var emailAddressValidator = new EmailAddressAttribute();
                if (emailAddressValidator.IsValid(value))
                    return true;
                ErrorMessage = "Please, enter valid email";
                return false;
            }
        }
    }
}