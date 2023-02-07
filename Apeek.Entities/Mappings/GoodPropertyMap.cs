using Apeek.Entities.Entities;
using Apeek.Entities.Interfaces;
namespace Apeek.Entities.Mappings
{
   public class GoodPropertyMap : AuditEntityMap<GoodProperty>
    {
        public GoodPropertyMap()
        {
            Table("d_good_property");
            Id(x => x.Id, "id");
            Map(x => x.Name, "name");
            Map(x => x.TypeId, "type_id");
        }   
    }
}