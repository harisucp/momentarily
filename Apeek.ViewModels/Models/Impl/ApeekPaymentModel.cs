using Apeek.Core.Services;
namespace Apeek.ViewModels.Models.Impl
{
    public class ApeekPaymentModel:IApeekPayment
    {
        public int GoodId { get;set;}
        public int RequestId { get; set; }
        public string Type { get; set; }
        public double Cost { get; set; }
        public double SecurityDeposit { get; set; }
        public double ServiceFee { get; set; }
        public string GoodName { get; set; }
        public string GoodDescription { get; set; }
    }
    public class AppekPaymentVoid
    {
        public string PaymentId { get; set; }
        public int GoodsUserId { get; set; }
        public int GoodRequestId { get; set; }
        public string PayerId { get; set; }
    }
}
