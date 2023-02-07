using Apeek.Common.Interfaces;
using Apeek.Entities.Entities;
using Apeek.ViewModels.Models;
namespace Apeek.ViewModels.Mappers
{
    public interface IGoodRequestMapper : IDependency, IMapper<GoodRequest, GoodRequestViewModel>
    {
    }
}
