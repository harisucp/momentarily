namespace BraintreePayments.Models
{
    public class CreatePaymentMethodResult: BaseResult
    {
        public string Token { get; set; }
        public CreatePaymentMethodResult()
        {
            Token = string.Empty;
        }
    }
}
