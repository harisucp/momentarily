using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace PinPayments.Models
{
    public class RecipientAdd : PinError
    {
        public Recipient Response { get; set; }
    }
    public class RecipientResponse : PinError
    {
        public Recipient Response { get; set; }
    }
    public class RecipientPost
    {
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        public Bank Bank { get; set; }
        [JsonProperty("bank_account_token")]
        public string BankToken { get; set; }
    }
    public class Recipient
    {
        public Recipient()
        {
            _bank = new Bank();
        }
        private Bank _bank;
        [JsonProperty("bank_account")]
        public Bank Bank
        {
            get { return _bank; }
            set
            {
                if (value == null)
                {
                    _bank = new Bank();
                }
                else
                {
                    _bank = value;
                }
            }
        }
        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
