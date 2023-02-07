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
        // ������� ������ ��������
        private MessageQueue queueReceive;
        // �������, ���������� ��� ������ �������       
        //public event ProcessRequestEventHandler<RequestType, AnswerType> ProcessMessage;
        private List<MsmqProcessorPlugin> _plugins { get; set; }
        public MsmqServer(MsmqProcessingParams msmqProcessingParams)
            : base(msmqProcessingParams.FormatterType)
        {                       
            // �������� ������� ������ ���������, ���� ��� �� ����������
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
        // ���������� ����� �������� �� �������
        public void EndReceive()
        {
            queueReceive.PeekCompleted -= OnPeek;
        }
        // ���������� ������� PeekCompleted ������� � ��������
        private void OnPeek(Object source, PeekCompletedEventArgs asyncResult)
        {
            // �������� ���������� ���������� MSMQ 
            MessageQueueTransaction transaction = new MessageQueueTransaction();
            // ������ ����������
            transaction.Begin();
            try
            {
                queueReceive.EndPeek(asyncResult.AsyncResult);
                // ����� c�������� � ������ ���������� 
                Message message = queueReceive.Receive(transaction);
                // � ���� ResponseQueue ���������� ������ �� �������, 
                // ���� ������� ������� ����� �� ������
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
                                // ������� �������� � ������ ����������
                                queueResponse.Send(answerMessage, transaction);
                                queueResponse.Close();
                                queueResponse.Dispose();
                            }
                            // ���������� ����� ��������
                            queueReceive.BeginPeek();
                            // ��������� ���������� 
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
                // �������� ���������� � ������ ������
                Ioc.Get<IDbLogger>().LogException(LogSource.MsmqServer, ex);
                transaction.Abort();
            }
        }
    }
}