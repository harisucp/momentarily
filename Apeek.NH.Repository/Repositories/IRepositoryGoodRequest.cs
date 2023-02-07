using System.Collections.Generic;
using System.Threading.Tasks;
using Apeek.Common.Interfaces;
using Apeek.Entities.Entities;
using Apeek.NH.Repository.Common;
namespace Apeek.NH.Repository.Repositories
{
    public interface IRepositoryGoodRequest : IRepositoryAudit<GoodRequest>, IDependency
    {
        GoodRequest GetGoodRequest(int userId, int requestId);
        GoodRequest GetUserRequest(int userId, int requestId);
        GoodRequest GetUserRequest(int requestId);
        IEnumerable<GoodRequest> GetGoodRequests(int userId, int goodId);
        IList<T> GetItems<T>(string query);

     Task <List<GoodRequest>> GetGoodRequestNotRespondedJob();
        
    }
}
