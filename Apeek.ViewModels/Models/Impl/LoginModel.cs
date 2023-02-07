using System.ComponentModel.DataAnnotations;
using Apeek.Entities.Validators;
namespace Apeek.ViewModels.Models
{
    public class LoginModel
    {
        //[Required]
        //[Display(Name = "User name")]
        //public string UserName { get; set; }
        [Required]
        [ValidatorEmailOrPhoneNum]
        [Display(Name = "User email or phone number")]
        public string EmailAddressOrPhoneNum { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}