using System.Linq;
using Apeek.Common.Logger;
using Apeek.Entities.Entities;
using Apeek.NH.DataAccessLayer.DataAccess;
using Apeek.NH.Repository.Common;
namespace Apeek.NH.Repository.Repositories.Impl
{
    public class RepositoryPhoneNumber :  Repository<PhoneNumber>, IRepositoryPhoneNumber
    {
        public string GetUserPhone(int userId)
        {
            var phone = Table.FirstOrDefault(p => p.UserId == userId);
            return phone == null ? "" : phone.PhoneNum;
        }
        public string GetUsercountry(int userId)
        {
            var country = Table.FirstOrDefault(p => p.UserId == userId);
            return country == null ? "" : country.CountryCode;
        }
        public string GetUserPhoneForTemplate(int userId)        {            var phone = (dynamic)null;            Uow.Wrap(u =>            {                phone = Table.FirstOrDefault(p => p.UserId == userId);            }, null, LogSource.PersonService);            return phone == null ? "" : phone.PhoneNum;        }
    }
}