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
        // ������� ��� ������� �������� � ������ �������
        private MessageQueue queueSend;
        private MessageQueue queueReceive;
        // ������ ������������� ��������
        private Dictionary<String, RequestType> messages;  
        public Dictionary<String, RequestType> Messages 
        {
            get { return messages;}
        }
        // �������, ���������� ��� ������ ������
        public event ProcessAnswerEventHandler<RequestType, AnswerType> ProcessAnswer;
        public MsmqClient(MsmqProcessingParams msmqProcessingParams) : base(msmqProcessingParams.FormatterType)
        {
            // ������ ������������ ��������� ��� �������                                
            messages = new Dictionary<String,RequestType>();
            // �������� ������� ��� ������� ��������, ���� ��� �� ����������
            queueSend = MsmqTools.CreateQueue(msmqProcessingParams.QueueSendName, QueueType.Transactional);
            // �������� ������� ��� ������ �������, ���� ��� �����
            if (msmqProcessingParams.QueueReceiveName != null)
            {
                queueReceive = MsmqTools.CreateQueue(msmqProcessingParams.QueueReceiveName);         
                queueReceive.Formatter = answerFormatter;
                // ��������� �� ������� �������� CorrelationId
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
            // ���������� ���������� �� �������, ����������� ��� ��������� 
            // ��������� � �������            
            queueReceive.PeekCompleted += OnPeek;
            // ������ ������������ ����������� ��������� � �������
            queueReceive.BeginPeek();          
        }   
        // ���������� ����� ������� ������� 
        public void EndReceive()
        {
            if (queueReceive == null)
                throw new ApeekException("Cannot end receive. Receive Queue was not defined");
            if (ProcessAnswer == null)
                throw new ApeekException("Cannot end receive. Process Answer event was not defined");
            // ��������� ���������� 
            queueReceive.PeekCompleted -= OnPeek;
        }
        public bool Send(RequestType request)
        {
            try
            {
                // �������� ������ ���������
                Message message = new Message(request, requestFormatter);      
                message.ResponseQueue = queueReceive;           
                // ������������� ���������������� ���������
                message.Recoverable = Recoverable;
                // ������� ���������; ��������� ���������� ������� �� 
                // ������������ ��������, ������ �������-���������� ������������
                // �������� MessageQueueTransactionType.Single
                queueSend.Send(message, MessageQueueTransactionType.Single);
                if (queueReceive != null)
                {
                    // ���� message.Id ��������������� ����� ������� ���������;
                    // ������������� ��������� ����������� c ���������� ��������
                    // � ������ ������������� ��������
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
            // �������� ���������� ���������� MSMQ 
            MessageQueueTransaction transaction = new MessageQueueTransaction();
            // ������ ����������
            transaction.Begin();
            try
            {
                // ���������� �������� ��������� � ������� 
                queueReceive.EndPeek(asyncResult.AsyncResult);
                // �������� ��������� �� ������� � ������ ����������
                Message message = queueReceive.Receive(transaction);
                // � ���� CorrelationId ������ ���� ������������� ���������
                // � �������� ��������
                String messageId = message.CorrelationId;
                // ���� �� ����� ��������� � ������ ������������� ��������?
                if (messages.ContainsKey(messageId))
                {
                    if (message.Body is AnswerType)
                    {
                        // ������������� ���� ��������� � ���� ������
                        // � ������� ������� �� ��� ���������                         
                        AnswerType answer = (AnswerType)message.Body;
                        ProcessAnswer(this, messages[messageId], answer);
                    };
                    messages.Remove(messageId);
                }
                // ���������� ������� ���������
                BeginReceive();
                // �������� ���������� ����������
                transaction.Commit();
            }
            catch (Exception e)
            {
                // ������ ����������
                transaction.Abort();
                throw e;
            }
        }
    }
}