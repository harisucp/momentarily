using Apeek.Entities.Entities;
using Apeek.NH.Repository.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.NH.Repository.Repositories.Impl
{
   public class RepositorySubscibes : RepositoryAudit<Subscribes>, IRepositorySubscibes
    {
        public bool SubscriberAlreadyExsist(string email)        {            bool result = false;            string emailID = email.Trim();

            var exsist = Table.Where(x => x.Email == emailID && x.SubscribeStatus == true).Count();
            if (exsist == 0)
                result = false;
            else
                result = true;

            return result;        }
        public List<Subscribes> GetAllSubscriber()
        {
            return Table.Select(x => x).ToList();
        }
    }
}
