using Apeek.Entities.Entities;
using Apeek.ViewModels.Models.Impl;
namespace Momentarily.ViewModels.Mappers.Impl
{
    public class PinPaymentCardMapper:IPinPaymentCardMapper
    {
        public PinPaymentCardViewModel Map(PinPaymentCard source, PinPaymentCardViewModel dest)
        {
            dest.DisplayNumber = source.DisplayNumber;
            dest.CardToken = source.CardToken;
            return dest;
        }
        public PinPaymentCard Map(PinPaymentCardViewModel source, PinPaymentCard dest)
        {
            dest.CardToken = source.CardToken;
            dest.DisplayNumber = source.DisplayNumber;
            return dest;
        }
    }
}