using System;
namespace Apeek.Core.Services.Impl.MSMQ
{
    public class MsmqProcessingParams
    {
        public String QueueSendName { get; set; }
        public String QueueReceiveName{ get; set; }
        public QueueFormatter FormatterType { get; set; }
    }
}