using Apeek.Common.Interfaces;
using Apeek.Entities.Entities;
using Apeek.NH.Repository.Common;
namespace Apeek.NH.Repository.Repositories
{
    public interface IRepositoryPhoneNumber : IRepository<PhoneNumber>, IDependency
    {
        string GetUserPhone(int userId);
        string GetUsercountry(int userId);
        string GetUserPhoneForTemplate(int userId);
    }
}