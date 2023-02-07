using System.ComponentModel.DataAnnotations;
namespace Apeek.Entities.Validators
{
    public class ValidatorEmailOrPhoneNum : ValidatorBase
    {
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public override bool IsValid(object value)
        {
            var emailOrPhoneNum = value as string;
            if (string.IsNullOrWhiteSpace(emailOrPhoneNum))
            {
                ErrorMessage = "Please, enter email or phone number";
                return false;
            }
            else
            {
                var emailAddressValidator = new EmailAddressAttribute();
                if (emailAddressValidator.IsValid(value))
                {
                    EmailAddress = emailOrPhoneNum;
                    return true;
                }
                var phoneNumberValidator = new ValidatorPhoneNumber();
                if (phoneNumberValidator.IsValid(value))
                {
                    PhoneNumber = phoneNumberValidator.PhoneNumber;
                    return true;
                }
                ErrorMessage = "Please, enter valid email or phone number";
            }
            return false;
        }
    }
}