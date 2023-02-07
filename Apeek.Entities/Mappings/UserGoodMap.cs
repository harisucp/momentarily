using Apeek.Entities.Entities;
using Apeek.Entities.Interfaces;
namespace Apeek.Entities.Mappings
{
    public class UserGoodMap : AuditEntityMap<UserGood>
    {
        public UserGoodMap()
        {
            Table("c_user_good");
            Id(x => x.Id, "id");
            Map(x => x.UserId, "user_id");
            Map(x => x.GoodId, "good_id");
            References(x => x.User)
                .Column("user_id")
                .Not.LazyLoad()
                .Not.Update()
                .Not.Insert();
        }
    }
}
