using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Apeek.ViewModels.Settings
{
    public class PinPaymentSetting
    {
        public bool TestMode { get; set; }
        public string LiveSecretKey { get; set; }
        public string LivePublishKey { get; set; }
        public string TestSecretKey { get; set; }
        public string TestPublishKey { get; set; }
    }
}
