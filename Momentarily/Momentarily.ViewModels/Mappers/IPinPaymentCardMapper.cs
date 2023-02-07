using Apeek.Common.Interfaces;
using Apeek.Entities.Entities;
using Apeek.ViewModels.Mappers;
using Apeek.ViewModels.Models.Impl;
namespace Momentarily.ViewModels.Mappers
{
    public interface IPinPaymentCardMapper : IDependency, IMapper<PinPaymentCard, PinPaymentCardViewModel>, IMapper<PinPaymentCardViewModel, PinPaymentCard>
    {
    }
}