using Apeek.Entities.Entities;
using FluentNHibernate.Mapping;
namespace Apeek.Entities.Mappings
{
    public class AddressMap : ClassMap<Address>
    {
        public AddressMap()
        {
            Table("c_user_address");
            Id(x => x.Id, "id");
            Map(x => x.AddressLine1, "addressLine1");
            Map(x => x.AddressLine2, "addressLine2");
            Map(x => x.AddressLine3, "addressLine3");
            Map(x => x.PostalCode, "postalCode");
            Map(x => x.LocationId, "location_id");
            Map(x => x.Region, "region");
            Map(x => x.Country, "country");
            References(x => x.User, "person_Id");
            HasMany(x => x.PhoneNumberRecords)
                .KeyColumn("address_id");
        }
    }
}