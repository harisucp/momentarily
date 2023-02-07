using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apeek.Common;
using Apeek.Common.Definitions;
using Apeek.Common.Logger;
using Apeek.Entities.Entities;
using Apeek.NH.Repository.Common;
using Newtonsoft.Json;

namespace Apeek.NH.Repository.Repositories.Impl
{
    public class RepositoryGoodRequest : RepositoryAudit<GoodRequest>, IRepositoryGoodRequest
    {
        public GoodRequest GetGoodRequest(int userId, int requestId)
        {
            try
            {
                var data = (Table.Join(TableFor<UserGood>(), gR => gR.GoodId, uG => uG.GoodId, (gR, uG) => new { gR, uG })
                    .Where(@t => @t.gR.Id == requestId && @t.uG.UserId == userId)
                    .Select(@t => @t.gR)).FirstOrDefault();
                return data;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Task<List<GoodRequest>> GetGoodRequestNotRespondedJob()
        {
            int count = 0;
             Here:
            try
            {
                var data = Table.Where(x => x.StatusId == 1).ToList();
                if(data.Count==0 && count <=5)
                {
                    count++;
                    goto Here;
                }
               else
                {
                    foreach (GoodRequest req in data)
                    {
                        if (req.CreateDate < DateTime.Now.AddHours(-24))
                        {
                            Session.CreateSQLQuery("Update c_good_request set status_id=:status_id,mod_date=:mod_date where id=:id and status_id =:statusid")
                            //.SetInt32("id", 6)
                             .SetInt32("id", req.Id)
                             .SetInt32("status_id", (int)UserRequestStatus.NotResponded)
                               .SetDateTime("mod_date", DateTime.UtcNow)
                                .SetInt32("statusid", (int)UserRequestStatus.Pending)
                           .ExecuteUpdate();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if(count<=5)
                 goto Here;
                Ioc.Get<IDbLogger>().LogWarning(LogSource.GoodRequestService, string.Format("Not Responded user request fail. Ex: {0}.", ex));
            }

            return null;
        }

        public GoodRequest GetUserRequest(int userId, int requestId)
        {
            return Table.FirstOrDefault(p => p.UserId == userId && p.Id == requestId);
        }

        public GoodRequest GetUserRequest(int requestId)
        {
            return Table.FirstOrDefault(p => p.Id == requestId);
        }

        public IEnumerable<GoodRequest> GetGoodRequests(int userId, int goodId)
        {
            return Table.Join(TableFor<UserGood>(), gR => gR.GoodId, uG => uG.GoodId, (gR, uG) => new { gR, uG })
                    .Where(@t => @t.gR.GoodId == goodId && @t.uG.UserId == userId)
                    .Select(@t => @t.gR);
        }

        public IList<T> GetItems<T>(string query)
        {
            {
                return Session.CreateSQLQuery(query)
                       .SetResultTransformer(NHibernate.Transform.Transformers.AliasToBean(typeof(T)))
                       .List<T>();
            }
        }
    }
}
