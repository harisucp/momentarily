using Apeek.Entities.Validators;
namespace Apeek.ViewModels.Models
{
    public class CreateUserEmailModel
    {
        [ValidatorEmail]
        public string Email { get; set; }
        public string VerificationCode { get; set; }
    }
}