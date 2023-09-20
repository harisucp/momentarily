using FluentNHibernate.Mapping;
using Apeek.Entities.Entities;
using Apeek.Entities.Interfaces;
namespace Apeek.Entities.Mappings
{
    public class UserMap : AuditEntityUtMap<User>
    {
        public UserMap()
        {
            Table("c_user");
            Id(x => x.Id, "id");
            Map(x => x.FirstName, "first_name");
            Map(x => x.LastName, "last_name");
            Map(x => x.Url, "url");
            Map(x => x.DateOfBirth, "date_of_birth").Nullable();
            Map(x => x.FullName, "full_name");
            Map(x => x.VerificationCode, "verification_code");
            Map(x => x.Verified, "verified");
            Map(x => x.Email, "email");
            Map(x => x.Pwd, "pwd");
            Map(x => x.Description, "description");
            Map(x => x.TempPwd, "temp_pwd");
            Map(x => x.TempPwdCreateDate, "temp_pwd_create_date");
            Map(x => x.Website, "website_url");
            Map(x => x.AccountAssociationId, "account_association_id");
            Map(x => x.GoogleId, "google_id");
            Map(x => x.FacebookId, "facebook_id");
            Map(x => x.IgnoreMarketingEmails, "ignore_marketing_emails");
            Map(x => x.GeneralUpdate, "general_updates");
            Map(x => x.SendLinkDate, "send_link_date");
            Map(x => x.IsAdmin, "is_admin"); 
            Map(x => x.IsMobileVerified, "is_mobile_verified"); 
            Map(x => x.IsBlocked, "is_blocked");
            Map(x => x.IsRemoved, "is_removed");
            Map(x => x.CreatedDate, "created_date");
            Map(x => x.IsLockout, "IsLockout");
            Map(x => x.OTPCount, "OTPCount");
            Map(x => x.OTPGeneratedDate, "OTPGeneratedDate");
            References(x => x.Address, "address_id");
            HasMany(x => x.UserImages)
                .KeyColumn("user_id")
                .Inverse()
                .Fetch.Join();
        }
    }
    public class PersonFreeTextMap : ClassMap<PersonFreeText>
    {
        public PersonFreeTextMap()
        {
            Table("c_person");
            Id(x => x.Id, "id");
            Map(x => x.FreeText, "free_text");
        }
    }
}
