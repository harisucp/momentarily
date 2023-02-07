using Apeek.Entities.Entities;
using Apeek.Entities.Interfaces;
namespace Apeek.Entities.Mappings
{
    public class GoodCategoryMap : AuditEntityMap<GoodCategory>
    {
        public GoodCategoryMap()
        {
            Table("c_good_category");
            Id(x => x.Id, "id");
            //Map(x => x.CreateDate, "create_date");
            //Map(x => x.ModDate, "mod_date");
            //Map(x => x.CreateBy, "create_by");
            //Map(x => x.ModBy, "mod_by");
            Map(x => x.GoodId, "good_id");
            Map(x => x.CategoryId, "category_id");
        }
    }
}