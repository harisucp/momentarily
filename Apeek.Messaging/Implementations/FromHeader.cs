using System.Collections.Specialized;
namespace Apeek.Messaging.Implementations
{
    public class FromHeader
    {
        public string From { get; set; }
        public string FromName { get; set; }
        public string Subject { get; set; }
        public NameValueCollection Headers { get; set; }
        public FromHeader()
        {
            Headers = new NameValueCollection();
        }
    }
}