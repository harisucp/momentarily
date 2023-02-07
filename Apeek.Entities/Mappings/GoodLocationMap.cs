using Apeek.Entities.Entities;
using Apeek.Entities.Interfaces;
namespace Apeek.Entities.Mappings
{
    public class GoodLocationMap : AuditEntityMap<GoodLocation>
    {
        public GoodLocationMap()
        {
            Table("c_good_location");
            Id(x => x.GoodId, "good_id").GeneratedBy.Assigned();
            Map(x => x.Latitude, "latitude");
            Map(x => x.Longitude, "longitude");
        }
    }
}
