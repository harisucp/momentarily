using System.ComponentModel.DataAnnotations;
namespace Apeek.ViewModels.Models
{
    public class RegisterExternalLoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }
        public string ExternalLoginData { get; set; }
    }
}