using Apeek.Entities.Entities;
using Apeek.Entities.Interfaces;
namespace Apeek.Entities.Mappings
{
    public class GoodBookingRankMap : AuditEntityMap<GoodBookingRank>
    {
        public GoodBookingRankMap()
        {
            Table(GoodBookingRank._TableName);
            Id(x => x.Id, "id");
            Map(x => x.GoodRequestId, "good_request_id");
            Map(x => x.GoodId, "good_id");
            Map(x => x.SharerId, "sharer_id");
            Map(x => x.SeekerId, "seeker_id");
            Map(x => x.ReviewerId, "reviewer_id");
            Map(x => x.Rank, "rank");
            Map(x => x.Message, "message");
            Map(x => x.is_deleted, "is_deleted");
            References(x => x.Reviewer, "reviewer_id")
                .Not.LazyLoad()
                .Not.Update()
                .Not.Insert();
        }
    }
}
