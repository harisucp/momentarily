using System;
using Newtonsoft.Json;
namespace PinPayments.Models
{
    public class TransferAdd : PinError
    {
        public Transfer Response { get; set; }
    }
    public class Transfer
    {
        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("amount")]
        public int Amount { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("currency")]
        public string Currency { get; set; }
        [JsonProperty("total_debits")]
        public string TotalDebits { get; set; }
        [JsonProperty("total_credits")]
        public string TotalCredits { get; set; }
        [JsonProperty("created_at")]
        public DateTime Created { get; set; }
        [JsonProperty("paid_at")]
        public DateTime Paid { get; set; }
        [JsonProperty("bank_account")]
        public Bank Bank { get; set; }
        [JsonProperty("recipient")]
        public string RecipientToken { get; set; }
    }
    public class TransferPost
    {
        [JsonProperty("amount")]
        public int Amount { get; set; }
        private string _currency { get; set; }
        [JsonProperty("currency")]
        public string Currency
        {
            get
            {
                if (_currency == null)
                {
                    return "AUD";
                }
                return _currency;
            }
            set { _currency = value; }
        }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("recipient")]
        public string RecipientToken { get; set; }
    }
}