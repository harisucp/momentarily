using System.Net;
using System.Web;
namespace Apeek.Sms.Implementations.Turbosms.Service
{
    /// <summary>
    ///     Класс для работы с сервисом
    /// </summary>
    internal class SMSWorker : global::Service
    {
        // ключ сессии для хранения инстанса объекта
        private const string SessionKey = "SMSWorker_Instance";
        // конструктор
        public SMSWorker()
        {
            this.CookieContainer = new CookieContainer();
        }
        public static SMSWorker GetInstance()
        {
            if (HttpContext.Current == null)
                return new SMSWorker();
            // определяем, существует ли объект в сессии
            SMSWorker worker = HttpContext.Current.Session[SessionKey] as SMSWorker;
            if (worker == null)
            {
                // создаем объект
                worker = new SMSWorker();
                // помещаем в сессию
                HttpContext.Current.Session[SessionKey] = worker;
            }
            return worker;
        }
        /// <summary>
        ///    Обнуляем объект в сессии (если нужно)
        /// </summary>
        public void CloseSession()
        {
            if (HttpContext.Current != null)
                HttpContext.Current.Session[SessionKey] = null;
        }
    }
}
