using System;
using System.Collections.Generic;
using System.Messaging;
using Apeek.Common;
using Apeek.Common.Interfaces;
using Apeek.Common.Logger;
namespace Apeek.Core.Services.Impl.MSMQ
{
    public interface IMsmqClient<RequestType> : IDependency
    {
        bool Send(RequestType request);
    }
    public class MsmqClient<RequestType, AnswerType> : MsmqUser, IDisposable, IMsmqClient<RequestType>
    {
        // очереди для отсылки запросов и приема ответов
        private MessageQueue queueSend;
        private MessageQueue queueReceive;
        // список необслуженных запросов
        private Dictionary<String, RequestType> messages;  
        public Dictionary<String, RequestType> Messages 
        {
            get { return messages;}
        }
        // событие, вызываемое при приеме ответа
        public event ProcessAnswerEventHandler<RequestType, AnswerType> ProcessAnswer;
        public MsmqClient(MsmqProcessingParams msmqProcessingParams) : base(msmqProcessingParams.FormatterType)
        {
            // список отправленных сообщений без ответов                                
            messages = new Dictionary<String,RequestType>();
            // создание очереди для посылки запросов, если она не существует
            queueSend = MsmqTools.CreateQueue(msmqProcessingParams.QueueSendName, QueueType.Transactional);
            // создание очереди для приема ответов, если она нужна
            if (msmqProcessingParams.QueueReceiveName != null)
            {
                queueReceive = MsmqTools.CreateQueue(msmqProcessingParams.QueueReceiveName);         
                queueReceive.Formatter = answerFormatter;
                // считывать из очереди свойство CorrelationId
                queueReceive.MessageReadPropertyFilter.CorrelationId = true;
            }
            else
            {
                queueReceive = null;
            }
        }
        public void Dispose()
        {
            queueSend.Close();
            queueSend.Dispose();
            if (queueReceive != null)
            {
                queueReceive.Close();
                queueReceive.Dispose();
            }
        }
        public void BeginReceive()
        {
            if (queueReceive == null)
                throw new ApeekException("Cannot begin receive. Receive Queue was not defined");
            if (ProcessAnswer == null)
                throw new ApeekException("Cannot begin receive. Process Answer event was not defined");
            // установить обработчик на событие, возникающее при появлении 
            // сообщения в очереди            
            queueReceive.PeekCompleted += OnPeek;
            // начать отслеживание поступления сообщения в очередь
            queueReceive.BeginPeek();          
        }   
        // прекратить прием ответов сервера 
        public void EndReceive()
        {
            if (queueReceive == null)
                throw new ApeekException("Cannot end receive. Receive Queue was not defined");
            if (ProcessAnswer == null)
                throw new ApeekException("Cannot end receive. Process Answer event was not defined");
            // отключить обработчик 
            queueReceive.PeekCompleted -= OnPeek;
        }
        public bool Send(RequestType request)
        {
            try
            {
                // создание нового сообщения
                Message message = new Message(request, requestFormatter);      
                message.ResponseQueue = queueReceive;           
                // использование восстаналиваемых сообщений
                message.Recoverable = Recoverable;
                // послать сообщение; поскольку транзакция состоит из 
                // единственной операции, вместо объекта-транзакции используется
                // значение MessageQueueTransactionType.Single
                queueSend.Send(message, MessageQueueTransactionType.Single);
                if (queueReceive != null)
                {
                    // поле message.Id устанавливается после посылки сообщения;
                    // идентификатор сообщения связывается c отосланным запросом
                    // в списке необслуженных запросов
                    messages.Add(message.Id, request);
                }
                return true;
            }
            catch (Exception ex)
            {
                var message = string.Format("Cannot send message to msmq: ({0})", request.ToString());
                Ioc.Get<IDbLogger>().LogException(LogSource.MsmqServer, message, ex);
            }
            return false;
        }
        private void OnPeek(Object source, PeekCompletedEventArgs asyncResult)
        {
            // создание внутренней транзакции MSMQ 
            MessageQueueTransaction transaction = new MessageQueueTransaction();
            // начало транзакции
            transaction.Begin();
            try
            {
                // прекратить ожидание сообщений в очереди 
                queueReceive.EndPeek(asyncResult.AsyncResult);
                // получить сообщение из очереди в рамках транзакции
                Message message = queueReceive.Receive(transaction);
                // в поле CorrelationId должен быть идентификатор сообщения
                // с исходным запросом
                String messageId = message.CorrelationId;
                // есть ли такое сообщение в списке невыполненных запросов?
                if (messages.ContainsKey(messageId))
                {
                    if (message.Body is AnswerType)
                    {
                        // преобразовать тело сообщения к типу ответа
                        // и вызвать событие по его обработке                         
                        AnswerType answer = (AnswerType)message.Body;
                        ProcessAnswer(this, messages[messageId], answer);
                    };
                    messages.Remove(messageId);
                }
                // продолжить ожидать сообщения
                BeginReceive();
                // успешное завершение транзакции
                transaction.Commit();
            }
            catch (Exception e)
            {
                // отмена транзакции
                transaction.Abort();
                throw e;
            }
        }
    }
}