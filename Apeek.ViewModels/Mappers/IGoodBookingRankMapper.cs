using Apeek.Common.Interfaces;
using Apeek.Entities.Entities;
using Apeek.ViewModels.Models.Impl;
namespace Apeek.ViewModels.Mappers
{
    public interface IGoodBookingRankMapper : IDependency, IMapper<GoodBookingRank, ReviewViewModel>
    {
    }
}