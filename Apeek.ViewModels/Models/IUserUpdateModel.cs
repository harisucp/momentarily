using Apeek.Entities.Entities;
namespace Apeek.ViewModels.Models
{
    public interface IUserUpdateModel
    {
        int Id { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        string FullName { get; set; }
        string Email { get; set; }
        UserImageModel UserImage { get; set; }
        string Description { get; set; }
        string PhoneNumber { get; set; }
        bool IgnoreMarketingEmails { get; set; }
        bool IsAdmin { get; set; }
       bool IsBlocked { get; set; }
       bool IsMobileVerified { get; set; }
       string CountryCode { get; set; }
    }
}