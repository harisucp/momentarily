using FluentNHibernate.Mapping;
using Apeek.Entities.Entities;
namespace Apeek.Entities.Mappings
{
    public class UserSecurityDataChangeRequestMap : ClassMap<UserSecurityDataChangeRequest>
    {
        public UserSecurityDataChangeRequestMap()
        {
            Table("c_user_security_data_change_request");
            Id(x => x.Id, "user_security_data_change_request_id");
            Map(x => x.UserId, "user_id");
            Map(x => x.DataType, "data_type");
            Map(x => x.NewValue, "new_value");
            Map(x => x.OldValue, "old_value");
            Map(x => x.VerificationCode, "verification_code");
            Map(x => x.CreateDate, "create_date");
            Map(x => x.Verified, "verified");
        }
    }
}