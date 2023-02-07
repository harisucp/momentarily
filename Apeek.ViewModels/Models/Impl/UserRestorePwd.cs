using System.ComponentModel.DataAnnotations;
using Apeek.Entities.Validators;
namespace Apeek.ViewModels.Models
{
    public class UserRestorePwd
    {
        [Required]
        [ValidatorEmailOrPhoneNum]
        [Display(Name = "User email or phone number")]
        public string EmailAddressOrPhoneNum { get; set; }
    }
}