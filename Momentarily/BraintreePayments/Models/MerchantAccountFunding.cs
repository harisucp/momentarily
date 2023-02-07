namespace BraintreePayments.Models
{
    public class MerchantAccountFunding
    {
        public string AccountNumber { get; set; }                
        public string RoutingNumber { get; set; }
        public MerchantAccountFunding()
        {
            AccountNumber = string.Empty;            
            RoutingNumber = string.Empty;
        }
    }
}
