namespace BraintreePayments.Models
{
    public class MerchantAccount
    {
        public string Id { get; set; }
        public MerchantAccountIndividual Individual { get; set; }
        public MerchantAccountFunding Funding { get; set; }
        public MerchantAccount()
        {
            Id = string.Empty;
            Individual = new MerchantAccountIndividual();
            Funding = new MerchantAccountFunding();
        }
    }
}
