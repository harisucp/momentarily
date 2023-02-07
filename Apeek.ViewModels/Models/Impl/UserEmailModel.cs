using System.ComponentModel.DataAnnotations;
using Apeek.Common;
namespace Apeek.ViewModels.Models
{
    public class UserEmailModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "User email")]
        public string UserEmail { get; set; }
        [EmailAddress]
        public string OldUserEmail { get; set; }
        public CreateResult? Result { get; set; }
        public UserEmailModel()
        {
            Result = CreateResult.Error;
        }
    }
}