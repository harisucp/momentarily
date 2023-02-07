using Apeek.Common.Interfaces;
using Apeek.NH.Repository.Common;
using Momentarily.Entities.Entities;
namespace Momentarily.NH.Repository.Repositories
{
    public interface IPinPaymentCustomerRepository : IRepositoryAudit<PinPaymentCustomer>, IDependency
    {
    }
}