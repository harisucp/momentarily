using System;
using System.ComponentModel.DataAnnotations;
using Apeek.Entities.Entities;
namespace Apeek.ViewModels.Models
{
    public abstract class RegisterModel : IRegisterModel
    {
        [Required(ErrorMessage = "E-mail address is required.")]
        [Display(Name = "User email")]
        [DataType(DataType.EmailAddress,ErrorMessage = "E-mail address is invalid.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "First name is required.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last name is required.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        public Address Address { get; set; }
        public string CountryCode { get; set; }
        [Required(ErrorMessage = " The phone number is required.")]
        [MaxLength(10 ,ErrorMessage = "Maximum 10 digits allowed")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage =" The password is required.")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        [Required (ErrorMessage = "Confirm password is required.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password does not match. Try again.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Date of birth is required.")]
        [Display(Name = "Date of birthday")]
        public virtual DateTime DateOfBirthday { get; set; }
        public virtual DateTime? SendLinkDate { get; set; }
        public bool IsExternal { get; set; }
       public string GoogleId { get; set; }
        public string FacebookId { get; set; }
        public bool IgnoreMarketingEmails { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsBlocked { get; set; }
        public bool IsMobileVerified { get; set; }
        [Required(ErrorMessage = "The country is required.")]
        public int? CountryId { get; set; }
    }
}