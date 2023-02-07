using FluentNHibernate.Mapping;
using Apeek.Entities.Entities;
namespace Apeek.Entities.Mappings
{
    public class UserImgMap : ClassMap<UserImg>
    {
        public UserImgMap()
        {
            Table("c_user_img");
            Id(x => x.Id, "user_img_id");
            Map(x => x.UserId, "user_id");
            Map(x => x.Type, "type");
            Map(x => x.Sequence, "sequence");
            Map(x => x.FileName, "file_name");
        }
    }
}