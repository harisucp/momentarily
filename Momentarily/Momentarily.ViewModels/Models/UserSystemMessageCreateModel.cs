using Apeek.Common.Validation;
using Momentarily.Common.Definitions;
namespace Momentarily.ViewModels.Models
{
    public class UserSystemMessageCreateModel
    {
        public int AuthorId { get; set; }
        public int ReceiverId { get; set; }
        public int ItemId { get; set; }
        public string ItemUrl { get; set; }
        public SystemMessageType MessageType { get; set; }
        [StringPropertyBind]
        public string Message { get; set; }
        public string PaymentUrl { get; set; }
        //public string DepositUrl { get; set; }
    }
}
