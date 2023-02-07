using System.Collections.Generic;
namespace Apeek.ViewModels.Models
{
    public class UserConversationViewModel
    {
        public List<UserMessageViewModel> Messages { get; set; }
        public int AuthorId { get; set; }
        public int ReceiverId { get; set; }
        public string AuthorUserName { get; set; }
        public string AuthorImageUrl { get; set; }
        public string ReceiverUserName { get; set; }
        public string ReceiverImageUrl { get; set; }
        public string messageUrlPath { get; set; }
    }
}