using System;
using Apeek.Entities.Entities;
namespace Apeek.ViewModels.Models
{
    public interface IRegisterModel {
        string Email { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        Address Address { get; set; }
        string CountryCode { get; set; }
        string PhoneNumber { get; set; }
        string Password { get; set; }
        string ConfirmPassword { get; set; }
        DateTime DateOfBirthday { get; set; }
        DateTime? SendLinkDate { get; set; }
        bool IsExternal { get; set; }
        string GoogleId { get; set; }
        string FacebookId { get; set; }
        bool IgnoreMarketingEmails { get; set; }
        bool GeneralUpdate { get; set; }
        bool IsAdmin { get; set; }
        bool IsBlocked { get; set; }
        bool IsMobileVerified { get; set; }
        int? CountryId { get; set; }

    }
}