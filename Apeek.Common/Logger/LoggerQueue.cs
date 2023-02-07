using System.Collections.Generic;
namespace Apeek.Common.Logger
{
    public class LoggerQueue
    {
        public delegate void IsNewMessageDelegate();
        public static event IsNewMessageDelegate IsNewMessageEvent;
        public static Queue<string> _messageQueque;
        static LoggerQueue()
        {
            _messageQueque = new Queue<string>();
        }
        public static void Add(string message)
        {
            if(IsNewMessageEvent != null)
            {
                _messageQueque.Enqueue(message);
                IsNewMessageEvent();
            }
        }
        public static string Get()
        {
            if(_messageQueque.Count > 0)
            {
                return _messageQueque.Dequeue();   
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
