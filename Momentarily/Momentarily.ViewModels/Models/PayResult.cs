using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Momentarily.ViewModels.Models
{
   public class PayResult
    {
        public bool IsError { get; set; }
        public string Message { get; set; }
        public int GoodRequestId { get; set; }
        public bool MailSend { get; set; }
    }
    public class OrderPayResult
    {
        public bool IsError { get; set; }
        public string Message { get; set; }
        public int CovidGoodId { get; set; }
        public bool MailSend { get; set; }
    }
}
