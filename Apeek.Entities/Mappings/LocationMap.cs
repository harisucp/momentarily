using Apeek.Entities.Entities;
using Apeek.Entities.Interfaces;
namespace Apeek.Entities.Mappings
{
    public class LocationMap : AuditEntityUtMap<Location>
    {
        public LocationMap()
        {
            Table("d_location");
            Id(x => x.Id, "location_id");
            Map(x => x.Hidden, "hidden");
            Map(x => x.DisplayInMenu, "display_in_menu");
            Map(x => x.IsRoot, "is_root");
            Map(x => x.Verified, "verified");
            Map(x => x.IsDefault, "is_default");
        }
    }
}