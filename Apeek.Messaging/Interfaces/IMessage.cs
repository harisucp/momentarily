using Apeek.Common.Interfaces;
using Apeek.Messaging.Implementations;
namespace Apeek.Messaging.Interfaces
{
    public interface IMessage : IDependency
    {
        FromHeader FromHeader { get; set; }
        string To { get; set; }
        
        string Bcc { get; set; }
        string Body { get; set; }
        bool IsBodyHtml { get; set; }
    }
}