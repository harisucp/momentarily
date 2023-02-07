namespace BraintreePayments.Models
{
    public class Customer
    {
        public string Id { get; set; }
        public string PaymentMethodToken { get; set; }
        public Customer()
        {
            Id = string.Empty;
            PaymentMethodToken = string.Empty;
        }
    }
}
