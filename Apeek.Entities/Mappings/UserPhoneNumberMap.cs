using FluentNHibernate.Mapping;
using Apeek.Entities.Entities;

namespace Apeek.Entities.Mappings
{
    public class UserPhoneNumberMap : ClassMap<UserPhoneNumber>
    {
        public UserPhoneNumberMap()
        {
            Table("c_user_phone_number");

            Id(x => x.Id, "id");
            Map(x => x.UserId, "user_id");
            Map(x => x.PhoneNumber, "phone_number");
        }
    }
}