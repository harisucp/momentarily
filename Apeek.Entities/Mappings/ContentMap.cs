using FluentNHibernate.Mapping;
using Apeek.Entities.Entities;
namespace Apeek.Entities.Mappings
{
    public class ContentMap : ClassMap<Content>
    {
        public ContentMap()
        {
            Table("c_content");
            Id(x => x.Id, "content_id");
            Map(x => x.Hidden, "hidden");
            Map(x => x.CreateDate, "create_date");
            Map(x => x.ModDate, "mod_date");
            Map(x => x.CreateBy, "create_by");
            Map(x => x.ModBy, "mod_by");
            Map(x => x.PageName, "page_name");
        }
    }
}