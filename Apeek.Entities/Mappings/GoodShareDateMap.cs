using Apeek.Entities.Entities;
using FluentNHibernate.Mapping;
using System;
namespace Apeek.Entities.Mappings
{
    public class GoodShareDateMap : ClassMap<GoodShareDate>
    {
        public GoodShareDateMap()
        {
            Table("c_good_share_date");
            Id(x => x.Id, "id");
            Map(x => x.GoodId, "good_id");
            Map(x => x.ShareDate, "share_date");
            Map(x => x.StartTime, "start_time");            Map(x => x.EndTime, "end_time");
        }
    }
}
