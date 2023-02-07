using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apeek.Entities.Entities;
namespace Apeek.ViewModels.Models.Impl
{
    public class UserViewModel:IUserViewModel
    {
        public int Id { get; set; }
        public string FirstName { get;set;}
        public string LastName { get;set;}
        public string Email { get; set; }
        public string FullName { get;set;}
        [StringLength(1000, ErrorMessage = "Description cannot be longer than 1000 characters.")]
        public string Description { get;set;}
        public string PhoneNumber { get;set;}
        public bool IgnoreMarketingEmails { get; set; }
        public UserImageModel UserImage { get; set; }
        public string UserProfileImageUrl { get; set; }
        public string BigUserProfileImageUrl { get; set; }
        public bool Verified { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsBlocked { get; set; }
        public bool IsMobileVerified { get; set; }
    }
}
