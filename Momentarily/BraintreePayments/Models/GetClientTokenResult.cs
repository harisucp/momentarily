namespace BraintreePayments.Models
{
    public class GetClientTokenResult: BaseResult
    {
        public ClientToken ClientToken { get; set; }
        public GetClientTokenResult()
        {
            ClientToken = new ClientToken();
        }
    }
}
