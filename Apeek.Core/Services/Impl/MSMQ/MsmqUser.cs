using System;
using System.Messaging;
namespace Apeek.Core.Services.Impl.MSMQ
{
    public abstract class MsmqUser
    {
        // использование восстанавливаемых сообщений
        private bool recoverable = false;
        public bool Recoverable
        {
            get
            {
                return recoverable;
            }
            set
            {
                recoverable = value;
            }
        }
        // объекты форматирования для посылки  приема сообщений
        protected IMessageFormatter requestFormatter;
        protected IMessageFormatter answerFormatter;
        //
        protected MsmqUser(QueueFormatter formatterType)
        {
            if (formatterType == QueueFormatter.Xml)
            {
                requestFormatter = new XmlMessageFormatter(new Type[] { typeof(MsmqEnvelope) });
                answerFormatter = new XmlMessageFormatter(new Type[] { typeof(MsmqEnvelope) });
            }
            if (formatterType == QueueFormatter.Binary)
            {
                requestFormatter = new BinaryMessageFormatter();
                answerFormatter = new BinaryMessageFormatter();
            }
        }
    }
}