using FluentNHibernate.Mapping;
using Apeek.Entities.Entities;
namespace Apeek.Entities.Mappings
{
    public class UserFieldRankMap : ClassMap<UserFieldRank>
    {
        public UserFieldRankMap()
        {
            Table("g_user_field_rank");
            Id(x => x.Id, "id");
            Map(x => x.FieldName, "field");
            Map(x => x.Quality, "quality");
            Map(x => x.Value, "value");
        }
    }
}