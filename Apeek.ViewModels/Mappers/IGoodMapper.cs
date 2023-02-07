using Apeek.Common.Interfaces;
using Apeek.Entities.Entities;
using Apeek.ViewModels.Models;
using Apeek.ViewModels.Models.Impl;
namespace Apeek.ViewModels.Mappers
{
    public interface IGoodMapper : IDependency,
        IMapper<Good, IGoodViewModel>, IMapper<Good, UserProfileGoodViewModel>
    {
    }
}