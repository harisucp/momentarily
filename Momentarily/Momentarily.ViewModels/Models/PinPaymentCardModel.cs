using Apeek.ViewModels.Models.Impl;
namespace Momentarily.ViewModels.Models
{
    public class PinPaymentCardModel
    {
        public PinPaymentCardViewModel Card { get; set; }
        public bool IsNewCard { get; set; }
        public int RequestId { get; set; }
    }
}
