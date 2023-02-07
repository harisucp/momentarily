using System.Collections.Generic;
namespace Apeek.ViewModels.Settings
{
    public class PayPalSetting
    {
        public string Mode { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string EmailId { get; set; }
        public Dictionary<string, string> Config { get; set; }
    }
}
