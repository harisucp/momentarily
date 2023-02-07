using Apeek.Common.Extensions;
using Apeek.Entities.Entities;
using Apeek.ViewModels.Models.Impl;
namespace Apeek.ViewModels.Mappers.Imp
{
    public class UserMapper : AuditEntityMapper<User, UserUpdateModel>, IUserMapper
    {
        public UserUpdateModel Map(User source, UserUpdateModel dest)
        {
            dest = AuditMap(source, dest);
            dest.Id = source.Id;
            dest.FirstName = source.FirstName;
            dest.LastName = source.LastName;
            dest.Description = source.Description;
            dest.IgnoreMarketingEmails = source.IgnoreMarketingEmails;
            dest.UserProfileImageUrl = source.UserImages.MainImageUrlNormal(false);
            dest.BigUserProfileImageUrl = source.UserImages.MainImageUrlLarge(false);
            return dest;
        }
        public User Map(UserUpdateModel source, User dest)
        {
            dest = AuditMap(source, dest);
            dest.FirstName = source.FirstName;
            dest.LastName = source.LastName;
            dest.FullName = source.FirstName + " " + source.LastName;
            dest.Description = source.Description;
            dest.IgnoreMarketingEmails = source.IgnoreMarketingEmails;
            return dest;
        }
        public UserViewModel Map(User source, UserViewModel dest)
        {
            dest.Id = source.Id;
            dest.FirstName = source.FirstName;
            dest.LastName = source.LastName;
            dest.Description = source.Description;
            dest.Email = source.Email;
            dest.IgnoreMarketingEmails = source.IgnoreMarketingEmails;
            return dest;
        }
    }
}