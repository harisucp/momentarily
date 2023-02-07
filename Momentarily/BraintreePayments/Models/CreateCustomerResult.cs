namespace BraintreePayments.Models
{
    public class CreateCustomerResult: BaseResult
    {
        public Customer Customer { get; set; }               
        public CreateCustomerResult()
        {
            Customer = new Customer();
        }
    }
}
