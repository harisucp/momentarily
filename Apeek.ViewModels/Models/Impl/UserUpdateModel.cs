using System;
using System.ComponentModel.DataAnnotations;
using Apeek.Entities.Entities;
using Apeek.Entities.Interfaces;
using Apeek.Entities.Validators;
namespace Apeek.ViewModels.Models.Impl
{
    public class UserUpdateModel : IUserUpdateModel, IAuditEntity
    {
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public UserImageModel UserImage { get; set; }
        [StringLength(1000, ErrorMessage = "Description cannot be longer than 1000 characters.")]
        public string Description { get; set; }
        [Required(ErrorMessage = " The phone number is required.")]
        [MaxLength(10, ErrorMessage = "Maximum 10 digits allowed")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        public string PhoneNumber { get; set; }
        public bool IgnoreMarketingEmails { get; set; }
        public string UserProfileImageUrl { get; set; }
        public string BigUserProfileImageUrl { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModDate { get; set; }
        public int ModBy { get; set; }
        public int CreateBy { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsBlocked { get; set; }
        public bool IsMobileVerified { get; set; }
        [Required(ErrorMessage = "The country is required.")]
        public string CountryCode { get; set; }
        public  int OTPCount { get; set; }
        public  bool IsLockout { get; set; }
        public  DateTime? OTPGeneratedDate { get; set; }
    }
}