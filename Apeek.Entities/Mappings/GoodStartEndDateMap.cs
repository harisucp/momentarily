using Apeek.Entities.Entities;
using Apeek.Entities.Interfaces;
namespace Apeek.Entities.Mappings
{
    public class GoodStartEndDateMap : AuditEntityMap<GoodStartEndDate>
    {
        public GoodStartEndDateMap()
        {
            Table("c_good_start_end_date");
            Id(x => x.GoodId, "good_id").GeneratedBy.Assigned();
            Map(x => x.DateStart, "date_start").Nullable();
            Map(x => x.DateEnd, "date_end").Nullable();
        }
    }
}
