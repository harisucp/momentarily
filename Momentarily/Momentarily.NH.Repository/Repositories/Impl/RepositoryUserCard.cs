using System.Linq;
using Apeek.Entities.Entities;
using Apeek.NH.Repository.Common;
namespace Apeek.NH.Repository.Repositories.Impl
{
    public class RepositoryUserCard : RepositoryAudit<PinPaymentCard>, IRepositoryUserCard
    {
        public IQueryable<PinPaymentCard> GetUserCards(int userId)
        {
            return Table.Where(c => c.UserId == userId);
        }
    }
}
