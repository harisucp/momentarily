namespace BraintreePayments.Models
{
    public class CreateOrUpdateMerchantAccountResult: BaseResult
    {
        public MerchantAccount MerchantAccount { get; set; }
        public CreateOrUpdateMerchantAccountResult()
        {
            MerchantAccount = null;
        }
    }
}
