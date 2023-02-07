using FluentNHibernate.Mapping;
using Apeek.Entities.Entities;
namespace Apeek.Entities.Mappings
{
    public class GoodImgMap : ClassMap<GoodImg>
    {
        public GoodImgMap()
        {
            Table("c_good_img");
            Id(x => x.Id, "id");
            Map(x => x.GoodId, "good_id");
            Map(x => x.UserId, "user_id");
            Map(x => x.Type, "type");
            Map(x => x.Sequence, "sequence");
            Map(x => x.FileName, "file_name");
            Map(x => x.Folder, "folder");
        }
    }
}