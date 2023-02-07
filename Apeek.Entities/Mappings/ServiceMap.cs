using Apeek.Entities.Entities;
using Apeek.Entities.Interfaces;
namespace Apeek.Entities.Mappings
{
    public class ServiceMap : AuditEntityUtMap<Service>
    {
        public ServiceMap()
        {
            Table("c_service");
            Id(x => x.Id, "id");
            Map(x => x.Status, "status");
            Map(x => x.Hidden, "hidden");
            Map(x => x.IsRoot, "is_root");
            Map(x => x.Type, "service_type");
            Map(x => x.ShowTags, "show_tags");
        }
    }
}