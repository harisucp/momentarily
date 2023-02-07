using Apeek.Messaging.Interfaces;
using Twilio;
namespace Momentarily.Core.Service.Impl
{
    public class TwilioSmsSendProvider : ISmsSendProvider
    {
        private TwilioRestClient _client;
        private string _from;
        public bool Auth(ISendProperty sendProperty)
        {
            _client = new TwilioRestClient(sendProperty.Login, sendProperty.Pwd);            
            return _client != null;
        }
        public bool SendMessage(IMessage message)
        {
            var sendMessage = _client.SendMessage(message.FromHeader.From, message.To, message.Body);
            return (sendMessage.Status != "failed" && sendMessage.Status != null);
        }
    }
}