using Apeek.Common.Interfaces;
using Apeek.Entities.Entities;
using Apeek.NH.Repository.Common;
namespace Apeek.NH.Repository.Repositories
{
    public interface IRepositoryUserNotification : IRepositoryAudit<UserNotification>, IDependency
    {
    }
}
