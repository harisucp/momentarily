using Apeek.Entities.Entities;
using FluentNHibernate.Mapping;
namespace Apeek.Entities.Mappings
{
  public class PhoneNumberMap : ClassMap<PhoneNumber>
    {
        public PhoneNumberMap()
        {
            Table("c_phone_number");
            References(x => x.Address).Column("address_id");
            Id(x => x.Id, "id");
            Map(x => x.UserId, "user_id");
            Map(x => x.CountryCode, "country_code");
            Map(x => x.PhoneNum, "phone_number");
        }
    }
}
