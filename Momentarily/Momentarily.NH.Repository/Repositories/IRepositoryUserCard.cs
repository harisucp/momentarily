using System.Linq;
using Apeek.Common.Interfaces;
using Apeek.Entities.Entities;
using Apeek.NH.Repository.Common;
using NHibernate.Persister.Entity;
namespace Apeek.NH.Repository.Repositories
{
    public interface IRepositoryUserCard: IRepositoryAudit<PinPaymentCard>, IDependency
    {
        IQueryable<PinPaymentCard> GetUserCards(int userId);
    }
}