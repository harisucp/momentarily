using System;
using System.Messaging;
namespace Apeek.Core.Services.Impl.MSMQ
{
    public static class MsmqTools
    {             
        static public MessageQueue CreateQueue(String queueName)
        {
            return CreateQueue(queueName, QueueType.Transactional);
        }
        // функция проверяет наличие очереди и создает ее при необходимости
        static public MessageQueue CreateQueue(String queueName, QueueType type)
        {
            MessageQueue messageQueue;            
            // если это частная очередь удаленного компьютера,
            // то при попытке проверки ее наличие возникает исключение
            try 
            {
                if (!MessageQueue.Exists(queueName))
                {
                    MessageQueue.Create(queueName, type == QueueType.Transactional);
                }
            }    
            catch{}                       
            messageQueue = new MessageQueue(queueName);            
            return messageQueue;
        }
    }
}