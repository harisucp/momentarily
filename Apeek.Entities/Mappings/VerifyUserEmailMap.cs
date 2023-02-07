using FluentNHibernate.Mapping;
using Apeek.Entities.Entities;
namespace Apeek.Entities.Mappings
{
    public class VerifyUserEmailMap : ClassMap<VerifyUserEmail>
    {
        public VerifyUserEmailMap()
        {
            Table("c_verify_user_email");
            Id(x => x.Id, "id");
            Map(x => x.Email, "email");
            Map(x => x.VerificationCode, "verification_code");
            Map(x => x.Verified, "verified");
            Map(x => x.CreateDate, "create_date");
        }
    }
}