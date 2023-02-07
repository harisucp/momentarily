using Apeek.Entities.Entities;
using Apeek.Entities.Interfaces;
namespace Apeek.Entities.Mappings
{
  public class GoodPropertyValueMap: AuditEntityMap<GoodPropertyValue>
    {
        public GoodPropertyValueMap()
        {
            Table("c_good_property_value");
            Id(x => x.Id, "id");
            Map(x => x.GoodId, "good_id");
            Map(x => x.GoodPropertyId, "good_property_id");
            Map(x => x.PropertyValueDefinitionId, "property_value_definition_id").Nullable();
            Map(x => x.Value, "value");
        }   
    }
}