using System;
using System.Collections.Generic;
using System.Messaging;
using Apeek.Core.Services.Impl.Img;
using Apeek.Common;
using Apeek.Common.EventManager.DataTracker;
using Apeek.Common.Logger;
namespace Apeek.Core.Services.Impl.MSMQ
{
    public class MsmqServer: MsmqUser, IDisposable
    {
        // очередь приема запросов
        private MessageQueue queueReceive;
        // событие, вызываемое при приеме запроса       
        //public event ProcessRequestEventHandler<RequestType, AnswerType> ProcessMessage;
        private List<MsmqProcessorPlugin> _plugins { get; set; }
        public MsmqServer(MsmqProcessingParams msmqProcessingParams)
            : base(msmqProcessingParams.FormatterType)
        {                       
            // создание очереди приема сообщений, если она не существует
            queueReceive = MsmqTools.CreateQueue(msmqProcessingParams.QueueReceiveName, QueueType.Transactional);         
            queueReceive.Formatter = requestFormatter;
            _plugins = new List<MsmqProcessorPlugin>();
            _plugins.Add(new ImageProcessor());
        }
        public void Dispose()
        {
            EndReceive();
            queueReceive.Close();
            queueReceive.Dispose();
        }
        public void BeginReceive()
        {
            queueReceive.PeekCompleted += OnPeek;
            queueReceive.BeginPeek();
        }
        // прекратить прием запросов от клиента
        public void EndReceive()
        {
            queueReceive.PeekCompleted -= OnPeek;
        }
        // обработчки события PeekCompleted очереди с запосами
        private void OnPeek(Object source, PeekCompletedEventArgs asyncResult)
        {
            // создание внутренней транзакции MSMQ 
            MessageQueueTransaction transaction = new MessageQueueTransaction();
            // начало транзакции
            transaction.Begin();
            try
            {
                queueReceive.EndPeek(asyncResult.AsyncResult);
                // прием cообщения в рамках транзакции 
                Message message = queueReceive.Receive(transaction);
                // в поле ResponseQueue содержится ссылка на очередь, 
                // куда следует послать ответ на запрос
                MessageQueue queueResponse = message.ResponseQueue;
                MsmqEnvelope envelope = message.Body as MsmqEnvelope;
                if (envelope != null)
                {
                    Ioc.Get<IDbLogger>().LogMessage(LogSource.MsmqServer, string.Format("Received message for processing: {0}", envelope));
                    int processStatus = ProcessStatus.Unprocessed;
                    foreach (var msmqProcessorPlugin in _plugins)
                    {
                        processStatus = msmqProcessorPlugin.Process(envelope);
                        if (processStatus == ProcessStatus.Processed)
                        {
                            //Send answer back to sender
                            if ((queueResponse != null) && (msmqProcessorPlugin.Answer != null))
                            {
                                Message answerMessage = new Message(msmqProcessorPlugin.Answer, answerFormatter);
                                answerMessage.Label = "Answer";
                                answerMessage.CorrelationId = message.Id;
                                answerMessage.Recoverable = Recoverable;
                                // послать собщение в рамках транзакции
                                queueResponse.Send(answerMessage, transaction);
                                queueResponse.Close();
                                queueResponse.Dispose();
                            }
                            // продолжить прием запросов
                            queueReceive.BeginPeek();
                            // завершить транзакцию 
                            transaction.Commit();
                            break;
                        }
                    }
                    if (processStatus == ProcessStatus.Unprocessed || processStatus == ProcessStatus.Error)
                    {
                        Ioc.Get<IDbLogger>().LogError(LogSource.MsmqServer, string.Format("Cannot process message: {0}", envelope));
                        transaction.Abort();
                    }
                }
                else
                {
                    Ioc.Get<IDbLogger>().LogError(LogSource.MsmqServer, "Cannot process msmq message: envelope is null");
                    transaction.Abort();
                }
            }
            catch (Exception ex)
            {
                // отменить транзакцию в случае ошибки
                Ioc.Get<IDbLogger>().LogException(LogSource.MsmqServer, ex);
                transaction.Abort();
            }
        }
    }
}