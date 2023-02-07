using FluentNHibernate.Mapping;
using Apeek.Entities.Entities;
namespace Apeek.Entities.Mappings
{
    public class HistoryUserMap : ClassMap<HistoryUser>
    {
        public HistoryUserMap()
        {
            Table("c_history_user");
            Id(x => x.Id, "history_user_id");
            Map(x => x.Name, "name");
            Map(x => x.Email, "email");
            Map(x => x.LocationId, "location_id");
            HasMany(x => x.HistoryPhoneNumbers).KeyColumn("history_user_id")
                //.Not.KeyNullable()
                .Cascade.SaveUpdate()
                .Not.LazyLoad();
        }
    }
}