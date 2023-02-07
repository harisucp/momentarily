using Apeek.Entities.Entities;
using Apeek.Entities.Interfaces;
namespace Apeek.Entities.Mappings
{
   public class GoodPropertyValueDefinitionMap: AuditEntityMap<GoodPropertyValueDefinition>
    {
        public GoodPropertyValueDefinitionMap()
        {
            Table("c_good_property_value_definition");
            Id(x => x.Id, "id");
            Map(x => x.GoodPropertyId, "good_property_id");
            Map(x => x.Name, "name");
            Map(x => x.Value, "value");
        }   
    }
}