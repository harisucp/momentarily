using Apeek.Messaging.Interfaces;
namespace Apeek.Messaging.Implementations
{
    public class Message : IMessage
    {
        public FromHeader FromHeader { get; set; }
        public string To { get; set; }
      
        public string Bcc { get; set; }
        public string Body { get; set; }
        public bool IsBodyHtml { get; set; }
        public Message()
        {
            Bcc = string.Empty;
        }
    }
}