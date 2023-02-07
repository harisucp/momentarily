using Apeek.Common.Interfaces;
using Apeek.Common.Models;
using Apeek.Core.Services;
using Apeek.ViewModels.Models;
namespace Momentarily.UI.Service.Services
{
    public interface IMomentarilyGoodRequestService : IGoodRequestService, IDependency
    {
        Result<RequestViewModel> GetGoodRequest(int userId, RequestModel requestModel);
        int CloseHoursAfterEnd { get; }
        string getItemPickupLocation(int goodId);
    }
}
