using System.Net;
using System.Web;
namespace Apeek.Sms.Implementations.Turbosms.Service
{
    /// <summary>
    ///     ����� ��� ������ � ��������
    /// </summary>
    internal class SMSWorker : global::Service
    {
        // ���� ������ ��� �������� �������� �������
        private const string SessionKey = "SMSWorker_Instance";
        // �����������
        public SMSWorker()
        {
            this.CookieContainer = new CookieContainer();
        }
        public static SMSWorker GetInstance()
        {
            if (HttpContext.Current == null)
                return new SMSWorker();
            // ����������, ���������� �� ������ � ������
            SMSWorker worker = HttpContext.Current.Session[SessionKey] as SMSWorker;
            if (worker == null)
            {
                // ������� ������
                worker = new SMSWorker();
                // �������� � ������
                HttpContext.Current.Session[SessionKey] = worker;
            }
            return worker;
        }
        /// <summary>
        ///    �������� ������ � ������ (���� �����)
        /// </summary>
        public void CloseSession()
        {
            if (HttpContext.Current != null)
                HttpContext.Current.Session[SessionKey] = null;
        }
    }
}
