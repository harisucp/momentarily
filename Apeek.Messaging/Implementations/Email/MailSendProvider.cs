using System;
using System.Net.Mail;
using System.Text;
using Apeek.Messaging.Interfaces;
namespace Apeek.Messaging.Implementations.Email
{
    public class MailSendProvider : IMailSendProvider
    {
        private IMessage _message;
        private ISendProperty _sendProperty;
        private bool _authenticated;
        public bool SendMessage(IMessage message)
        {
            try
            {
                _message = message;
                if (_authenticated)
                {
                    ValidateEmailProperties();
                    return SendEmailUsingSmtp();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return false;
        }
        public bool Auth(ISendProperty sendProperty)
        {
            _sendProperty = sendProperty;
            ValidateSMTPProperties();
            _authenticated = true;
            return _authenticated;
        }
        private bool SendEmailUsingSmtp()
        {
            // Create a new SmtpClient for sending the email
            SmtpClient client = new SmtpClient();
            // Use the properties of the activity to construct a new MailMessage
            using (var message = new MailMessage())
            {
                message.From = new MailAddress(_message.FromHeader.From, _message.FromHeader.FromName);
                SetToAddresses(message);
                SetHeaders(message);
                if (!String.IsNullOrEmpty(_message.FromHeader.Subject))
                {
                    message.Subject = _message.FromHeader.Subject;
                    message.SubjectEncoding = Encoding.Unicode;
                }
                if (!String.IsNullOrEmpty(_message.Body))
                {
                    message.Body = _message.Body;
                }
                message.IsBodyHtml = _message.IsBodyHtml;
                // Set the SMTP host and send the mail
                SetSMTPSettings(client);
                client.Send(message);
            }
            return true;
        }
        private void SetHeaders(MailMessage message)
        {
            if (_message.FromHeader.Headers != null && _message.FromHeader.Headers.HasKeys())
            {
                message.Headers.Add(_message.FromHeader.Headers);
            }
        }
        private void SetToAddresses(MailMessage message)
        {
            string[] addresses = _message.To.Split(new char[] { ';' });
            foreach (string address in addresses)
            {
                message.To.Add(address);
            }
            string[] bccs = _message.Bcc.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string bcc in bccs)
            {
                message.Bcc.Add(bcc);
            }
        }
        private void SetSMTPSettings(SmtpClient client)
        {
            if (string.IsNullOrEmpty(_sendProperty.ProviderHost))
            {
                throw new Exception("Smtp host is not set");
            }
            client.Host = _sendProperty.ProviderHost;
            if (_sendProperty.ProviderPort != null)
            {
                client.Port = (int)_sendProperty.ProviderPort;
            }
            client.EnableSsl = true;
            client.UseDefaultCredentials = true;
            if ((!String.IsNullOrEmpty(_sendProperty.Login)) && (!String.IsNullOrEmpty(_sendProperty.Pwd)))
            {
                client.Credentials = new System.Net.NetworkCredential(_sendProperty.Login, _sendProperty.Pwd);
            }
        }
        private void ValidateEmailProperties()
        {
            ValidateEmailAddress(_message.To, "To");
            ValidateEmailAddress(_message.FromHeader.From, "From");
        }
        private static void ValidateEmailAddress(string emailAddress, string mailTypeDirection)
        {
            if (string.IsNullOrEmpty(emailAddress))
            {
                throw new Exception(string.Format("{0} e-mail address is not set", mailTypeDirection));
            }
            try
            {
                new MailAddress(emailAddress);
            }
            catch
            {
                throw new Exception(string.Format("Invalid {0} e-mail address", mailTypeDirection));
            }
        }
        private void ValidateSMTPProperties()
        {
            if (string.IsNullOrEmpty(_sendProperty.ProviderHost))
            {
                throw new Exception("SmtpHost is not set");
            }
            if (_sendProperty.ProviderPort == 0)
            {
                throw new Exception("Port is not set");
            }
            else if (_sendProperty.ProviderPort < 1)
            {
                throw new Exception("Invalid Port Number");
            }
            if (string.IsNullOrEmpty(_sendProperty.Pwd))
            {
                throw new Exception("Password is not set");
            }
            if (string.IsNullOrEmpty(_sendProperty.Login))
            {
                throw new Exception("UserName is not set");
            }
        }
    }
}
