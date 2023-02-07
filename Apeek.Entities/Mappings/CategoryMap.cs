using Apeek.Entities.Entities;
using Apeek.Entities.Interfaces;
namespace Apeek.Entities.Mappings
{
   public class CategoryMap : AuditEntityMap<Category>
    {
        public CategoryMap()
        {
            Table("c_category");
            Id(x => x.Id, "id");
            Map(x => x.ParentId, "parent_id");
            Map(x => x.IsRoot, "is_root");
            Map(x => x.Name, "name");
            Map(x => x.ImageFileName, "image_file_name");
        }   
    }
}