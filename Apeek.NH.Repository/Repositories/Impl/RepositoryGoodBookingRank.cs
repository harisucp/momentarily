using System.Linq;
using Apeek.Entities.Entities;
using Apeek.NH.Repository.Common;
namespace Apeek.NH.Repository.Repositories.Impl
{
    public class RepositoryGoodBookingRank : RepositoryAudit<GoodBookingRank>, IRepositoryGoodBookingRank
    {
        public IQueryable<GoodBookingRank> GetRankFromSeekers(int userId)
        {
            var ranks = from r in Table
                    join r1 in Table on r.GoodRequestId equals r1.GoodRequestId
                    where r.SharerId == userId && r1.Id!=r.Id && r1.SeekerId== r.ReviewerId && r.is_deleted == false
                    //orderby r.CreateBy descending
                    select r;
            return ranks;
        }
        public IQueryable<GoodBookingRank> GetRankFromSharers(int userId)
        {
            var ranks = from r in Table
                        join r1 in Table on r.GoodRequestId equals r1.GoodRequestId
                        where r.SeekerId == userId && r1.Id != r.Id && r1.SharerId == r.ReviewerId && r.is_deleted == false
                        //orderby r.CreateBy descending
                        select r;
            return ranks;
        }
    }
}
