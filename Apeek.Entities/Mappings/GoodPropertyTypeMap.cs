using Apeek.Entities.Entities;
using Apeek.Entities.Interfaces;
namespace Apeek.Entities.Mappings
{
   public class GoodPropertyTypeMap : AuditEntityMap<GoodPropertyType>
    {
        public GoodPropertyTypeMap()
        {
            Table("d_good_property_type");
            Id(x => x.Id, "id");
            Map(x => x.Name, "name");
        }
    }
}