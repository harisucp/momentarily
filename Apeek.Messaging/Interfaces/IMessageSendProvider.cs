using Apeek.Common.Interfaces;
namespace Apeek.Messaging.Interfaces
{
    public interface IMessageSendProvider
    {
        bool Auth(ISendProperty sendProperty);
        bool SendMessage(IMessage message);
    }
    public interface IMailSendProvider : IMessageSendProvider, IDependency { }
    public interface ISmsSendProvider : IMessageSendProvider, IDependency { }
}