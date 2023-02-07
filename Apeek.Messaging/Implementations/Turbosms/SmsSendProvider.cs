using System;
using Apeek.Messaging.Interfaces;
using Apeek.Sms.Implementations.Turbosms.Service;
using Apeek.Common;
using Apeek.Common.Logger;
namespace Apeek.Messaging.Implementations.Turbosms
{
    public class SmsSendProvider : ISmsSendProvider
    {
        private SMSWorker worker;
        public SmsSendProvider()
        {
            worker = SMSWorker.GetInstance();
        }
        public bool Auth(ISendProperty sendProperty)
        {
            bool authenticated = false;
            try
            {
                string res = worker.Auth(sendProperty.Login, sendProperty.Pwd);
                if (res == "Вы успешно авторизировались")
                {
                    authenticated = true;
                }
                else
                {
                    worker.CloseSession();
                    Ioc.Get<DbLogger>().LogError(LogSource.TurbosmsProvider, string.Format("Cannot auth to torbosms service. The result is: {0}", res));
                }
            }
            catch (Exception ex)
            {
                Ioc.Get<DbLogger>().LogException(LogSource.TurbosmsProvider, string.Format("Cannot auth to torbosms service."), ex);
            }
            return authenticated;
        }
        public bool SendMessage(IMessage message)
        {
            string resStr = "";
            try
            {
                if(message.Body.Length > 159)
                    throw new ApeekException(string.Format("SMS message length exceeds 159 chars: {0}", message.Body));
                string[] res = worker.SendSMS(message.FromHeader.From, message.To, message.Body, string.Empty);
                if (res.Length == 2)
                    resStr = String.Format("{0} {1}", res[0], res[1]);
                else
                    resStr = res[0];
                Ioc.Get<DbLogger>().LogMessage(LogSource.TurbosmsProvider, string.Format("Message sent via torbosms service. The result is: {0}", resStr));
                return res[0] == "Сообщения успешно отправлены";
            }
            catch (Exception ex)
            {
                Ioc.Get<DbLogger>().LogException(LogSource.TurbosmsProvider, string.Format("Cannot send messages via torbosms service."), ex);
            }
            return false;
        }
    }
}