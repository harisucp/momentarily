using System;
using System.Messaging;
namespace Apeek.Core.Services.Impl.MSMQ
{
    public enum QueueType { NonTransactional, Transactional };
    public enum QueueFormatter { Binary, Xml };
    public delegate AnswerType ProcessRequestEventHandler <RequestType, AnswerType>(Object sender, RequestType request,MessageQueue queueResponse);
    public delegate void ProcessAnswerEventHandler<RequestType, AnswerType> (Object sender, RequestType request, AnswerType answer);
}