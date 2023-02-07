using System.ComponentModel.DataAnnotations;
namespace Apeek.ViewModels.Models
{
    public class UserPwdModel
    {
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        [Required]
        public string NewPassword { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        [Required]
        public string ConfirmPassword { get; set; } 
        public bool? Result { get; set; }
        public override string ToString()
        {
            return string.Format("NewPwd:{0}; ConfirmPwd:{1}.", NewPassword, ConfirmPassword);
        }
    }
}