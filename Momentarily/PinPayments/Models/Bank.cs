using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace PinPayments.Models
{
    public class Bank
    {
        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("bank_name")]
        public string BankName { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("bsb")]
        public string BSB { get; set; }
        [JsonProperty("number")]
        public string Number { get; set; }
    }
}
