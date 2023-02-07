using FluentNHibernate.Mapping;
using Apeek.Entities.Entities;
namespace Apeek.Entities.Mappings
{
    public class UserRankMap : ClassMap<UserRank>
    {
        public UserRankMap()
        {
            Table("g_user_rank");
            Id(x => x.UserId, "user_id").GeneratedBy.Assigned();
            Map(x => x.Rank, "rank");
        }
    }
}