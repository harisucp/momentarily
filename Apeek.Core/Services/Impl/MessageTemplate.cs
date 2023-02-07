using System.IO;
using Apeek.Common;
using Apeek.Common.Configuration;
namespace Apeek.Core.Services.Impl
{
    public class MessageTemplate
    {
        public static string NotificationServiceChangedTemlp = "notification_service_changed";
        public static string VerifySecuritySataTemlp = "verify_security_data";
        public static string AdminNotificationNewClientReviewTeml = "admin_notification_new_client_review";
        public static string NotificationServiceChangedTemlpPath = string.Format(@"{0}\mails\{1}\{2}.cshtml", AppSettings.GetInstance().AppdataDirectory, Language.ru, NotificationServiceChangedTemlp);
        public static string VerifySecuritySataTemlpPath = string.Format(@"{0}\mails\{1}\{2}.cshtml", AppSettings.GetInstance().AppdataDirectory, Language.en, VerifySecuritySataTemlp);
        public static string AdminNotificationNewClientReviewTemlpPath = string.Format(@"{0}\mails\{1}\{2}.cshtml", AppSettings.GetInstance().AppdataDirectory, Language.ru, AdminNotificationNewClientReviewTeml);
        public static string GetMailTemplate(Language lang, string templateName)
        {
            string path = string.Format(@"{0}\mails\{1}\{2}.cshtml", AppSettings.GetInstance().AppdataDirectory, lang, templateName);
            return File.ReadAllText(path);
        }
        public static string GetSmsTemplate(Language lang, string templateName)
        {
            string path = string.Format(@"{0}\sms\{1}\{2}.txt", AppSettings.GetInstance().AppdataDirectory, lang, templateName);
            return File.ReadAllText(path);
        }
    }
}