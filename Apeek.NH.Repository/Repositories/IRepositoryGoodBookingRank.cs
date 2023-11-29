using System.Linq;
using Apeek.Common.Interfaces;
using Apeek.Entities.Entities;
using Apeek.NH.Repository.Common;
namespace Apeek.NH.Repository.Repositories
{
    public interface IRepositoryGoodBookingRank : IRepositoryAudit<GoodBookingRank>, IDependency
    {
        IQueryable<GoodBookingRank> GetRankFromSeekers(int? userId);
        IQueryable<GoodBookingRank> GetRankFromSharers(int? userId);
    }
}
