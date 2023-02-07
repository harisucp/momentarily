using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace Momentarily.ViewModels.Models
{
    public class UserBankInfoViewModel
    {
        public int Id { get; set; }
        public string RecipientToken { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [RegularExpression("^[0-9]{3}$", ErrorMessage = "First part of BSB must contain 3 digits")]
        public string BSB1 { get; set; }
        [Required]
        [RegularExpression("^[0-9]{3}$", ErrorMessage = "Second part of BSB must contain 3 digits")]
        public string BSB2 { get; set; }
        //[Required]
        //public string BSB { get; set; }
        [Required]
        [RegularExpression("^[0-9]{6,10}$", ErrorMessage = "Account Number must contain 6-10 digits")]
        public string Number { get; set; }
        [ReadOnly(true)]
        public string BankName { get; set; }
    }
}