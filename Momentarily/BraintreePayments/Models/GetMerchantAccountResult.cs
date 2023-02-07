namespace BraintreePayments.Models
{
    public class GetMerchantAccountResult: BaseResult
    {
        public MerchantAccount MerchantAccount { get; set; }
        public GetMerchantAccountResult()
        {
            MerchantAccount = null;
        }
    }
}
