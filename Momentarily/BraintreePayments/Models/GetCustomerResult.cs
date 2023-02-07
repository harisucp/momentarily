namespace BraintreePayments.Models
{
    public class GetCustomerResult : BaseResult
    {
        public Customer Customer { get; set; }               
        public GetCustomerResult()
        {
            Customer = new Customer();
        }
    }
}
