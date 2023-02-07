namespace Momentarily.ViewModels.Models.Braintree
{
    public class BraintreePayViewModel
    {
        public int GoodRequestId { get; set; }
        public string PaymentMethodNonce { get; set; }
        public string ClientToken { get; set; }
    }
}
