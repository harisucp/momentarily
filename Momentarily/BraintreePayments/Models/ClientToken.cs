namespace BraintreePayments.Models
{
    public class ClientToken
    {
        public string Token { get; set; }
        public ClientToken()
        {
            Token = string.Empty;
        }
    }
}
