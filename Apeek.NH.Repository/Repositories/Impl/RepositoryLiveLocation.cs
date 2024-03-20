using Apeek.Entities.Entities;
using Apeek.NH.Repository.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.NH.Repository.Repositories.Impl
{
    public class RepositoryLiveLocation : RepositoryAudit<LiveLocation>, IRepositoryLiveLocation
    {
        public bool CheckRequest(int requestId)
        {
           var result = Table.Where(x => x.GoodRequestId == requestId).FirstOrDefault();
            if (result.GoodRequestId == null) return false;
            return true;
        }

        public LiveLocation GetByRequestId(int requestId)
        {
           var result = Table.Where(x => x.GoodRequestId == requestId).Select(x => x).FirstOrDefault();
           if (result.DeliveryConfirm == null && result.ReturnConfirm == null) return null;
           return result;
        }

        public LiveLocation GetLocation(string locationId)
        {
            return Table.Where(x => x.LocationId == locationId).Select(x => x).FirstOrDefault();
        }
    }
}