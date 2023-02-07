using System;
using System.Collections.Generic;
using Apeek.Common;
using Apeek.Common.Configuration;
using Apeek.Common.Controllers;
using Apeek.Common.Logger;
using Apeek.Common.Models;
using Apeek.Core.Interfaces;
using Apeek.Core.RazorRenderEngine;
using Apeek.Entities.Entities;
using Apeek.Messaging.Implementations;
using Apeek.Messaging.Interfaces;
using Apeek.ViewModels.Models;
using Apeek.ViewModels.Models.Impl;
using System.Configuration;
using System.Web;
using Apeek.Common.HttpContextImpl;
using System.IO;
using System.Text.RegularExpressions;
using Momentarily.ViewModels.Models;

namespace Apeek.Core.Services.Impl
{
    public class SendMessageService : ISendMessageService
    {
        const string phPersonId = "{person-id}";
        const string phPersonName = "{person-name}";
        const string phLocation = "{location}";
        const string phServices = "{services}";

        private FromHeader _fromHeader;
        public FromHeader FromHeader { get { return _fromHeader; } set { _fromHeader = value; } }
        public SendMessageService()
        {
            Settings = new SettingsDataService();
        }
        //public SendMessageService(FromHeader fromHeader) : this()
        //{
        //    _fromHeader = fromHeader;
        //}
        public bool SendVia(IMessageSendProvider messageSendProvider, IMessage message, ISendProperty sendProperty)
        {
            if (messageSendProvider.Auth(sendProperty))
            {
                return messageSendProvider.SendMessage(message);
            }
            return false;
        }
        private ISettingsDataService Settings { get; set; }
        public bool SendEmailAccountActivationMessage(User user, List<Address> addresses, string verificationUrl)
        {
            const string phVerificationUrl = "{verification-url}";
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            try
            {
                var t = Ioc.Get<ITranslateController>();
                var message = new Message();
                if (_fromHeader != null)
                    message.FromHeader = _fromHeader;
                else
                {
                    message.FromHeader = new FromHeader()
                    {
                        From = Settings.GetSmtpFrom(),
                        FromName = Settings.GetSmtpFromName(),
                        Subject = string.Format("{0}", t["momentarily account confirmation"])
                    };
                }
                message.To = user.Email;
                //message.Bcc = Settings.GetEmailAdmin();
                message.Body = MessageTemplate.GetMailTemplate(LanguageController.CurLang, "account_activation");
                message.Body = message.Body.Replace(phVerificationUrl, verificationUrl);
                message.Body = message.Body.Replace("my_hostings", domainName);
                message.Body = message.Body.Replace(phPersonName, user.FullName);
                if (addresses[0].LocationLang != null)
                    message.Body = message.Body.Replace(phLocation, addresses[0].LocationLang.Name);
                else
                    message.Body = message.Body.Replace(phLocation, "");
                message.IsBodyHtml = true;
                return SendVia(Ioc.Get<IMailSendProvider>(), message, new SettingsDataService().GetEmailSendProperty());
            }
            catch (Exception ex)
            {
                Ioc.Get<DbLogger>().LogException(LogSource.SendMail, string.Format("Error when sending verification message to person id: {0}; person email: {1}", user.Id, user.Email), ex);
            }
            return false;
        }
        //public bool SendEmailVerifySecurityDataMessage(UserSecurityDataChangeRequest request)
        //{
        //    try
        //    {
        //        string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
        //        var t = Ioc.Get<ITranslateController>();
        //        var message = new Message();
        //        if (_fromHeader != null)
        //            message.FromHeader = _fromHeader;
        //        else
        //        {
        //            message.FromHeader = new FromHeader()
        //            {
        //                From = Settings.GetSmtpFrom(),
        //                FromName = Settings.GetSmtpFromName(),
        //                Subject = string.Format("{0}", t["Email change confirmation"])
        //            };
        //        }
        //        message.To = !string.IsNullOrWhiteSpace(request.OldValue) ? request.OldValue : request.NewValue;
        //        //message.Bcc = Settings.GetEmailAdmin();
        //        var renderEngine = Ioc.Get<IRazorEngine>();
        //        message.Body = renderEngine.Render(request);
        //        message.Body = message.Body.Replace("my_hostings", domainName);
        //        message.IsBodyHtml = true;
        //        return SendVia(Ioc.Get<IMailSendProvider>(), message, new SettingsDataService().GetEmailSendProperty());
        //    }
        //    catch (Exception ex)
        //    {
        //        Ioc.Get<DbLogger>().LogException(LogSource.SendMail, string.Format("Error when sending email verification security data message to user id: {0}; person email: {1}", request.UserId, request.OldValue), ex);
        //    }
        //    return false;
        //}

        public bool SendEmailVerifySecurityDataMessage(UserSecurityDataChangeRequest request, string username)
        {
            const string phEmail = "{user-email-new}";
            const string phname = "{user-name}";
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string currentYear = Convert.ToString(DateTime.Now.Year);
            string activationURL = domainName + "/User/VerifySecurityRequest?vc=" + request.VerificationCode;
            //string UnsubscribertURL = domainName + "/Account/NewsLetterUnSubscribe?Email=" + email;
            try
            {
                var t = Ioc.Get<ITranslateController>();
                var message = new Message();
                if (_fromHeader != null)
                    message.FromHeader = _fromHeader;
                else
                {
                    message.FromHeader = new FromHeader()
                    {
                        From = Settings.GetEmailHelp(),
                        FromName = Settings.GetSmtpFromName(),
                        Subject = string.Format("{0}", t["Email change confirmation"])
                    };
                }
                message.To = !string.IsNullOrWhiteSpace(request.OldValue) ? request.OldValue : request.NewValue;
                //message.Bcc = Settings.GetEmailAdmin();
                message.Body = MessageTemplate.GetMailTemplate(LanguageController.CurLang, "change_email_under_userprofile");
                message.Body = message.Body.Replace(phEmail, request.NewValue);
                message.Body = message.Body.Replace("my_hostings", domainName);
                message.Body = message.Body.Replace("{Activation-URL}", activationURL);
                message.Body = message.Body.Replace(phname, username);
                message.Body = message.Body.Replace("{old-email}", request.OldValue);
                message.Body = message.Body.Replace("CurrentYear", currentYear);
                //message.Body = message.Body.Replace("UnsubscribeLink", UnsubscribertURL);

                message.IsBodyHtml = true;
                return SendVia(Ioc.Get<IMailSendProvider>(), message, new SettingsDataService().GetEmailSendPropertyComingHelp());
            }
            catch (Exception ex)
            {
                Ioc.Get<DbLogger>().LogException(LogSource.SendMail, string.Format("Error when sending email verification security data message to user id: {0}; person email: {1}", request.UserId, request.OldValue), ex);
            }
            return false;
        }

        public Message ProcessViaRenderEngine<T>(T model, string subject, string to)
        {
            var t = Ioc.Get<ITranslateController>();
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            var message = new Message();
            if (_fromHeader != null)
                message.FromHeader = _fromHeader;
            else
            {
                message.FromHeader = new FromHeader()
                {
                    From = Settings.GetSmtpFrom(),
                    FromName = Settings.GetSmtpFromName(),
                    Subject = string.Format("{0}", t[subject])
                };
            }
            message.To = to;
            var renderEngine = Ioc.Get<IRazorEngine>();
            message.Body = renderEngine.Render(model);
            message.Body = message.Body.Replace("my_hostings", domainName);
            message.IsBodyHtml = true;
            return message;
        }
        public bool SendEmailQuickAccountActivationMessage(string email, string verificationUrl, string firstname)
        {
            const string phVerificationUrl = "{verification-url}";
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string currentYear = Convert.ToString(DateTime.Now.Year);
            string UnsubscribertURL = domainName + "/Account/NewsLetterUnSubscribe?Email=" + email;
            try
            {
                var t = Ioc.Get<ITranslateController>();
                var message = new Message();
                if (_fromHeader != null)
                    message.FromHeader = _fromHeader;
                else
                {
                    message.FromHeader = new FromHeader()
                    {
                        From = Settings.GetSmtpFrom(),
                        FromName = Settings.GetSmtpFromName(),
                        Subject = string.Format("{0}", t["Account confirmation"])
                    };
                }
                message.To = email;
                //message.Bcc = Settings.GetEmailAdmin();
                //message.Bcc = "hello@momentarily.com";
                //message.Bcc = "princydhuparwins@gmail.com";
                message.Body = MessageTemplate.GetMailTemplate(LanguageController.CurLang, "account_activation_quick");
                message.Body = message.Body.Replace(phVerificationUrl, verificationUrl);
                message.Body = message.Body.Replace("my_hostings", domainName);
                message.Body = message.Body.Replace("firstname", firstname);
                message.Body = message.Body.Replace("CurrentYear", currentYear);
                message.Body = message.Body.Replace("UnsubscribeLink", UnsubscribertURL);
                message.IsBodyHtml = true;
                return SendVia(Ioc.Get<IMailSendProvider>(), message, new SettingsDataService().GetEmailSendProperty());
            }
            catch (Exception ex)
            {
                Ioc.Get<DbLogger>().LogException(LogSource.SendMail, string.Format("Error when sending verification message to email: {0}", email), ex);
            }
            return false;
        }


        public bool SendEmailBulkMailSendToUnknownMessage(string email, string verificationUrl, string serviceName)
        {
            const string phVerificationUrl = "{verification-url}";
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            const string phService = "{service}";
            try
            {
                var t = Ioc.Get<ITranslateController>();
                var message = new Message();
                if (_fromHeader != null)
                    message.FromHeader = _fromHeader;
                else
                {
                    message.FromHeader = new FromHeader()
                    {
                        From = Settings.GetSmtpFrom(),
                        FromName = Settings.GetSmtpFromName(),
                        Subject = t["Клиентов много не бывает"]
                    };
                }
                message.To = email;
                message.Body = MessageTemplate.GetMailTemplate(LanguageController.CurLang, "bulk_mail_send_to_unknown");
                message.Body = message.Body.Replace(phVerificationUrl, verificationUrl);
                message.Body = message.Body.Replace("my_hostings", domainName);
                message.Body = message.Body.Replace(phService, serviceName);
                message.IsBodyHtml = true;
                return SendVia(Ioc.Get<IMailSendProvider>(), message, new SettingsDataService().GetEmailSendProperty());
            }
            catch (Exception ex)
            {
                Ioc.Get<DbLogger>().LogException(LogSource.SendMail, string.Format("Error when sending verification message to email: {0}", email), ex);
            }
            return false;
        }
        public IMessage GetEmailLoginMessage(User user, string loginUrl)
        {
            const string phLoginUrl = "{login-url}";
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            var t = Ioc.Get<ITranslateController>();
            var message = new Message();
            if (_fromHeader != null)
                message.FromHeader = _fromHeader;
            else
            {
                message.FromHeader = new FromHeader()
                {
                    From = Settings.GetSmtpFrom(),
                    FromName = Settings.GetSmtpFromName(),
                    Subject = string.Format("{0}", t["Log in"])
                };
            }
            message.To = user.Email;
            //message.Bcc = Settings.GetEmailAdmin();
            message.Body = MessageTemplate.GetMailTemplate(LanguageController.CurLang, "user_login");
            message.Body = message.Body.Replace("my_hostings", domainName);
            message.Body = message.Body.Replace(phLoginUrl, loginUrl);
            message.IsBodyHtml = true;
            return message;
        }
        public IMessage GetEmailRestorePwdMessage(User user, string loginUrl)
        {
            const string phLoginUrl = "{login-url}";
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string UnsubscribertURL = domainName + "/Account/NewsLetterUnSubscribe?Email=" + user.Email;
            string currentYear = Convert.ToString(DateTime.Now.Year);
            const string phName = "{user-name}";
            var t = Ioc.Get<ITranslateController>();
            var message = new Message();
            if (_fromHeader != null)
                message.FromHeader = _fromHeader;
            else
            {
                message.FromHeader = new FromHeader()
                {
                    From = Settings.GetSmtpFrom(),
                    FromName = Settings.GetSmtpFromName(),
                    Subject = string.Format("{0}", t["Password reset request"])
                };
            }
            message.To = user.Email;
            //message.Bcc = Settings.GetEmailAdmin();
            message.Body = MessageTemplate.GetMailTemplate(LanguageController.CurLang, "user_sms_restore_pwd");
            message.Body = message.Body.Replace(phLoginUrl, loginUrl);
            message.Body = message.Body.Replace("my_hostings", domainName);
            message.Body = message.Body.Replace(phName, user.FirstName);
            message.Body = message.Body.Replace("CurrentYear", currentYear);
            message.Body = message.Body.Replace("UnsubscribeLink", UnsubscribertURL);
            message.IsBodyHtml = true;
            return message;
        }
        public IMessage GetSmsRestorePwdMessage(string phoneNumber, string tempPwd)
        {
            const string phTempPwd = "{temp-pwd}";
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            var message = new Message();
            if (_fromHeader != null)
                message.FromHeader = _fromHeader;
            else
            {
                message.FromHeader = new FromHeader()
                {
                    From = Settings.GetSmsFrom(),
                };
            }
            message.To = phoneNumber;
            message.Body = MessageTemplate.GetMailTemplate(LanguageController.CurLang, "user_sms_restore_pwd");
            message.Body = message.Body.Replace(phTempPwd, tempPwd);
            message.Body = message.Body.Replace("my_hostings", domainName);
            return message;
        }
        public bool SendSmsUserInvitationMessage(string phoneNumberTo, string userName, string userLogin, string userPwd)
        {
            const string phTUserName = "{user_name}";
            const string phUserLogin = "{user_login}";
            const string phUserPwd = "{user_pwd}";
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            var message = new Message();
            if (_fromHeader != null)
                message.FromHeader = _fromHeader;
            else
            {
                message.FromHeader = new FromHeader()
                {
                    From = Settings.GetSmsFrom(),
                };
            }
            message.To = phoneNumberTo;
            message.Body = MessageTemplate.GetSmsTemplate(LanguageController.CurLang, "user_invitation");
            message.Body = message.Body.Replace(phTUserName, userName);
            message.Body = message.Body.Replace(phUserLogin, userLogin);
            message.Body = message.Body.Replace(phUserPwd, userPwd);
            message.Body = message.Body.Replace("my_hostings", domainName);
            return SendVia(Ioc.Get<ISmsSendProvider>(), message, Settings.GetSmsSendPropertyAlt());
        }
        public bool SendEmailContactUs(ContactUsEntry contactUsEntry)
        {
            const string phFromEmail = "{from-email}";
            const string phFromName = "{from-name}";
            const string phSubject = "{subject}";
            const string phMessage = "{message}";
            const string phIssuesFacing = "{issuesfacing}";
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string currentYear = Convert.ToString(DateTime.Now.Year);
            string UnsubscribertURL = domainName + "/Account/NewsLetterUnSubscribe?Email=" + contactUsEntry.EmailAddress;
            try
            {
                var message = new Message
                {
                    FromHeader = new FromHeader()
                    {
                        //From = contactUsEntry.EmailAddress,
                        From = Settings.GetEmailHelp(),
                        Subject = "Support Ticket",
                    },
                    //To = Settings.GetEmailContactUs(),
                    To = "help@momentarily.com",
                    Body = MessageTemplate.GetMailTemplate(LanguageController.CurLang, "contact_us"),
                    IsBodyHtml = true
                };
                message.Body = message.Body.Replace(phFromEmail, contactUsEntry.EmailAddress);
                message.Body = message.Body.Replace(phFromName, contactUsEntry.Name);
                message.Body = message.Body.Replace(phMessage, contactUsEntry.Message.Replace("\r", "<br>"));
                message.Body = message.Body.Replace(phSubject, contactUsEntry.Subject);
                message.Body = message.Body.Replace("my_hostings", domainName);
                message.Body = message.Body.Replace(phIssuesFacing, contactUsEntry.Issuesfacing);
                message.Body = message.Body.Replace("CurrentYear", currentYear);
                message.Body = message.Body.Replace("UnsubscribeLink", UnsubscribertURL);
                return SendVia(Ioc.Get<IMailSendProvider>(), message, Settings.GetEmailSendProperty());
            }
            catch (Exception ex)
            {
                Ioc.Get<DbLogger>().LogException(LogSource.SendMail, string.Format("Error when sending contact us email. From name {0}; From email: {1}; Message: {2}.", contactUsEntry.Name, contactUsEntry.EmailAddress, contactUsEntry.Message), ex);
            }
            return false;
        }

        public bool SendEmailContactUsForUser(ContactUsEntry contactUsEntry)
        {
            const string phFromEmail = "{from-email}";
            const string phFromName = "{from-name}";
            const string phSubject = "{subject}";
            const string phMessage = "{message}";
            const string phIssuesFacing = "{issuesfacing}";
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string currentYear = Convert.ToString(DateTime.Now.Year);
            string UnsubscribertURL = domainName + "/Account/NewsLetterUnSubscribe?Email=" + contactUsEntry.EmailAddress;
            try
            {
                var message = new Message
                {
                    FromHeader = new FromHeader()
                    {
                        //From = contactUsEntry.EmailAddress,
                        From = Settings.GetEmailHelp(),
                        Subject = "Support Ticket",
                    },
                    //To = Settings.GetEmailContactUs(),
                    To = contactUsEntry.EmailAddress,
                    Body = MessageTemplate.GetMailTemplate(LanguageController.CurLang, "contact_us"),
                    IsBodyHtml = true
                };
                message.Body = message.Body.Replace(phFromEmail, contactUsEntry.EmailAddress);
                message.Body = message.Body.Replace(phFromName, contactUsEntry.Name);
                message.Body = message.Body.Replace(phMessage, contactUsEntry.Message.Replace("\r", "<br>"));
                message.Body = message.Body.Replace(phSubject, contactUsEntry.Subject);
                message.Body = message.Body.Replace("my_hostings", domainName);
                message.Body = message.Body.Replace(phIssuesFacing, contactUsEntry.Issuesfacing);
                message.Body = message.Body.Replace("CurrentYear", currentYear);
                message.Body = message.Body.Replace("UnsubscribeLink", UnsubscribertURL);
                return SendVia(Ioc.Get<IMailSendProvider>(), message, Settings.GetEmailSendProperty());
            }
            catch (Exception ex)
            {
                Ioc.Get<DbLogger>().LogException(LogSource.SendMail, string.Format("Error when sending contact us email. From name {0}; From email: {1}; Message: {2}.", contactUsEntry.Name, contactUsEntry.EmailAddress, contactUsEntry.Message), ex);
            }
            return false;
        }

        public void SendAdminNotificationEmail_Errors(string messageBody, string logSource)
        {
            if (AppSettings.GetInstance().SendErrorsToAdminEmail)
            {
                string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
                var message = new Message();
                if (_fromHeader != null)
                    message.FromHeader = _fromHeader;
                else
                {
                    message.FromHeader = new FromHeader()
                    {
                        From = Settings.GetSmtpFrom(),
                        Subject = string.Format("Tophands - ERROR in {0}", logSource),
                    };
                }
                message.To = Settings.GetEmailAdmin();
                //message.To = Settings.GetEmailContactUs();
                message.Body = messageBody;
                message.Body = message.Body.Replace("my_hostings", domainName);
                message.IsBodyHtml = false;
                SendVia(Ioc.Get<IMailSendProvider>(), message, Settings.GetEmailSendProperty());
            }
        }
        public bool SendEmailChangePasswordMessage(string email, string firstname)
        {
            const string phEmail = "{user-email}";
            const string phname = "{user-name}";
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string currentYear = Convert.ToString(DateTime.Now.Year);
            string UnsubscribertURL = domainName + "/Account/NewsLetterUnSubscribe?Email=" + email;
            try
            {
                var t = Ioc.Get<ITranslateController>();
                var message = new Message();
                if (_fromHeader != null)
                    message.FromHeader = _fromHeader;
                else
                {
                    message.FromHeader = new FromHeader()
                    {
                        From = Settings.GetEmailHelp(),
                        FromName = Settings.GetSmtpFromName(),
                        Subject = string.Format("{0}", t["Your password has been changed"])
                    };
                }
                message.To = email;
                //message.Bcc = Settings.GetEmailAdmin();
                message.Body = MessageTemplate.GetMailTemplate(LanguageController.CurLang, "change_password");
                message.Body = message.Body.Replace(phEmail, email);
                message.Body = message.Body.Replace("my_hostings", domainName);
                message.Body = message.Body.Replace(phname, firstname);
                message.Body = message.Body.Replace("CurrentYear", currentYear);
                message.Body = message.Body.Replace("UnsubscribeLink", UnsubscribertURL);

                message.IsBodyHtml = true;
                return SendVia(Ioc.Get<IMailSendProvider>(), message, new SettingsDataService().GetEmailSendPropertyComingHelp());
            }
            catch (Exception ex)
            {
                Ioc.Get<DbLogger>().LogException(LogSource.SendMail, string.Format("Error when sending change password message to email: {0}", email), ex);
            }
            return false;
        }
        public bool SendEmailChangeEmailMessage(string email, string activationUrl)
        {
            const string phEmail = "{user-email}";
            const string phActivationUrl = "{activation-url}";
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            try
            {
                var t = Ioc.Get<ITranslateController>();
                var message = new Message();
                if (_fromHeader != null)
                    message.FromHeader = _fromHeader;
                else
                {
                    message.FromHeader = new FromHeader()
                    {
                        From = Settings.GetSmtpFrom(),
                        FromName = Settings.GetSmtpFromName(),
                        Subject = string.Format("{0}", t["Your email has been changed"])
                    };
                }
                message.To = email;
                //message.Bcc = Settings.GetEmailAdmin();
                message.Body = MessageTemplate.GetMailTemplate(LanguageController.CurLang, "change_email");
                message.Body = message.Body.Replace(phEmail, email);
                message.Body = message.Body.Replace("my_hostings", domainName);
                message.Body = message.Body.Replace(phActivationUrl, activationUrl);
                message.IsBodyHtml = true;
                return SendVia(Ioc.Get<IMailSendProvider>(), message, new SettingsDataService().GetEmailSendProperty());
            }
            catch (Exception ex)
            {
                Ioc.Get<DbLogger>().LogException(LogSource.SendMail, string.Format("Error when sending change email message to email: {0}", email), ex);
            }
            return false;
        }
        public bool SendEmailFollowUpMessage(string email, string firstname)
        {
            try
            {
                var t = Ioc.Get<ITranslateController>();
                string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
                var message = new Message();
                if (_fromHeader != null)
                    message.FromHeader = _fromHeader;
                else
                {
                    message.FromHeader = new FromHeader()
                    {
                        From = Settings.GetEmailMarketing(),
                        FromName = Settings.GetSmtpFromName(),
                        Subject = string.Format("{0}", t["Welcome to momentarily"])
                    };
                }
                message.To = email;
                //message.Bcc = Settings.GetEmailAdmin();
                message.Body = MessageTemplate.GetMailTemplate(LanguageController.CurLang, "follow_up");
                message.Body = message.Body.Replace("firstname", firstname);
                message.Body = message.Body.Replace("my_hostings", domainName);
                message.IsBodyHtml = true;
                return SendVia(Ioc.Get<IMailSendProvider>(), message, new SettingsDataService().GetEmailSendPropertyForMarketing());
            }
            catch (Exception ex)
            {
                Ioc.Get<DbLogger>().LogException(LogSource.SendMail, string.Format("Error when sending follow up message to email: {0}", email), ex);
            }
            return false;
        }
        public bool SendEmailGetMessageMessage(string email, string userFrom, string linkToMessage)
        {
            const string phUserFrom = "{user-from}";
            const string phLinkToMessage = "{link-to-message}";
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            try
            {
                var t = Ioc.Get<ITranslateController>();
                var message = new Message();
                if (_fromHeader != null)
                    message.FromHeader = _fromHeader;
                else
                {
                    message.FromHeader = new FromHeader()
                    {
                        From = Settings.GetSmtpFrom(),
                        FromName = Settings.GetSmtpFromName(),
                        Subject = string.Format("{0}", t["You've received a new message"])
                    };
                }
                message.To = email;
                //message.Bcc = Settings.GetEmailAdmin();
                message.Body = MessageTemplate.GetMailTemplate(LanguageController.CurLang, "get_message");
                message.Body = message.Body.Replace(phUserFrom, userFrom);
                message.Body = message.Body.Replace("my_hostings", domainName);
                message.Body = message.Body.Replace(phLinkToMessage, linkToMessage);
                message.IsBodyHtml = true;
                return SendVia(Ioc.Get<IMailSendProvider>(), message, new SettingsDataService().GetEmailSendProperty());
            }
            catch (Exception ex)
            {
                Ioc.Get<DbLogger>().LogException(LogSource.SendMail, string.Format("Error when sending get message message to email: {0}", email), ex);
            }
            return false;
        }
        public bool SendEmailGetBookingMessage(string email, string userFrom, string listing, string linkToRequest)
        {
            const string phUserFrom = "{user-from}";
            const string phListing = "{listing}";
            const string phLinkToRequest = "{link-to-request}";
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            try
            {
                var t = Ioc.Get<ITranslateController>();
                var message = new Message();
                if (_fromHeader != null)
                    message.FromHeader = _fromHeader;
                else
                {
                    message.FromHeader = new FromHeader()
                    {
                        From = Settings.GetEmailRental(),
                        FromName = Settings.GetSmtpFromName(),
                        Subject = string.Format("{0}", t["You've received a new booking request"])
                    };
                }
                message.To = email;
                //message.Bcc = Settings.GetEmailRental();
                message.Body = MessageTemplate.GetMailTemplate(LanguageController.CurLang, "get_booking");
                message.Body = message.Body.Replace(phUserFrom, userFrom);
                message.Body = message.Body.Replace(phListing, listing);
                message.Body = message.Body.Replace(phLinkToRequest, linkToRequest);
                message.Body = message.Body.Replace("my_hostings", domainName);
                message.IsBodyHtml = true;
                return SendVia(Ioc.Get<IMailSendProvider>(), message, new SettingsDataService().GetEmailSendPropertyForRental());
            }
            catch (Exception ex)
            {
                Ioc.Get<DbLogger>().LogException(LogSource.SendMail, string.Format("Error when sending get booking message to email: {0}", email), ex);
            }
            return false;
        }
        public bool SendEmailApproveBookingMessage(string email, string listing, string linkToRequest, string linkToPay, string userName)
        {
            const string phListing = "{listing}";
            const string phLinkToRequest = "{link-to-request}";
            const string phLinkToPay = "{link-to-pay}";
            const string phLinkToDeposit = "{link-to-deposit}";
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string currentYear = Convert.ToString(DateTime.Now.Year);
            string UnsubscribertURL = domainName + "/Account/NewsLetterUnSubscribe?Email=" + email;
            try
            {
                var t = Ioc.Get<ITranslateController>();
                var message = new Message();
                if (_fromHeader != null)
                    message.FromHeader = _fromHeader;
                else
                {
                    message.FromHeader = new FromHeader()
                    {
                        From = Settings.GetEmailRental(),
                        FromName = Settings.GetSmtpFromName(),
                        Subject = string.Format("{0}", t["Your booking request has been approved"])
                    };
                }
                message.To = email;
                //message.Bcc = Settings.GetEmailRental();
                message.Body = MessageTemplate.GetMailTemplate(LanguageController.CurLang, "approve_booking_new");
                message.Body = message.Body.Replace(phListing, listing);
                message.Body = message.Body.Replace(phLinkToRequest, linkToRequest);
                message.Body = message.Body.Replace(phLinkToPay, linkToPay);
                message.Body = message.Body.Replace("my_hostings", domainName);
                message.Body = message.Body.Replace("CurrentYear", currentYear);
                message.Body = message.Body.Replace("UnsubscribeLink", UnsubscribertURL);
                message.Body = message.Body.Replace("{user-name}", userName);
                //message.Body = message.Body.Replace(phLinkToDeposit, linkToDeposit);
                message.IsBodyHtml = true;
                return SendVia(Ioc.Get<IMailSendProvider>(), message, new SettingsDataService().GetEmailSendPropertyForRental());
            }
            catch (Exception ex)
            {
                Ioc.Get<DbLogger>().LogException(LogSource.SendMail, string.Format("Error when sending approve booking message to email: {0}", email), ex);
            }
            return false;
        }
        public bool SendEmailDeclineBookingMessage(string email, string listing, string linkToRequest, string userName)
        {
            const string phListing = "{listing}";
            const string phLinkToRequest = "{link-to-request}";
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string currentYear = Convert.ToString(DateTime.Now.Year);
            string UnsubscribertURL = domainName + "/Account/NewsLetterUnSubscribe?Email=" + email;
            try
            {
                var t = Ioc.Get<ITranslateController>();
                var message = new Message();
                if (_fromHeader != null)
                    message.FromHeader = _fromHeader;
                else
                {
                    message.FromHeader = new FromHeader()
                    {
                        From = Settings.GetEmailRental(),
                        FromName = Settings.GetSmtpFromName(),
                        Subject = string.Format("{0}", t["Your booking request has been declined"])
                    };
                }
                message.To = email;
                //message.Bcc = Settings.GetEmailRental();
                message.Body = MessageTemplate.GetMailTemplate(LanguageController.CurLang, "decline_booking_new");
                message.Body = message.Body.Replace(phListing, listing);
                message.Body = message.Body.Replace("my_hostings", domainName);
                message.Body = message.Body.Replace(phLinkToRequest, linkToRequest);
                message.Body = message.Body.Replace("CurrentYear", currentYear);
                message.Body = message.Body.Replace("UnsubscribeLink", UnsubscribertURL);
                message.Body = message.Body.Replace("{user-name}", userName);
                message.IsBodyHtml = true;
                return SendVia(Ioc.Get<IMailSendProvider>(), message, new SettingsDataService().GetEmailSendPropertyForRental());
            }
            catch (Exception ex)
            {
                Ioc.Get<DbLogger>().LogException(LogSource.SendMail, string.Format("Error when sending decline booking message to email: {0}", email), ex);
            }
            return false;
        }
        public bool SendEmailPayRequestMessage(string email, string listing, string linkToRequest)
        {
            const string phListing = "{listing}";
            const string phLinkToRequest = "{link-to-request}";
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            try
            {
                var t = Ioc.Get<ITranslateController>();
                var message = new Message();
                if (_fromHeader != null)
                    message.FromHeader = _fromHeader;
                else
                {
                    message.FromHeader = new FromHeader()
                    {
                        From = Settings.GetEmailRental(),
                        FromName = Settings.GetSmtpFromName(),
                        Subject = string.Format("{0}", t["Payment received"])
                    };
                }
                message.To = email;
                //message.Bcc = Settings.GetEmailRental();
                message.Body = MessageTemplate.GetMailTemplate(LanguageController.CurLang, "pay_booking");
                message.Body = message.Body.Replace(phListing, listing);
                message.Body = message.Body.Replace("my_hostings", domainName);
                message.Body = message.Body.Replace(phLinkToRequest, linkToRequest);
                message.IsBodyHtml = true;
                return SendVia(Ioc.Get<IMailSendProvider>(), message, new SettingsDataService().GetEmailSendPropertyForRental());
            }
            catch (Exception ex)
            {
                Ioc.Get<DbLogger>().LogException(LogSource.SendMail, string.Format("Error when sending pay request message to email: {0}", email), ex);
            }
            return false;
        }
        public bool SendEmailDepositRequestMessage(string email, string listing, string linkToRequest)
        {
            const string phListing = "{listing}";
            const string phLinkToRequest = "{link-to-request}";
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            try
            {
                var t = Ioc.Get<ITranslateController>();
                var message = new Message();
                if (_fromHeader != null)
                    message.FromHeader = _fromHeader;
                else
                {
                    message.FromHeader = new FromHeader()
                    {
                        From = Settings.GetEmailRental(),
                        FromName = Settings.GetSmtpFromName(),
                        Subject = string.Format("{0}", t["Deposit request"])
                    };
                }
                message.To = email;
                //message.Bcc = Settings.GetEmailRental();
                message.Body = MessageTemplate.GetMailTemplate(LanguageController.CurLang, "deposit_booking");
                message.Body = message.Body.Replace(phListing, listing);
                message.Body = message.Body.Replace("my_hostings", domainName);
                message.Body = message.Body.Replace(phLinkToRequest, linkToRequest);
                message.IsBodyHtml = true;
                return SendVia(Ioc.Get<IMailSendProvider>(), message, new SettingsDataService().GetEmailSendPropertyForRental());
            }
            catch (Exception ex)
            {
                Ioc.Get<DbLogger>().LogException(LogSource.SendMail, string.Format("Error when sending deposit request message to email: {0}", email), ex);
            }
            return false;
        }
        public bool SendEmailSharerStartDispute(string listing, string sahrerName, string disputeMessage, int requestId, string emailId)        {            const string phListing = "{listing}";
            //const string phLinkToRequest = "{link-to-request}";
            const string phSharerName = "{sharer-name}";            const string phDisputeMessage = "{message}";            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);            string currentYear = Convert.ToString(DateTime.Now.Year);            try            {                var t = Ioc.Get<ITranslateController>();                var message = new Message();                if (_fromHeader != null)                    message.FromHeader = _fromHeader;                else                {                    message.FromHeader = new FromHeader()                    {                        From = Settings.GetEmailRental(),                        FromName = Settings.GetSmtpFromName(),                        Subject = string.Format("{0}", t["Dispute submitted"])                    };                }
                //TODO: for debug using
                //message.To = "dkosyak@gmail.com";
                message.To = emailId;
                //message.Bcc = Settings.GetEmailRental();
                message.Body = MessageTemplate.GetMailTemplate(LanguageController.CurLang, "sharer_dispute_submitted");                message.Body = message.Body.Replace(phListing, listing);                message.Body = message.Body.Replace(phSharerName, sahrerName);                message.Body = message.Body.Replace("$requestId$", Convert.ToString(requestId));                message.Body = message.Body.Replace("my_hostings", domainName);                message.Body = message.Body.Replace(phDisputeMessage, disputeMessage);                message.Body = message.Body.Replace("CurrentYear", currentYear);                message.IsBodyHtml = true;                return SendVia(Ioc.Get<IMailSendProvider>(), message, new SettingsDataService().GetEmailSendPropertyForRental());            }            catch (Exception ex)            {                Ioc.Get<DbLogger>().LogException(LogSource.SendMail, string.Format("Error when sending sharer dispute message from user : {0}", sahrerName), ex);            }            return false;        }
        public bool SendEmailSeekerStartDispute(string listing, int requestId, string seekerName, string disputeMessage, string emailId)
        {
            const string phListing = "{listing}";
            //const string phLinkToRequest = "{link-to-request}";
            const string phSeekerName = "{seeker-name}";
            const string phDisputeMessage = "{message}";
            const string phRequestId = "{requestId}";
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string currentYear = Convert.ToString(DateTime.Now.Year);
            try
            {
                var t = Ioc.Get<ITranslateController>();
                var message = new Message();
                if (_fromHeader != null)
                    message.FromHeader = _fromHeader;
                else
                {
                    message.FromHeader = new FromHeader()
                    {
                        From = Settings.GetEmailRental(),
                        FromName = Settings.GetSmtpFromName(),
                        Subject = string.Format("{0}", t["Dispute submitted"])
                    };
                }
                //TODO: for debug using
                //message.To = "dkosyak@gmail.com";
                message.To = emailId;
                //message.To = Settings.GetEmailRental();

                message.Body = MessageTemplate.GetMailTemplate(LanguageController.CurLang, "seeker_dispute_submitted");
                message.Body = message.Body.Replace(phListing, listing);
                message.Body = message.Body.Replace(phSeekerName, seekerName);
                message.Body = message.Body.Replace("my_hostings", domainName);
                message.Body = message.Body.Replace(phRequestId, requestId.ToString());
                message.Body = message.Body.Replace(phDisputeMessage, disputeMessage);
                message.Body = message.Body.Replace("CurrentYear", currentYear);
                message.IsBodyHtml = true;
                return SendVia(Ioc.Get<IMailSendProvider>(), message, new SettingsDataService().GetEmailSendPropertyForRental());
            }
            catch (Exception ex)
            {
                Ioc.Get<DbLogger>().LogException(LogSource.SendMail, string.Format("Error when sending seeker dispute message from user : {0}", seekerName), ex);
            }
            return false;
        }
        public bool SendConfirmEmailToSharer(string email, string listing, string startDate, string endDate, string price, string pickupLocation, UserContactInfo contactInfo, string linkToRequest)
        {
            const string phListing = "{listing}";
            const string phLinkToRequest = "{link-to-request}";
            const string phSeekerName = "{seeker-name}";
            const string phSeekerEmail = "{seeker-email}";
            const string phSeekerPhone = "{seeker-phone}";
            const string phStartDate = "{start-date}";
            const string phEndDate = "{end-date}";
            const string phTotalPrice = "{price}";
            const string phPickupLocation = "{pickup-location}";
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            //const string phRequestId = "{requestId}";
            try
            {
                var t = Ioc.Get<ITranslateController>();
                var message = new Message();
                if (_fromHeader != null)
                    message.FromHeader = _fromHeader;
                else
                {
                    message.FromHeader = new FromHeader()
                    {
                        From = Settings.GetEmailRental(),
                        FromName = Settings.GetSmtpFromName(),
                        Subject = string.Format("{0}", t["Reservation confirmed"])
                    };
                }
                //TODO: for debug using
                //message.To = "dkosyak@gmail.com";
                message.To = email;
                //message.Bcc = Settings.GetEmailRental();
                message.Body = MessageTemplate.GetMailTemplate(LanguageController.CurLang, "sharer_confirm_booking");
                message.Body = message.Body.Replace(phListing, listing);
                message.Body = message.Body.Replace(phLinkToRequest, linkToRequest);
                message.Body = message.Body.Replace(phStartDate, startDate);
                message.Body = message.Body.Replace(phEndDate, endDate);
                message.Body = message.Body.Replace(phTotalPrice, price);
                message.Body = message.Body.Replace(phSeekerName, contactInfo.Name);
                message.Body = message.Body.Replace(phSeekerEmail, contactInfo.Email);
                message.Body = message.Body.Replace(phSeekerPhone, contactInfo.Phone);
                message.Body = message.Body.Replace(phLinkToRequest, linkToRequest);
                message.Body = message.Body.Replace(phPickupLocation, pickupLocation);
                message.Body = message.Body.Replace("my_hostings", domainName);
                //message.Body = message.Body.Replace(phRequestId, requestId.ToString());
                //message.Body = message.Body.Replace(phDisputeMessage, disputeMessage);
                message.IsBodyHtml = true;
                return SendVia(Ioc.Get<IMailSendProvider>(), message, new SettingsDataService().GetEmailSendPropertyForRental());
            }
            catch (Exception ex)
            {
                Ioc.Get<DbLogger>().LogException(LogSource.SendMail, string.Format("Error when sending confirm email to user : {0}", email), ex);
            }
            return false;
        }
        public bool SendConfirmEmailToSeeker(string email, string listing, string startDate, string endDate, string bookingPrice, string serviceFee, string deposit, string totalPrice, string pickupLocation, UserContactInfo sharerInfo, string linkToRequest)
        {
            const string phListing = "{listing}";
            const string phLinkToRequest = "{link-to-request}";
            const string phSharerName = "{sharer-name}";
            const string phSharerEmail = "{sharer-email}";
            const string phSharerPhone = "{sharer-phone}";
            const string phBookingPrice = "{booking-price}";
            const string phServiceFee = "{service-fee}";
            const string phSecurityDeposit = "{secutiry-deposit}";
            const string phStartDate = "{start-date}";
            const string phEndDate = "{end-date}";
            const string phTotalPrice = "{price}";
            const string phPickupLocation = "{pickup-location}";
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            //const string phRequestId = "{requestId}";
            try
            {
                var t = Ioc.Get<ITranslateController>();
                var message = new Message();
                if (_fromHeader != null)
                    message.FromHeader = _fromHeader;
                else
                {
                    message.FromHeader = new FromHeader()
                    {
                        From = Settings.GetEmailRental(),
                        FromName = Settings.GetSmtpFromName(),
                        Subject = string.Format("{0}", t["Reservation confirmed"])
                    };
                }
                //TODO: for debug using
                //message.To = "dkosyak@gmail.com";
                message.To = email;
                //message.Bcc = Settings.GetEmailRental();
                message.Body = MessageTemplate.GetMailTemplate(LanguageController.CurLang, "seeker_confirm_booking");
                message.Body = message.Body.Replace(phListing, listing);
                message.Body = message.Body.Replace(phSharerName, sharerInfo.Name);
                message.Body = message.Body.Replace(phSharerEmail, sharerInfo.Email);
                message.Body = message.Body.Replace(phSharerPhone, sharerInfo.Phone);
                message.Body = message.Body.Replace(phBookingPrice, bookingPrice);
                message.Body = message.Body.Replace(phServiceFee, serviceFee);
                message.Body = message.Body.Replace(phSecurityDeposit, deposit);
                message.Body = message.Body.Replace(phStartDate, startDate);
                message.Body = message.Body.Replace(phEndDate, endDate);
                message.Body = message.Body.Replace(phTotalPrice, totalPrice);
                message.Body = message.Body.Replace(phPickupLocation, pickupLocation);
                message.Body = message.Body.Replace(phLinkToRequest, linkToRequest);
                message.Body = message.Body.Replace("my_hostings", domainName);
                //message.Body = message.Body.Replace(phRequestId, requestId.ToString());
                //message.Body = message.Body.Replace(phDisputeMessage, disputeMessage);
                message.IsBodyHtml = true;
                return SendVia(Ioc.Get<IMailSendProvider>(), message, new SettingsDataService().GetEmailSendPropertyForRental());
            }
            catch (Exception ex)
            {
                Ioc.Get<DbLogger>().LogException(LogSource.SendMail, string.Format("Error when sending confirm email to user : {0}", email), ex);
            }
            return false;
        }
        private bool SendEmail(string toEmail, Dictionary<string, object> replaceParameters, string subject, string templateName)
        {
            try
            {
                var t = Ioc.Get<ITranslateController>();
                string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
                var message = new Message();
                if (_fromHeader != null)
                    message.FromHeader = _fromHeader;
                else
                {
                    message.FromHeader = new FromHeader()
                    {
                        From = Settings.GetSmtpFrom(),
                        FromName = Settings.GetSmtpFromName(),
                        Subject = string.Format("{0}", t[subject])
                    };
                }
                //TODO: for debug using
                message.To = toEmail;
                //message.To = email;
                //message.Bcc = Settings.GetEmailAdmin();
                message.Body = MessageTemplate.GetMailTemplate(LanguageController.CurLang, templateName);
                foreach (var kv in replaceParameters)
                {
                    message.Body = message.Body.Replace(kv.Key, kv.Value.ToString());
                }
                message.Body = message.Body.Replace("my_hostings", domainName);
                message.IsBodyHtml = true;
                return SendVia(Ioc.Get<IMailSendProvider>(), message, new SettingsDataService().GetEmailSendProperty());
            }
            catch (Exception ex)
            {
                Ioc.Get<DbLogger>().LogException(LogSource.SendMail, string.Format("Error when sending review message to user : {0}", toEmail), ex);
            }
            return false;
        }
        public bool SendReviewMessageForSeeker(string email, string sharerName, string listing, string linkToReview)
        {
            const string phListing = "{listing}";
            const string phSharerName = "{sharer}";
            const string phLinkToReview = "{link-to-review}";
            var dictionary = new Dictionary<string, object>();
            dictionary.Add(phListing, listing);
            dictionary.Add(phSharerName, sharerName);
            dictionary.Add(phLinkToReview, linkToReview);
            return SendEmail(email, dictionary, "Write a review for " + sharerName, "seeker_review_booking");
        }
        public bool SendReviewMessageForSharer(string email, string seekerName, string listing, string linkToReview)
        {
            const string phListing = "{listing}";
            const string phSeekerName = "{seeker}";
            const string phLinkToReview = "{link-to-review}";
            var dictionary = new Dictionary<string, object>();
            dictionary.Add(phListing, listing);
            dictionary.Add(phSeekerName, seekerName);
            dictionary.Add(phLinkToReview, linkToReview);
            return SendEmail(email, dictionary, "Write a review for " + seekerName, "sharer_review_booking");
        }
        //public bool SendEmailSharerWroteReview(string email, string sharerName, string listing, string linkToReview)
        //{
        //    //const string phListing = "{listing}";
        //    const string phSharerName = "{sharer}";
        //    const string phLinkToReview = "{link-to-review}";
        //    var dictionary = new Dictionary<string, object>();
        //    //dictionary.Add(phListing, listing);
        //    dictionary.Add(phSharerName, sharerName);
        //    dictionary.Add(phLinkToReview, linkToReview);
        //    return SendEmail(email, dictionary, sharerName + " wrote you a review", "sharer_wrote_review");
        //}
        public bool SendEmailReviewFromSharer(string email, string sharerName, string listing, string linkToReview, int userId)
        {
            const string phSharerName = "{sharer}";
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string currentYear = Convert.ToString(DateTime.Now.Year);
            //string UnsubscribertURL = domainName + "/Account/NewsLetterUnSubscribe?Email=" + email;
            try
            {
                var t = Ioc.Get<ITranslateController>();
                var message = new Message();
                if (_fromHeader != null)
                    message.FromHeader = _fromHeader;
                else
                {
                    message.FromHeader = new FromHeader()
                    {
                        From = Settings.GetSmtpFrom(),
                        FromName = Settings.GetSmtpFromName(),
                        Subject = string.Format("{0}", t[sharerName + " wrote you a review"])
                    };
                }
                message.To = email;
                message.Body = MessageTemplate.GetMailTemplate(LanguageController.CurLang, "review_from_owner");

                message.Body = message.Body.Replace("my_hostings", domainName);
                message.Body = message.Body.Replace(phSharerName, sharerName);
                message.Body = message.Body.Replace("{userId}", Convert.ToString(userId));
                message.Body = message.Body.Replace("CurrentYear", currentYear);

                //message.Body = message.Body.Replace("UnsubscribeLink", UnsubscribertURL);
                message.IsBodyHtml = true;
                return SendVia(Ioc.Get<IMailSendProvider>(), message, new SettingsDataService().GetEmailSendProperty());
            }
            catch (Exception ex)
            {
                Ioc.Get<DbLogger>().LogException(LogSource.SendMail, string.Format("Error when sending verification message to email: {0}", email), ex);
            }
            return false;
        }
        //public bool SendEmailSeekerWroteReview(string email, string seekerName, string listing, string linkToReview)
        //{
        //    const string phSeekerName = "{seeker}";
        //    const string phLinkToReview = "{link-to-review}";
        //    var dictionary = new Dictionary<string, object>();
        //    //dictionary.Add(phListing, listing);
        //    dictionary.Add(phSeekerName, seekerName);
        //    dictionary.Add(phLinkToReview, linkToReview);
        //    return SendEmail(email, dictionary, seekerName + " wrote you a review", "seeker_wrote_review");
        //}

        public bool SendEmailReviewFromBorrower(string email, string seekerName, string listing, string linkToReview, int userId)
        {
            const string phSeekerName = "{seeker}";
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string currentYear = Convert.ToString(DateTime.Now.Year);
            //string UnsubscribertURL = domainName + "/Account/NewsLetterUnSubscribe?Email=" + email;
            try
            {
                var t = Ioc.Get<ITranslateController>();
                var message = new Message();
                if (_fromHeader != null)
                    message.FromHeader = _fromHeader;
                else
                {
                    message.FromHeader = new FromHeader()
                    {
                        From = Settings.GetSmtpFrom(),
                        FromName = Settings.GetSmtpFromName(),
                        Subject = string.Format("{0}", t[seekerName + " wrote you a review"])
                    };
                }
                message.To = email;
                message.Body = MessageTemplate.GetMailTemplate(LanguageController.CurLang, "review_from_borrower");

                message.Body = message.Body.Replace("my_hostings", domainName);
                message.Body = message.Body.Replace(phSeekerName, seekerName);
                message.Body = message.Body.Replace("{userId}", Convert.ToString(userId));
                message.Body = message.Body.Replace("CurrentYear", currentYear);
                //message.Body = message.Body.Replace("UnsubscribeLink", UnsubscribertURL);
                message.IsBodyHtml = true;
                return SendVia(Ioc.Get<IMailSendProvider>(), message, new SettingsDataService().GetEmailSendProperty());
            }
            catch (Exception ex)
            {
                Ioc.Get<DbLogger>().LogException(LogSource.SendMail, string.Format("Error when sending verification message to email: {0}", email), ex);
            }
            return false;
        }

        public bool SendEmailSeekerReadReview(string email, string sharerName, string text, string listing, string linkToReview)
        {
            const string phSharerName = "{sharer}";
            //const string phLinkToReview = "{link-to-review}";
            const string phText = "{text}";
            var dictionary = new Dictionary<string, object>();
            //dictionary.Add(phListing, listing);
            dictionary.Add(phSharerName, sharerName);
            //dictionary.Add(phLinkToReview, linkToReview);
            dictionary.Add(phText, text);
            return SendEmail(email, dictionary, String.Format("Read {0}'s review", sharerName), "seeker_read_review");
        }
        public bool SendEmailSharerReadReview(string email, string seekerName, string text, string listing, string linkToReview)
        {
            const string phSeekerName = "{seeker}";
            //const string phLinkToReview = "{link-to-review}";
            const string phText = "{text}";
            var dictionary = new Dictionary<string, object>();
            //dictionary.Add(phListing, listing);
            dictionary.Add(phSeekerName, seekerName);
            //dictionary.Add(phLinkToReview, linkToReview);
            dictionary.Add(phText, text);
            return SendEmail(email, dictionary, String.Format("Read {0}'s review", seekerName), "sharer_read_review");
        }
        public bool SendPaymentEmailTemplate(PayPal.Api.Payment payment, Result<GoodRequestViewModel> request, string url)
        {
            try
            {
                string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
                var address = payment.payer.payer_info.shipping_address.line1 + " " +
                   payment.payer.payer_info.shipping_address.line2 + ", " +
                   payment.payer.payer_info.shipping_address.city + ", " +
                   payment.payer.payer_info.shipping_address.state + ", " +
                   payment.payer.payer_info.shipping_address.postal_code;
                var t = Ioc.Get<ITranslateController>();
                var message = new Message();
                if (_fromHeader != null)
                    message.FromHeader = _fromHeader;
                else
                {
                    message.FromHeader = new FromHeader()
                    {
                        From = Settings.GetEmailRental(),
                        FromName = Settings.GetSmtpFromName(),
                        Subject = string.Format("{0}", t["Payment detail"])
                    };
                }
                message.To = request.Obj.UserEmail;
                //message.Bcc = Settings.GetEmailRental();
                message.Body = MessageTemplate.GetMailTemplate(LanguageController.CurLang, "invoice");
                message.Body = message.Body.Replace("order", payment.transactions[0].invoice_number);
                message.Body = message.Body.Replace("dated", DateTime.Now.ToShortDateString());
                message.Body = message.Body.Replace("itemname", payment.transactions[0].item_list.items[0].name);
                message.Body = message.Body.Replace("dayprice", Convert.ToString(request.Obj.Price));
                message.Body = message.Body.Replace("days", Convert.ToString(request.Obj.Days));
                message.Body = message.Body.Replace("daycost", Convert.ToString(request.Obj.DaysCost));
                message.Body = message.Body.Replace("deliverprice", Convert.ToString(request.Obj.DiliveryPrice));
                message.Body = message.Body.Replace("deliverrate", Convert.ToString(request.Obj.DiliveryCost));
                message.Body = message.Body.Replace("servicefee", Convert.ToString(request.Obj.CustomerServiceFeeCost + request.Obj.CustomerCharityCost));
                message.Body = message.Body.Replace("customercost", Convert.ToString(request.Obj.CustomerCost));
                message.Body = message.Body.Replace("startday", request.Obj.StartDate.DayOfWeek.ToString());
                message.Body = message.Body.Replace("startdate", request.Obj.StartDate.ToShortDateString());
                message.Body = message.Body.Replace("starttime", request.Obj.StartDate.TimeOfDay.ToString());
                message.Body = message.Body.Replace("endday", request.Obj.EndDate.DayOfWeek.ToString());
                message.Body = message.Body.Replace("enddate", request.Obj.EndDate.ToShortDateString());
                message.Body = message.Body.Replace("endtime", request.Obj.EndDate.TimeOfDay.ToString());
                message.Body = message.Body.Replace("pfirst", payment.payer.payer_info.first_name);
                message.Body = message.Body.Replace("plast", payment.payer.payer_info.last_name);
                message.Body = message.Body.Replace("paddress", "");
                message.Body = message.Body.Replace("pcontact", payment.payer.payer_info.phone);
                message.Body = message.Body.Replace("pmail", request.Obj.UserEmail);
                message.Body = message.Body.Replace("dname", request.Obj.OwnerName);
                message.Body = message.Body.Replace("demail", request.Obj.OwnerEmail);
                message.Body = message.Body.Replace("dcontact", request.Obj.OwnerPhone);
                message.Body = message.Body.Replace("itempath", url);
                message.Body = message.Body.Replace("my_hostings", domainName);

                message.IsBodyHtml = true;
                SendVia(Ioc.Get<IMailSendProvider>(), message, new SettingsDataService().GetEmailSendPropertyForRental());
                return true;
            }
            catch (Exception ex)
            {
                Ioc.Get<DbLogger>().LogException(LogSource.SendMail, string.Format("Error when sending verification message to email: {0}", request.Obj.UserEmail), ex);
                return false;
            }
        }
        public bool SendAbusiveReminder(int goodid, User user, string url)
        {
            try
            {
                string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
                string currentYear = Convert.ToString(DateTime.Now.Year);
                const string phVerificationUrl = "{verification-url}";
                var t = Ioc.Get<ITranslateController>();
                var message = new Message();
                if (_fromHeader != null)
                    message.FromHeader = _fromHeader;
                else
                {
                    message.FromHeader = new FromHeader()
                    {
                        From = Settings.GetEmailRental(),
                        FromName = Settings.GetSmtpFromName(),
                        Subject = string.Format("{0}", t["Item Violation"])
                    };
                }
                message.To = user.Email;
                //message.Bcc = Settings.GetEmailRental();
                message.Body = MessageTemplate.GetMailTemplate(LanguageController.CurLang, "abusive_item_template");
                message.Body = message.Body.Replace(phVerificationUrl, url);
                message.Body = message.Body.Replace("my_hostings", domainName);
                message.Body = message.Body.Replace("firstname", user.FirstName);
                message.Body = message.Body.Replace("{GoodId}", Convert.ToString(goodid));
                message.Body = message.Body.Replace("CurrentYear", currentYear);
                message.IsBodyHtml = true;
                return SendVia(Ioc.Get<IMailSendProvider>(), message, new SettingsDataService().GetEmailSendPropertyForRental());
            }
            catch (Exception ex)
            {
                Ioc.Get<DbLogger>().LogException(LogSource.SendMail, string.Format("Error when sending verification message to email: {0}", user.Email), ex);
            }
            return false;
        }

        public bool SendEmailThankYouTemplate(int borrowerId, string borrowerName, string borrowerEmail, string promoCode, string url, double discountAmount, string discountType)
        {
            try
            {
                string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
                string currentYear = Convert.ToString(DateTime.Now.Year);
                string UnsubscribertURL = domainName + "/Account/NewsLetterUnSubscribe?Email=" + borrowerEmail;

                string couponAmount = string.Empty;
                const string phVerificationUrl = "{verification-url}";
                var t = Ioc.Get<ITranslateController>();
                var message = new Message();
                if (_fromHeader != null)
                    message.FromHeader = _fromHeader;
                else
                {
                    message.FromHeader = new FromHeader()
                    {
                        From = Settings.GetEmailMarketing(),
                        FromName = Settings.GetSmtpFromName(),
                        Subject = string.Format("{0}", t["A thank you gift from momentarily"])
                    };
                }
                message.To = borrowerEmail;
                //message.Bcc = Settings.GetEmailAdmin();
                //message.Bcc = "hello@momentarily.com";
                //message.Bcc = "princydhuparwins@gmail.com";
                message.Body = MessageTemplate.GetMailTemplate(LanguageController.CurLang, "ThankyouNew");
                //message.Body = message.Body.Replace(phVerificationUrl, url);
                message.Body = message.Body.Replace("my_hostings", domainName);
                message.Body = message.Body.Replace("(First Name)", borrowerName);
                message.Body = message.Body.Replace("{coupon}", promoCode);
                message.Body = message.Body.Replace("CurrentYear", currentYear);
                message.Body = message.Body.Replace("UnsubscribeLink", UnsubscribertURL);
                if (discountType == "%")
                    couponAmount = Convert.ToString(discountAmount) + discountType;
                else if (discountType == "$")
                    couponAmount = discountType + Convert.ToString(discountAmount);
                else
                    couponAmount = Convert.ToString(discountAmount);
                message.Body = message.Body.Replace("$discountAmt$", couponAmount);

                message.IsBodyHtml = true;
                return SendVia(Ioc.Get<IMailSendProvider>(), message, new SettingsDataService().GetEmailSendPropertyForMarketing());
            }
            catch (Exception ex)
            {
                Ioc.Get<DbLogger>().LogException(LogSource.SendMail, string.Format("Error when sending verification message to email: {0}", borrowerEmail), ex);
            }
            return false;
        }

        public bool SendPaymentEmailTemplateInvoiceDetail(PayPal.Api.Payment payment, Result<GoodRequestViewModel> request, string url, User sharerInfo, User borrowerInfo, string sharerPhone, string borrowerPhone, string getPickupLocation)        {            try            {                string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);                string UnsubscribertURL = domainName + "/Account/NewsLetterUnSubscribe?Email=" + request.Obj.UserEmail;                string currentYear = Convert.ToString(DateTime.Now.Year);                var address = payment.payer.payer_info.shipping_address.line1 + " " +                   payment.payer.payer_info.shipping_address.line2 + ", " +                   payment.payer.payer_info.shipping_address.city + ", " +                   payment.payer.payer_info.shipping_address.state + ", " +                   payment.payer.payer_info.shipping_address.postal_code;                string dollerSign = "$";                string _dayCost = string.Format("{0:N2}", request.Obj.DaysCost);                string _diliveryRate = string.Format("{0:N2}", request.Obj.DiliveryCost);
                //string _serviceFee = string.Format("{0:N2}", request.Obj.CustomerServiceFeeCost + request.Obj.CustomerCharityCost);
                string _serviceFee = string.Format("{0:N2}", request.Obj.CustomerServiceFeeCost);                string _charitycost = string.Format("{0:N2}", request.Obj.CustomerCharityCost);                string _securityDeposit = string.Format("{0:N2}", request.Obj.SecurityDeposit);                string _customerCost = string.Format("{0:N2}", request.Obj.CustomerCost);                string _phoneNumber = formatPhoneNumber(Convert.ToString(request.Obj.OwnerPhone), "");                string dummyItemImage = "/Content/Images/Good/error-bg.png";                string imagePath = string.Empty;                string lastWord = url.Substring(url.LastIndexOf('/') + 1);                var filePath = HttpContextFactory.Current.Server.MapPath(                         AppSettings.GetInstance().GoodImageLocalStoragePath + lastWord);                if (!File.Exists(filePath))                {                    imagePath = domainName + dummyItemImage;                }                else                {                    imagePath = domainName + url;                }                var t = Ioc.Get<ITranslateController>();                var message = new Message();                if (_fromHeader != null)                    message.FromHeader = _fromHeader;                else                {                    message.FromHeader = new FromHeader()                    {                        From = Settings.GetEmailRental(),                        FromName = Settings.GetSmtpFromName(),                        Subject = string.Format("{0}", t["Your item rental receipt and details"])                    };                }                message.To = request.Obj.UserEmail;
                //message.Bcc = Settings.GetEmailRental();
                //message.Bcc = "hello@momentarily.com";
                //message.Bcc = "princydhuparwins@gmail.com";
                //message.Bcc = request.Obj.OwnerEmail;
                message.Body = MessageTemplate.GetMailTemplate(LanguageController.CurLang, "invoiceReciept");                message.Body = message.Body.Replace("$order$", payment.transactions[0].invoice_number);                message.Body = message.Body.Replace("$dated$", DateTime.Now.ToShortDateString());                message.Body = message.Body.Replace("$itemname$", payment.transactions[0].item_list.items[0].name);                message.Body = message.Body.Replace("$dayprice$", Convert.ToString(request.Obj.Price));                message.Body = message.Body.Replace("$days$", Convert.ToString(request.Obj.Days));                message.Body = message.Body.Replace("$daycost$", "$ " + Convert.ToString(_dayCost));                message.Body = message.Body.Replace("$deliverprice$", Convert.ToString(request.Obj.DiliveryPrice));                message.Body = message.Body.Replace("$shippingdistance$", Convert.ToString(request.Obj.ShippingDistance));                message.Body = message.Body.Replace("$deliverrate$", "$ " + Convert.ToString(_diliveryRate));                if (request.Obj.DiliveryCost == 0)                {                    message.Body = message.Body.Replace("$DeliveryText$", "No Delivery Charges");                }                if (request.Obj.DiliveryCost == 5)                {                    message.Body = message.Body.Replace("$DeliveryText$", "$5 (Minimum Delivery Charges");                }                if (request.Obj.DiliveryCost > 5)                {                    message.Body = message.Body.Replace("$DeliveryText$", "$" + request.Obj.DiliveryPrice + " x " + request.Obj.ShippingDistance);                }                message.Body = message.Body.Replace("$servicefee$", "$ " + Convert.ToString(_serviceFee));                message.Body = message.Body.Replace("$charityCost$", "$ " + Convert.ToString(_charitycost));                message.Body = message.Body.Replace("$securityDeposit$", "$ " + Convert.ToString(_securityDeposit));                message.Body = message.Body.Replace("$customercost$", "$ " + Convert.ToString(_customerCost));                message.Body = message.Body.Replace("$startday$", request.Obj.StartDate.DayOfWeek.ToString());                message.Body = message.Body.Replace("$startdate$", request.Obj.StartDate.ToShortDateString());                message.Body = message.Body.Replace("$starttime$", request.Obj.StartTime);                message.Body = message.Body.Replace("$endday$", request.Obj.EndDate.DayOfWeek.ToString());                message.Body = message.Body.Replace("$enddate$", request.Obj.EndDate.ToShortDateString());                message.Body = message.Body.Replace("$endtime$", request.Obj.EndTime);                message.Body = message.Body.Replace("$pfirst$", sharerInfo.FirstName);                message.Body = message.Body.Replace("$plast$", sharerInfo.LastName);                message.Body = message.Body.Replace("$paddress$", getPickupLocation);                message.Body = message.Body.Replace("$pcontact$", sharerPhone == "" ? "N/A" : formatPhoneNumber(sharerPhone, ""));                message.Body = message.Body.Replace("$pmail$", sharerInfo.Email);                message.Body = message.Body.Replace("$dname$", sharerInfo.FirstName);                message.Body = message.Body.Replace("$demail$", sharerInfo.Email);                message.Body = message.Body.Replace("$dcontact$", sharerPhone == "" ? "N/A" : formatPhoneNumber(sharerPhone, ""));                message.Body = message.Body.Replace("$dAddress$", request.Obj.ApplyForDelivery == true ? request.Obj.ShippingAddress : getPickupLocation);                message.Body = message.Body.Replace("$itempath$", imagePath);                message.Body = message.Body.Replace("my_hostings", domainName);                message.Body = message.Body.Replace("$dollorsign$", dollerSign);                message.Body = message.Body.Replace("CurrentYear", currentYear);                message.Body = message.Body.Replace("UnsubscribeLink", UnsubscribertURL);
                message.Body = message.Body.Replace("{BlockDelivery}", request.Obj.ApplyForDelivery == true ? "table-row" : "none");                message.IsBodyHtml = true;                SendVia(Ioc.Get<IMailSendProvider>(), message, new SettingsDataService().GetEmailSendPropertyForRental());                return true;            }            catch (Exception ex)            {                Ioc.Get<DbLogger>().LogException(LogSource.SendMail, string.Format("Error when sending verification message to email: {0}", request.Obj.UserEmail), ex);                Ioc.Get<DbLogger>().LogException(LogSource.SendMail, string.Format("Error when sending verification message to email: {0}", request.Obj.OwnerEmail), ex);                return false;            }        }
        public bool SendCancelEmailTemplateDetailForBorrower(PaypalPayment payment, Result<GoodRequestViewModel> request, string refundAmount)        {            try            {                string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);                string UnsubscribertURL = domainName + "/Account/NewsLetterUnSubscribe?Email=" + request.Obj.UserEmail;                string currentYear = Convert.ToString(DateTime.Now.Year);                string dollerSign = "$";                string _dayCost = string.Format("{0:N2}", request.Obj.DaysCost);                string _diliveryRate = string.Format("{0:N2}", request.Obj.DiliveryCost);
                //string _serviceFee = string.Format("{0:N2}", request.Obj.CustomerServiceFeeCost + request.Obj.CustomerCharityCost);
                string _serviceFee = string.Format("{0:N2}", request.Obj.CustomerServiceFeeCost);                string _charitycost = string.Format("{0:N2}", request.Obj.CustomerCharityCost);                string _securityDeposit = string.Format("{0:N2}", request.Obj.SecurityDeposit);                string _customerCost = string.Format("{0:N2}", request.Obj.CustomerCost);                string _phoneNumber = formatPhoneNumber(Convert.ToString(request.Obj.OwnerPhone), "");                var t = Ioc.Get<ITranslateController>();                var message = new Message();                if (_fromHeader != null)                    message.FromHeader = _fromHeader;                else                {                    message.FromHeader = new FromHeader()                    {                        From = Settings.GetEmailRental(),                        FromName = Settings.GetSmtpFromName(),                        Subject = string.Format("{0}", t["Your rental has been cancelled"])                    };                }                message.To = request.Obj.UserEmail;
                //message.Bcc = Settings.GetEmailRental();
                //message.Bcc = "hello@momentarily.com";
                //message.Bcc = "princydhuparwins@gmail.com";
                //message.Bcc = request.Obj.OwnerEmail;
                message.Body = MessageTemplate.GetMailTemplate(LanguageController.CurLang, "cancelRecieptForBorrowerAfterPayement");                message.Body = message.Body.Replace("$order$", payment.PaymentInvoiceNumber);                message.Body = message.Body.Replace("$dated$", DateTime.Now.ToShortDateString());                message.Body = message.Body.Replace("$itemname$", request.Obj.GoodName);                message.Body = message.Body.Replace("$dayprice$", Convert.ToString(request.Obj.Price));                message.Body = message.Body.Replace("$days$", Convert.ToString(request.Obj.Days));                message.Body = message.Body.Replace("$daycost$", "$ " + Convert.ToString(_dayCost));                message.Body = message.Body.Replace("$deliverprice$", Convert.ToString(request.Obj.DiliveryPrice));                message.Body = message.Body.Replace("$shippingdistance$", Convert.ToString(request.Obj.ShippingDistance));                message.Body = message.Body.Replace("$deliverrate$", "$ " + Convert.ToString(_diliveryRate));                if (request.Obj.DiliveryCost == 0)                {                    message.Body = message.Body.Replace("$DeliveryText$", "No Delivery Charges");                }                if (request.Obj.DiliveryCost == 5)                {                    message.Body = message.Body.Replace("$DeliveryText$", "$5 (Minimum Delivery Charges");                }                if (request.Obj.DiliveryCost > 5)                {                    message.Body = message.Body.Replace("$DeliveryText$", "$" + request.Obj.DiliveryPrice + " x " + request.Obj.ShippingDistance);                }                message.Body = message.Body.Replace("$servicefee$", "$ " + Convert.ToString(_serviceFee));                message.Body = message.Body.Replace("$charityCost$", "$ " + Convert.ToString(_charitycost));                message.Body = message.Body.Replace("$securityDeposit$", "$ " + Convert.ToString(_securityDeposit));                message.Body = message.Body.Replace("$customercost$", "$ " + Convert.ToString(_customerCost));
                message.Body = message.Body.Replace("$refundAmount$", "$ " + Convert.ToString(refundAmount));
                message.Body = message.Body.Replace("my_hostings", domainName);                message.Body = message.Body.Replace("$dollorsign$", dollerSign);                message.Body = message.Body.Replace("CurrentYear", currentYear);                message.Body = message.Body.Replace("UnsubscribeLink", UnsubscribertURL);
                message.Body = message.Body.Replace("{BlockDelivery}", request.Obj.ApplyForDelivery == true ? "table-row" : "none");                message.IsBodyHtml = true;                SendVia(Ioc.Get<IMailSendProvider>(), message, new SettingsDataService().GetEmailSendPropertyForRental());                return true;            }            catch (Exception ex)            {                Ioc.Get<DbLogger>().LogException(LogSource.SendMail, string.Format("Error when sending verification message to email: {0}", request.Obj.UserEmail), ex);                Ioc.Get<DbLogger>().LogException(LogSource.SendMail, string.Format("Error when sending verification message to email: {0}", request.Obj.OwnerEmail), ex);                return false;            }        }
        public string formatPhoneNumber(string phoneNum, string phoneFormat)
        {
            if (phoneFormat == "")
            {
                // If phone format is empty, code will use default format (###) ###-####
                phoneFormat = "(###) ###-####";
            }
            // First, remove everything except of numbers
            Regex regexObj = new Regex(@"[^\d]");
            phoneNum = regexObj.Replace(phoneNum, "");
            // Second, format numbers to phone string
            if (phoneNum.Length > 0)
            {
                phoneNum = Convert.ToInt64(phoneNum).ToString(phoneFormat);
            }
            return phoneNum;
        }

        public bool SendPaymentEmailTemplateInvoiceDetailForOwnerEmail(PayPal.Api.Payment payment, Result<GoodRequestViewModel> request, string url, User sharerInfo, User borrowerInfo, string sharerPhone, string borrowerPhone, string getPickupLocation)        {            try            {                string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);                string currentYear = Convert.ToString(DateTime.Now.Year);                string UnsubscribertURL = domainName + "/Account/NewsLetterUnSubscribe?Email=" + request.Obj.OwnerEmail;                var address = payment.payer.payer_info.shipping_address.line1 + " " +                   payment.payer.payer_info.shipping_address.line2 + ", " +                   payment.payer.payer_info.shipping_address.city + ", " +                   payment.payer.payer_info.shipping_address.state + ", " +                   payment.payer.payer_info.shipping_address.postal_code;                string dollerSign = "$";                string _dayCost = string.Format("{0:N2}", request.Obj.DaysCost);                string _diliveryRate = string.Format("{0:N2}", request.Obj.DiliveryCost);
                //string _serviceFee = string.Format("{0:N2}", request.Obj.CustomerServiceFeeCost + request.Obj.CustomerCharityCost);
                string _serviceFee = string.Format("{0:N2}", request.Obj.SharerServiceFeeCost);                string _sharerCharityCost = string.Format("{0:N2}", request.Obj.SharerCharityCost);                string _phoneNumber = formatPhoneNumber(Convert.ToString(request.Obj.OwnerPhone), "");                string dummyItemImage = "/Content/Images/Good/error-bg.png";                string imagePath = string.Empty;                string lastWord = url.Substring(url.LastIndexOf('/') + 1);                var filePath = HttpContextFactory.Current.Server.MapPath(                         AppSettings.GetInstance().GoodImageLocalStoragePath + lastWord);                if (!File.Exists(filePath))                {                    imagePath = domainName + dummyItemImage;                }                else                {                    imagePath = domainName + url;                }                var t = Ioc.Get<ITranslateController>();                var message = new Message();                if (_fromHeader != null)                    message.FromHeader = _fromHeader;                else                {                    message.FromHeader = new FromHeader()                    {                        From = Settings.GetEmailRental(),                        FromName = Settings.GetSmtpFromName(),                        Subject = string.Format("{0}", t["Important details about your rental"])                    };                }                message.To = request.Obj.OwnerEmail;
                //message.Bcc = Settings.GetEmailRental();
                //message.Bcc = "hello@momentarily.com";
                //message.Bcc = "princydhuparwins@gmail.com";
                //message.Bcc = request.Obj.OwnerEmail;
                message.Body = MessageTemplate.GetMailTemplate(LanguageController.CurLang, "ownerReciept");                message.Body = message.Body.Replace("$order$", payment.transactions[0].invoice_number);                message.Body = message.Body.Replace("$dated$", DateTime.Now.ToShortDateString());                message.Body = message.Body.Replace("$itemname$", payment.transactions[0].item_list.items[0].name);                message.Body = message.Body.Replace("$dayprice$", Convert.ToString(request.Obj.Price));                message.Body = message.Body.Replace("$days$", Convert.ToString(request.Obj.Days));                message.Body = message.Body.Replace("$daycost$", "$ " + Convert.ToString(_dayCost));                message.Body = message.Body.Replace("$deliverprice$", Convert.ToString(request.Obj.DiliveryPrice));                message.Body = message.Body.Replace("$shippingdistance$", Convert.ToString(request.Obj.ShippingDistance));                message.Body = message.Body.Replace("$deliverrate$", "$ " + Convert.ToString(_diliveryRate));                if (request.Obj.DiliveryCost == 0)                {                    message.Body = message.Body.Replace("$DeliveryText$", "No Delivery Charges");                }                if (request.Obj.DiliveryCost == 5)                {                    message.Body = message.Body.Replace("$DeliveryText$", "$5 (Minimum Delivery Charges");                }                if (request.Obj.DiliveryCost > 5)                {                    message.Body = message.Body.Replace("$DeliveryText$", "$" + request.Obj.DiliveryPrice + " x " + request.Obj.ShippingDistance);                }                message.Body = message.Body.Replace("$servicefee$", "-$ " + Convert.ToString(_serviceFee));                message.Body = message.Body.Replace("$sharercharitycost$", "-$ " + Convert.ToString(_sharerCharityCost));                message.Body = message.Body.Replace("$sharercost$", Convert.ToString(request.Obj.SharerCost));                var getserviceFee = request.Obj.CustomerServiceFeeCost + request.Obj.CustomerCharityCost;                var minusServiceFeeFromCustomerCost = request.Obj.CustomerCost - Convert.ToDouble(getserviceFee);                string _customerCost = string.Format("{0:N2}", minusServiceFeeFromCustomerCost);                message.Body = message.Body.Replace("$customercost$", "$ " + Convert.ToString(_customerCost));                message.Body = message.Body.Replace("$startday$", request.Obj.StartDate.DayOfWeek.ToString());                message.Body = message.Body.Replace("$startdate$", request.Obj.StartDate.ToShortDateString());                message.Body = message.Body.Replace("$starttime$", request.Obj.StartTime);                message.Body = message.Body.Replace("$endday$", request.Obj.EndDate.DayOfWeek.ToString());                message.Body = message.Body.Replace("$enddate$", request.Obj.EndDate.ToShortDateString());                message.Body = message.Body.Replace("$endtime$", request.Obj.EndTime);                message.Body = message.Body.Replace("$pfirst$", borrowerInfo.FirstName);                message.Body = message.Body.Replace("$plast$", borrowerInfo.LastName);                message.Body = message.Body.Replace("$paddress$", request.Obj.ApplyForDelivery == true ? request.Obj.ShippingAddress : getPickupLocation);                message.Body = message.Body.Replace("$pcontact$", borrowerPhone == "" ? "N/A" : formatPhoneNumber(borrowerPhone, ""));                message.Body = message.Body.Replace("$pmail$", borrowerInfo.Email);                message.Body = message.Body.Replace("$dname$", borrowerInfo.FirstName);                message.Body = message.Body.Replace("$demail$", borrowerInfo.Email);                message.Body = message.Body.Replace("$dcontact$", borrowerPhone == "" ? "N/A" : formatPhoneNumber(borrowerPhone, ""));                message.Body = message.Body.Replace("$daddress$", request.Obj.ApplyForDelivery == true ? request.Obj.ShippingAddress : getPickupLocation);                message.Body = message.Body.Replace("$itempath$", imagePath);                message.Body = message.Body.Replace("my_hostings", domainName);                message.Body = message.Body.Replace("$dollorsign$", dollerSign);                message.Body = message.Body.Replace("CurrentYear", currentYear);                message.Body = message.Body.Replace("UnsubscribeLink", UnsubscribertURL);                message.IsBodyHtml = true;                SendVia(Ioc.Get<IMailSendProvider>(), message, new SettingsDataService().GetEmailSendPropertyForRental());                return true;            }            catch (Exception ex)            {                Ioc.Get<DbLogger>().LogException(LogSource.SendMail, string.Format("Error when sending verification message to email: {0}", request.Obj.UserEmail), ex);                Ioc.Get<DbLogger>().LogException(LogSource.SendMail, string.Format("Error when sending verification message to email: {0}", request.Obj.OwnerEmail), ex);                return false;            }        }

        public bool SendPaymentEmailTemplateOfferReceivedForSharer(User user, GoodRequest request, string url)        {            try            {                string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);                string UnsubscribertURL = domainName + "/Account/NewsLetterUnSubscribe?Email=" + request.Good.UserGood.User.Email;                string currentYear = Convert.ToString(DateTime.Now.Year);                string borrorerName = string.Empty;                if (!string.IsNullOrWhiteSpace(user.FullName))                    borrorerName = user.FullName;                else                    borrorerName = user.FirstName;                string dollerSign = "$";

                //var _dayCost = request.DaysCost.ToString("N2");
                string _dayCost = string.Format("{0:N2}", request.DaysCost);                string _diliverycost = string.Format("{0:N2}", request.DiliveryCost);                string _customerCost = string.Format("{0:N2}", request.CustomerCost);
                //string _discountAmt = string.Format("{0:N2}", request.DiscountAmount);
                string _serviceFee = string.Format("{0:N2}", request.SharerServiceFeeCost);                string _charitycost = string.Format("{0:N2}", request.SharerCharityCost);                string _totalcustomerCost = string.Format("{0:N2}", request.DaysCost - (request.SharerServiceFeeCost + request.SharerCharityCost));                var t = Ioc.Get<ITranslateController>();                var message = new Message();                if (_fromHeader != null)                    message.FromHeader = _fromHeader;                else                {                    message.FromHeader = new FromHeader()                    {                        From = Settings.GetEmailRental(),                        FromName = Settings.GetSmtpFromName(),                        Subject = string.Format("{0}", t["Offer received detail"])                    };                }                message.To = request.Good.UserGood.User.Email;
                //message.Bcc = "";
                //message.Bcc = "hello@momentarily.com";
                // message.Bcc = "pawankwins@gmail.com";
                message.Body = MessageTemplate.GetMailTemplate(LanguageController.CurLang, "offer_received");                message.Body = message.Body.Replace("username", borrorerName);                message.Body = message.Body.Replace("itemname", request.Good.Name);                message.Body = message.Body.Replace("days", Convert.ToString(request.Days));                message.Body = message.Body.Replace("dayprice", Convert.ToString(request.Price));                message.Body = message.Body.Replace("daycost", "$ " + Convert.ToString(_dayCost));                message.Body = message.Body.Replace("servicefee", "-$ " + Convert.ToString(_serviceFee));                message.Body = message.Body.Replace("$charitycost$", "-$ " + Convert.ToString(_charitycost));                message.Body = message.Body.Replace("deliverycharges", "-$ " + Convert.ToString(request.DiliveryPrice));                message.Body = message.Body.Replace("shippingdistance", Convert.ToString(request.ShippingDistance));                message.Body = message.Body.Replace("deliverycost", "$ " + Convert.ToString(_diliverycost));                message.Body = message.Body.Replace("$sharercost$", "$ " + Convert.ToString(request.SharerCost));                if (request.DiliveryCost == 0)                {                    message.Body = message.Body.Replace("DeliveryText", "No Delivery Charges");                }                if (request.DiliveryCost == 5)                {                    message.Body = message.Body.Replace("DeliveryText", "$5 (Minimum Delivery Charges");                }                if (request.DiliveryCost > 5)                {                    message.Body = message.Body.Replace("DeliveryText", "$" + request.DiliveryPrice + " x " + request.ShippingDistance);                }
                // message.Body = message.Body.Replace("discountAmount", "-$ " + Convert.ToString(_discountAmt));
                message.Body = message.Body.Replace("customercost", "$ " + Convert.ToString(_totalcustomerCost));                message.Body = message.Body.Replace("startday", request.GoodBooking.StartDate.DayOfWeek.ToString());                message.Body = message.Body.Replace("startdate", request.GoodBooking.StartDate.ToShortDateString());                message.Body = message.Body.Replace("starttime", request.GoodBooking.StartTime);                message.Body = message.Body.Replace("endday", request.GoodBooking.EndDate.DayOfWeek.ToString());                message.Body = message.Body.Replace("enddate", request.GoodBooking.EndDate.ToShortDateString());                message.Body = message.Body.Replace("endtime", request.GoodBooking.EndTime);
                //message.Body = message.Body.Replace("itempath", imagePath);
                message.Body = message.Body.Replace("my_hostings", domainName);                message.Body = message.Body.Replace("accept_request_url", url);                message.Body = message.Body.Replace("dollorsign", dollerSign);                message.Body = message.Body.Replace("CurrentYear", currentYear);                message.Body = message.Body.Replace("UnsubscribeLink", UnsubscribertURL);                message.IsBodyHtml = true;                SendVia(Ioc.Get<IMailSendProvider>(), message, new SettingsDataService().GetEmailSendPropertyForRental());                return true;            }            catch (Exception ex)            {                Ioc.Get<DbLogger>().LogException(LogSource.SendMail, string.Format("Error when sending verification message to email: {0}", request.Good.UserGood.User.Email), ex);                return false;            }        }

        public bool SendEmailNewsLetterTemplate(string userEmail, string url)
        {
            try
            {
                string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
                //string UnsubscribertURL = domainName + "/Account/NewsLetterUnSubscribe?Email=" + userEmail;
                string currentYear = Convert.ToString(DateTime.Now.Year);
                const string phVerificationUrl = "{verification-url}";
                var t = Ioc.Get<ITranslateController>();
                string linkUrl = domainName + url;
                var message = new Message();
                if (_fromHeader != null)
                    message.FromHeader = _fromHeader;
                else
                {
                    message.FromHeader = new FromHeader()
                    {
                        //From = Settings.GetSmtpFrom(),
                        From = Settings.GetEmailMarketing(),
                        FromName = Settings.GetSmtpFromName(),
                        Subject = string.Format("{0}", t["Newsletter"])
                    };
                }
                message.To = userEmail;
                //message.Bcc = Settings.GetEmailAdmin();
                //message.Bcc = "hello@momentarily.com";
                //message.Bcc = "princydhuparwins@gmail.com";
                message.Body = MessageTemplate.GetMailTemplate(LanguageController.CurLang, "NewsLetter");
                message.Body = message.Body.Replace("my_hostings", domainName);
                message.Body = message.Body.Replace("buttonLink", linkUrl);
                message.Body = message.Body.Replace("CurrentYear", currentYear);
                //message.Body = message.Body.Replace("UnsubscribeLink", UnsubscribertURL);
                message.Body = message.Body.Replace("textEmail", "subscribeEmail");

                message.IsBodyHtml = true;
                return SendVia(Ioc.Get<IMailSendProvider>(), message, new SettingsDataService().GetEmailSendPropertyForMarketing());
            }
            catch (Exception ex)
            {
                Ioc.Get<DbLogger>().LogException(LogSource.SendMail, string.Format("Error when sending verification message to email: {0}", userEmail), ex);
            }
            return false;
        }


        public bool SendEmailWelcomeTemplate(string userEmail, string userName, string url)
        {
            try
            {
                string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
                string UnsubscribertURL = domainName + "/Account/NewsLetterUnSubscribe?Email=" + userEmail;
                string currentYear = Convert.ToString(DateTime.Now.Year);
                var t = Ioc.Get<ITranslateController>();
                var message = new Message();
                if (_fromHeader != null)
                    message.FromHeader = _fromHeader;
                else
                {
                    message.FromHeader = new FromHeader()
                    {
                        //From = Settings.GetSmtpFrom(),
                        From = Settings.GetEmailMarketing(),
                        FromName = Settings.GetSmtpFromName(),
                        Subject = string.Format("{0}", t[" Welcome to our sharing community!"])
                    };
                }
                message.To = userEmail;
                //message.Bcc = Settings.GetEmailAdmin();
                //message.Bcc = "hello@momentarily.com";
                //message.Bcc = "princydhuparwins@gmail.com";
                message.Body = MessageTemplate.GetMailTemplate(LanguageController.CurLang, "WelcomeSignUp");
                message.Body = message.Body.Replace("my_hostings", domainName);
                message.Body = message.Body.Replace("[userName]", userName);
                message.Body = message.Body.Replace("CurrentYear", currentYear);
                message.Body = message.Body.Replace("UnsubscribeLink", UnsubscribertURL);
                message.Body = message.Body.Replace("$Fashion$", domainName + "/Content/TemplateImg/images/ClothingPNG.jpg");
                message.Body = message.Body.Replace("$Education$", domainName + "/Content/TemplateImg/images/EducationPNG.jpg");
                message.Body = message.Body.Replace("$Electronics$", domainName + "/Content/TemplateImg/images/ElectronicsPNG.jpg");
                message.Body = message.Body.Replace("$Home$", domainName + "/Content/TemplateImg/images/HomePNG.jpg");
                message.Body = message.Body.Replace("$Kitchen$", domainName + "/Content/TemplateImg/images/KitchenPNG.jpg");
                message.Body = message.Body.Replace("$Outdoors$", domainName + "/Content/TemplateImg/images/OutdoorsPNG.jpg");
                message.Body = message.Body.Replace("$Motorized$", domainName + "/Content/TemplateImg/images/SpacePNG.jpg");
                message.Body = message.Body.Replace("$Tools1$", domainName + "/Content/TemplateImg/images/ToolsPNG.jpg");
                message.Body = message.Body.Replace("$Events$", domainName + "/Content/TemplateImg/images/EventsPNG.jpg");

                message.IsBodyHtml = true;
                return SendVia(Ioc.Get<IMailSendProvider>(), message, new SettingsDataService().GetEmailSendPropertyForMarketing());
            }
            catch (Exception ex)
            {
                Ioc.Get<DbLogger>().LogException(LogSource.SendMail, string.Format("Error when sending verification message to email: {0}", userEmail), ex);
            }
            return false;
        }

        public bool SendLaunchSoonMessage(string email, DateTime launchingdate, string username, string coupon, double couponDiscount, string couponType)
        {
            TimeSpan diff = launchingdate - DateTime.Now;
            var days = Convert.ToString(diff.Days);
            var hours = Convert.ToString(diff.Hours);
            var minutes = Convert.ToString(diff.Minutes);
            var seconds = Convert.ToString(diff.Seconds);
            string couponAmount = string.Empty;
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string currentYear = Convert.ToString(DateTime.Now.Year);
            string UnsubscribertURL = domainName + "/Account/NewsLetterUnSubscribe?Email=" + email;
            try
            {
                var t = Ioc.Get<ITranslateController>();
                var message = new Message();
                if (_fromHeader != null)
                    message.FromHeader = _fromHeader;
                else
                {
                    message.FromHeader = new FromHeader()
                    {
                        //From = Settings.GetSmtpFrom(),
                        From = Settings.GetEmailMarketing(),
                        FromName = Settings.GetSmtpFromName(),
                        Subject = string.Format("{0}", t["A special invitation to the community rental marketplace"])
                    };
                }
                message.To = email;
                //message.Bcc = Settings.GetEmailAdmin();
                //message.Bcc = "hello@momentarily.com";
                //message.Bcc = "princydhuparwins@gmail.com";
                message.Body = MessageTemplate.GetMailTemplate(LanguageController.CurLang, "Launching");
                message.Body = message.Body.Replace("my_hostings", domainName);
                message.Body = message.Body.Replace("CurrentYear", currentYear);
                message.Body = message.Body.Replace("UnsubscribeLink", UnsubscribertURL);
                message.Body = message.Body.Replace("{user}", username);
                message.Body = message.Body.Replace("{coupon}", coupon);
                message.Body = message.Body.Replace("{DaysLeft}", days);
                message.Body = message.Body.Replace("{HoursLeft}", hours);
                message.Body = message.Body.Replace("{MinutesLeft}", minutes);
                message.Body = message.Body.Replace("{SecondLeft}", seconds);
                if (couponType == "%")
                    couponAmount = Convert.ToString(couponDiscount) + couponType;
                else if (couponType == "$")
                    couponAmount = couponType + Convert.ToString(couponDiscount);
                else
                    couponAmount = Convert.ToString(couponDiscount);
                message.Body = message.Body.Replace("$discountAmt$", couponAmount);

                message.IsBodyHtml = true;
                return SendVia(Ioc.Get<IMailSendProvider>(), message, new SettingsDataService().GetEmailSendPropertyForMarketing());
            }
            catch (Exception ex)
            {
                Ioc.Get<DbLogger>().LogException(LogSource.SendMail, string.Format("Error when sending get launching message to email: {0}", email), ex);
            }
            return false;
        }

        public bool SendLaunchedMessage(string email, string username, string coupon, double couponDiscount, string couponType)
        {

            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string currentYear = Convert.ToString(DateTime.Now.Year);
            string UnsubscribertURL = domainName + "/Account/NewsLetterUnSubscribe?Email=" + email;
            string couponAmount = string.Empty;
            try
            {
                var t = Ioc.Get<ITranslateController>();
                var message = new Message();
                if (_fromHeader != null)
                    message.FromHeader = _fromHeader;
                else
                {
                    message.FromHeader = new FromHeader()
                    {
                        //From = Settings.GetSmtpFrom(),
                        From = Settings.GetEmailMarketing(),
                        FromName = Settings.GetSmtpFromName(),
                        Subject = string.Format("{0}", t["Rental marketplace now open for business!"])
                    };
                }
                message.To = email;
                //message.Bcc = Settings.GetEmailAdmin();
                //message.Bcc = "hello@momentarily.com";
                //message.Bcc = "princydhuparwins@gmail.com";
                message.Body = MessageTemplate.GetMailTemplate(LanguageController.CurLang, "Launched");
                message.Body = message.Body.Replace("my_hostings", domainName);
                message.Body = message.Body.Replace("CurrentYear", currentYear);
                message.Body = message.Body.Replace("UnsubscribeLink", UnsubscribertURL);
                message.Body = message.Body.Replace("{user}", username);
                message.Body = message.Body.Replace("{coupon}", coupon);
                if (couponType == "%")
                    couponAmount = Convert.ToString(couponDiscount) + couponType;
                else if (couponType == "$")
                    couponAmount = couponType + Convert.ToString(couponDiscount);
                else
                    couponAmount = Convert.ToString(couponDiscount);
                message.Body = message.Body.Replace("$discountAmt$", couponAmount);
                message.IsBodyHtml = true;
                return SendVia(Ioc.Get<IMailSendProvider>(), message, new SettingsDataService().GetEmailSendPropertyForMarketing());
            }
            catch (Exception ex)
            {
                Ioc.Get<DbLogger>().LogException(LogSource.SendMail, string.Format("Error when sending get launched message to email: {0}", email), ex);
            }
            return false;
        }

        public bool SendBlockedUserMessage(string email, string firstname, string status)        {            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string currentYear = Convert.ToString(DateTime.Now.Year);
            string UnsubscribertURL = domainName + "/Account/NewsLetterUnSubscribe?Email=" + email;            try            {                var t = Ioc.Get<ITranslateController>();                var message = new Message();                if (_fromHeader != null)                    message.FromHeader = _fromHeader;                else                {                    message.FromHeader = new FromHeader()                    {
                        //From = Settings.GetSmtpFrom(),
                        From = Settings.GetEmailHelp(),                        FromName = Settings.GetSmtpFromName(),                        Subject = string.Format("{0}", t["Account Status"])                    };                }                message.To = email;
                message.Body = MessageTemplate.GetMailTemplate(LanguageController.CurLang, "block_unblock");
                message.Body = message.Body.Replace("my_hostings", domainName);
                message.Body = message.Body.Replace("CurrentYear", currentYear);
                message.Body = message.Body.Replace("UnsubscribeLink", UnsubscribertURL);
                message.Body = message.Body.Replace("first_name", firstname);
                message.Body = message.Body.Replace("status_blocked", status);
                if (status == "blocked")
                {
                    message.Body = message.Body.Replace("blockedunblocked_content", "Your account has been blocked due to violations of our terms and conditions. If you feel like this was an error, please respond.");

                }
                else
                {
                    message.Body = message.Body.Replace("blockedunblocked_content", "Your account has been reviewed and unblocked. You may resume listing and borrowing items.");
                }
                // message.Body = "Hi " + firstname + ", Your account has been " + status;
                //message.Body = message.Body.Replace("my_hostings", domainName);
                //message.Body = message.Body.Replace("firstname", firstname);
                message.IsBodyHtml = true;                return SendVia(Ioc.Get<IMailSendProvider>(), message, new SettingsDataService().GetEmailSendPropertyComingHelp());            }            catch (Exception ex)            {                Ioc.Get<DbLogger>().LogException(LogSource.SendMail, string.Format("Error when sending block user message to email: {0}", email), ex);            }            return false;        }



        public bool SendTemporaryDeletedUserMessage(string email, string firstname)        {            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string currentYear = Convert.ToString(DateTime.Now.Year);
            string UnsubscribertURL = domainName + "/Account/NewsLetterUnSubscribe?Email=" + email;            try            {                var t = Ioc.Get<ITranslateController>();                var message = new Message();                if (_fromHeader != null)                    message.FromHeader = _fromHeader;                else                {                    message.FromHeader = new FromHeader()                    {
                        //From = Settings.GetSmtpFrom(),
                        From = Settings.GetEmailHelp(),                        FromName = Settings.GetSmtpFromName(),                        Subject = string.Format("{0}", t["Delete Account"])                    };                }                message.To = email;
                message.Body = MessageTemplate.GetMailTemplate(LanguageController.CurLang, "user_deleted");
                message.Body = message.Body.Replace("my_hostings", domainName);
                message.Body = message.Body.Replace("CurrentYear", currentYear);
                message.Body = message.Body.Replace("UnsubscribeLink", UnsubscribertURL);
                message.Body = message.Body.Replace("{user-name}", firstname);
                message.Body = message.Body.Replace("{mailID}", email);


                message.IsBodyHtml = true;                return SendVia(Ioc.Get<IMailSendProvider>(), message, new SettingsDataService().GetEmailSendPropertyComingHelp());            }            catch (Exception ex)            {                Ioc.Get<DbLogger>().LogException(LogSource.SendMail, string.Format("Error when temporary delete user by admin: {0}", email), ex);            }            return false;        }

        public bool SendTemporaryDeletedUserItemsMessage(string email, string firstname, string itemName)
        {
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string currentYear = Convert.ToString(DateTime.Now.Year);
            string UnsubscribertURL = domainName + "/Account/NewsLetterUnSubscribe?Email=" + email;            try            {                var t = Ioc.Get<ITranslateController>();                var message = new Message();                if (_fromHeader != null)                    message.FromHeader = _fromHeader;                else                {                    message.FromHeader = new FromHeader()                    {
                        //From = Settings.GetSmtpFrom(),
                        From = Settings.GetEmailHelp(),                        FromName = Settings.GetSmtpFromName(),                        Subject = string.Format("{0}", t["Delete Product"])                    };                }                message.To = email;
                message.Body = MessageTemplate.GetMailTemplate(LanguageController.CurLang, "item_delete");
                message.Body = message.Body.Replace("my_hostings", domainName);
                message.Body = message.Body.Replace("CurrentYear", currentYear);
                message.Body = message.Body.Replace("UnsubscribeLink", UnsubscribertURL);
                message.Body = message.Body.Replace("{user-name}", firstname);
                message.Body = message.Body.Replace("{mailID}", email);
                message.Body = message.Body.Replace("{itemName}", itemName);


                message.IsBodyHtml = true;                return SendVia(Ioc.Get<IMailSendProvider>(), message, new SettingsDataService().GetEmailSendPropertyComingHelp());            }            catch (Exception ex)            {                Ioc.Get<DbLogger>().LogException(LogSource.SendMail, string.Format("Error when temporary delete user item by admin: {0}", email), ex);            }            return false;
        }

        public bool SendFinalConfirmation(string email, string status, string borrowerName, string sharerName)        {            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);            string currentYear = Convert.ToString(DateTime.Now.Year);            string UnsubscribertURL = domainName + "/Account/NewsLetterUnSubscribe?Email=" + email;            try            {                var t = Ioc.Get<ITranslateController>();                var message = new Message();                if (_fromHeader != null)                    message.FromHeader = _fromHeader;                else                {                    message.FromHeader = new FromHeader()                    {
                        //From = Settings.GetSmtpFrom(),
                        From = Settings.GetEmailHelp(),                        FromName = Settings.GetSmtpFromName(),                        Subject = string.Format("{0}", t["Final Confirmation"])                    };                }                message.To = email;                message.Body = MessageTemplate.GetMailTemplate(LanguageController.CurLang, "final_Confirmation_New");                message.Body = message.Body.Replace("my_hostings", domainName);                message.Body = message.Body.Replace("{name}", status);                message.Body = message.Body.Replace("{status}", status);                message.Body = message.Body.Replace("{BorrowerName}", borrowerName);                message.Body = message.Body.Replace("{BlockButton}", status == "No Issue" ? "none" : "block");                message.Body = message.Body.Replace("CurrentYear", currentYear);                message.Body = message.Body.Replace("UnsubscribeLink", UnsubscribertURL);
                var wording = "";
                if (status != "No Issue")
                {
                    wording = "<b>" + sharerName + "</b>" + " marked your rental as <b>" + status + "</b>. " +
                       "Please submit your dispute within 24 hours, otherwise you will be charged according to the final rental status.";
                }                else
                {
                    wording = "Your rental has been completed with No Issue status." +
                        " Your payment will be processed shortly.Thanks for being with momentarily";
                }

                message.Body = message.Body.Replace("{wording}", wording);                message.IsBodyHtml = true;                return SendVia(Ioc.Get<IMailSendProvider>(), message, new SettingsDataService().GetEmailSendPropertyComingHelp());            }            catch (Exception ex)            {                Ioc.Get<DbLogger>().LogException(LogSource.SendMail, string.Format("Error when temporary delete user item by admin: {0}", email), ex);            }            return false;        }


        public bool SendEmailThankYouTemplateForSubscribing(int borrowerId, string borrowerName, string borrowerEmail, string promoCode, string url, double discountAmount, string discountType)
        {
            try
            {
                string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
                string currentYear = Convert.ToString(DateTime.Now.Year);
                string UnsubscribertURL = domainName + "/Account/NewsLetterUnSubscribe?Email=" + borrowerEmail;

                string couponAmount = string.Empty;
                const string phVerificationUrl = "{verification-url}";
                var t = Ioc.Get<ITranslateController>();
                var message = new Message();
                if (_fromHeader != null)
                    message.FromHeader = _fromHeader;
                else
                {
                    message.FromHeader = new FromHeader()
                    {
                        From = Settings.GetEmailMarketing(),
                        FromName = Settings.GetSmtpFromName(),
                        Subject = string.Format("{0}", t["Thanks for subscribing!"])
                    };
                }
                message.To = borrowerEmail;
                message.Body = MessageTemplate.GetMailTemplate(LanguageController.CurLang, "thank_you_for_subscribing");
                message.Body = message.Body.Replace("my_hostings", domainName);
                message.Body = message.Body.Replace("(First Name)", borrowerName);
                message.Body = message.Body.Replace("{coupon}", promoCode);
                message.Body = message.Body.Replace("CurrentYear", currentYear);
                message.Body = message.Body.Replace("UnsubscribeLink", UnsubscribertURL);
                if (discountType == "%")
                    couponAmount = Convert.ToString(discountAmount) + discountType;
                else if (discountType == "$")
                    couponAmount = discountType + Convert.ToString(discountAmount);
                else
                    couponAmount = Convert.ToString(discountAmount);
                message.Body = message.Body.Replace("$discountAmt$", couponAmount);

                message.IsBodyHtml = true;
                return SendVia(Ioc.Get<IMailSendProvider>(), message, new SettingsDataService().GetEmailSendPropertyForMarketing());
            }
            catch (Exception ex)
            {
                Ioc.Get<DbLogger>().LogException(LogSource.SendMail, string.Format("Error when sending verification message to email: {0}", borrowerEmail), ex);
            }
            return false;
        }

        public bool SendEmailCancelBookingByBorrower(int userid, string username, string email)
        {
            try            {                string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

                string currentYear = Convert.ToString(DateTime.Now.Year);                var t = Ioc.Get<ITranslateController>();                var message = new Message();                if (_fromHeader != null)                    message.FromHeader = _fromHeader;                else                {                    message.FromHeader = new FromHeader()                    {                        From = Settings.GetEmailRental(),                        FromName = Settings.GetSmtpFromName(),                        Subject = string.Format("{0}", t["Your rental has been cancelled"])                    };                }                message.To = email;
                message.Body = MessageTemplate.GetMailTemplate(LanguageController.CurLang, "cancelled_booking_by_borrower");
                message.Body = message.Body.Replace("my_hostings", domainName);                message.Body = message.Body.Replace("{UserName}", username);
                message.Body = message.Body.Replace("{UserId}", Convert.ToString(userid));                message.Body = message.Body.Replace("CurrentYear", currentYear);                message.IsBodyHtml = true;                SendVia(Ioc.Get<IMailSendProvider>(), message, new SettingsDataService().GetEmailSendPropertyForRental());                return true;            }            catch (Exception ex)            {                Ioc.Get<DbLogger>().LogException(LogSource.SendMail, string.Format("Error when sending verification message to email: {0}", email), ex);                return false;            }
        }
        public bool SendEmailCancelBookingBySharer(int userid, string username, string email, string coupon)
        {
            try            {                string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

                string currentYear = Convert.ToString(DateTime.Now.Year);                var t = Ioc.Get<ITranslateController>();                var message = new Message();                if (_fromHeader != null)                    message.FromHeader = _fromHeader;                else                {                    message.FromHeader = new FromHeader()                    {                        From = Settings.GetEmailRental(),                        FromName = Settings.GetSmtpFromName(),                        Subject = string.Format("{0}", t["Your rental has been cancelled"])                    };                }                message.To = email;
                message.Body = MessageTemplate.GetMailTemplate(LanguageController.CurLang, "cancelled_booking_by_owner");
                message.Body = message.Body.Replace("my_hostings", domainName);                message.Body = message.Body.Replace("{UserName}", username);
                message.Body = message.Body.Replace("{UserId}", Convert.ToString(userid));
                message.Body = message.Body.Replace("{coupon}", coupon);

                message.Body = message.Body.Replace("CurrentYear", currentYear);                message.IsBodyHtml = true;                SendVia(Ioc.Get<IMailSendProvider>(), message, new SettingsDataService().GetEmailSendPropertyForRental());                return true;            }            catch (Exception ex)            {                Ioc.Get<DbLogger>().LogException(LogSource.SendMail, string.Format("Error when sending verification message to email: {0}", email), ex);                return false;            }
        }

        public bool SendEmailBookingDeniedForBorrower(int userid, string username, string email, string couponCode, string itemName, string Message)
        {
            try            {                string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

                string currentYear = Convert.ToString(DateTime.Now.Year);                var t = Ioc.Get<ITranslateController>();                var message = new Message();                if (_fromHeader != null)                    message.FromHeader = _fromHeader;                else                {                    message.FromHeader = new FromHeader()                    {                        From = Settings.GetEmailRental(),                        FromName = Settings.GetSmtpFromName(),                        Subject = string.Format("{0}", t["Booking denied"])                    };                }                message.To = email;
                message.Body = MessageTemplate.GetMailTemplate(LanguageController.CurLang, "booking_denie_for_the_borrower");
                message.Body = message.Body.Replace("my_hostings", domainName);                message.Body = message.Body.Replace("{user-name}", username);
                message.Body = message.Body.Replace("{item-name}", itemName);
                message.Body = message.Body.Replace("{Message}", Message);
                message.Body = message.Body.Replace("{coupon}", couponCode);
                message.Body = message.Body.Replace("{UserId}", Convert.ToString(userid));                message.Body = message.Body.Replace("CurrentYear", currentYear);                message.IsBodyHtml = true;                SendVia(Ioc.Get<IMailSendProvider>(), message, new SettingsDataService().GetEmailSendPropertyForRental());                return true;            }            catch (Exception ex)            {                Ioc.Get<DbLogger>().LogException(LogSource.SendMail, string.Format("Error when sending verification message to email: {0}", email), ex);                return false;            }
        }

        public bool SendEmailAfterTransationComplete(string email, string borrowerName, int bookingId)
        {
            const string phBorrowerName = "{borrower}";
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string currentYear = Convert.ToString(DateTime.Now.Year);
            //string UnsubscribertURL = domainName + "/Account/NewsLetterUnSubscribe?Email=" + email;
            try
            {
                var t = Ioc.Get<ITranslateController>();
                var message = new Message();
                if (_fromHeader != null)
                    message.FromHeader = _fromHeader;
                else
                {
                    message.FromHeader = new FromHeader()
                    {
                        From = Settings.GetSmtpFrom(),
                        FromName = Settings.GetSmtpFromName(),
                        Subject = string.Format("{0}", t["Transaction Completed"])
                    };
                }
                message.To = email;
                message.Body = MessageTemplate.GetMailTemplate(LanguageController.CurLang, "transaction_complete");

                message.Body = message.Body.Replace("my_hostings", domainName);
                message.Body = message.Body.Replace(phBorrowerName, borrowerName);
                message.Body = message.Body.Replace("{BookingId}", Convert.ToString(bookingId));
                message.Body = message.Body.Replace("CurrentYear", currentYear);
                //message.Body = message.Body.Replace("UnsubscribeLink", UnsubscribertURL);
                message.IsBodyHtml = true;
                return SendVia(Ioc.Get<IMailSendProvider>(), message, new SettingsDataService().GetEmailSendProperty());
            }
            catch (Exception ex)
            {
                Ioc.Get<DbLogger>().LogException(LogSource.SendMail, string.Format("Error when sending verification message to email: {0}", email), ex);
            }
            return false;
        }


        public bool SendPaymentEmailTemplateCovidRentalInvoiceDetail(CovidGoodViewModel model, CovidOrderPaymentDetailViewModel orderDesc)        {            try            {                string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);                string UnsubscribertURL = domainName + "/Account/NewsLetterUnSubscribe?Email=" + model.BuyerEmailId;                string currentYear = Convert.ToString(DateTime.Now.Year);

                string dollerSign = "$";                string _goodPrice = string.Format("{0:N2}", model.GoodPrice);                double gettotalGoodPrice = Math.Round(model.GoodPrice * model.Quantity, 2);
                double gettaxAmount = Math.Round(gettotalGoodPrice * model.Tax / 100, 2);
                double _totalAmt = gettotalGoodPrice + gettaxAmount;
                double gettotalAmount = Math.Round(_totalAmt,2);                string _tax = string.Format("{0:N2}", model.Tax);
                string _totalPrice = string.Format("{0:N2}", model.TotalPrice);
                string _priceCost = string.Format("{0:N2}", model.GoodPrice * model.Quantity);


                string dummyItemImage = "/Content/Images/Good/error-bg.png";
                string imagePath = string.Empty;
                string lastWord = model.GoodImage.Substring(model.GoodImage.LastIndexOf('/') + 1);
                var filePath = HttpContextFactory.Current.Server.MapPath(
                         AppSettings.GetInstance().GoodImageCovidLocalStoragePath + lastWord);
                if (!File.Exists(filePath))
                {
                    imagePath = domainName + dummyItemImage;
                }
                else
                {
                    imagePath = domainName + "/Content/covid-image/" + model.GoodImage;
                }


                var t = Ioc.Get<ITranslateController>();
                var message = new Message();
                if (_fromHeader != null)
                    message.FromHeader = _fromHeader;
                else
                {
                    message.FromHeader = new FromHeader()
                    {
                        From = Settings.GetEmailRental(),
                        FromName = Settings.GetSmtpFromName(),
                        Subject = string.Format("{0}", t["Receipt"])
                    };
                }
                message.To = model.BuyerEmailId;

                message.Body = MessageTemplate.GetMailTemplate(LanguageController.CurLang, "covidRentalReciept");

                message.Body = message.Body.Replace("$order$", orderDesc.InvoiceNumber);
                message.Body = message.Body.Replace("$itemname$", model.GoodName);
                message.Body = message.Body.Replace("$goodPrice$", _goodPrice);
                message.Body = message.Body.Replace("$daycost$", "$ " + _priceCost);
                message.Body = message.Body.Replace("$dated$", DateTime.Now.ToShortDateString());
                message.Body = message.Body.Replace("$tax$", "$ " + gettaxAmount);
                message.Body = message.Body.Replace("$totalPrice$", "$ " + _totalPrice);
                message.Body = message.Body.Replace("$quantity$", Convert.ToString(model.Quantity));
                message.Body = message.Body.Replace("$address1$", model.DeliveryAddress1);
                message.Body = message.Body.Replace("$address2$", model.DeliveryAddress2);
                message.Body = message.Body.Replace("$emailId$", model.BuyerEmailId);
                message.Body = message.Body.Replace("$city$", model.City);
                message.Body = message.Body.Replace("$state$", model.State);
                message.Body = message.Body.Replace("$zipcode$", model.ZipCode);
                message.Body = message.Body.Replace("$itempath$", imagePath);
                message.Body = message.Body.Replace("$dollorsign$", dollerSign);
                message.Body = message.Body.Replace("my_hostings", domainName);                message.Body = message.Body.Replace("CurrentYear", currentYear);                message.Body = message.Body.Replace("UnsubscribeLink", UnsubscribertURL);


                message.IsBodyHtml = true;
                SendVia(Ioc.Get<IMailSendProvider>(), message, new SettingsDataService().GetEmailSendPropertyForRental());
                return true;            }            catch (Exception ex)            {                Ioc.Get<DbLogger>().LogException(LogSource.SendMail, string.Format("Error when sending verification message to email: {0}", model.BuyerEmailId), ex);

                return false;            }        }

        public bool SendForUserMessageAfterCovidRentalClose(string email, string buyerName)        {            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string currentYear = Convert.ToString(DateTime.Now.Year);
            string UnsubscribertURL = domainName + "/Account/NewsLetterUnSubscribe?Email=" + email;            try            {                var t = Ioc.Get<ITranslateController>();                var message = new Message();                if (_fromHeader != null)                    message.FromHeader = _fromHeader;                else                {                    message.FromHeader = new FromHeader()                    {
                        //From = Settings.GetSmtpFrom(),
                        From = Settings.GetEmailHelp(),                        FromName = Settings.GetSmtpFromName(),                        Subject = string.Format("{0}", t["Closed Order"])                    };                }                message.To = email;
                message.Body = MessageTemplate.GetMailTemplate(LanguageController.CurLang, "covidOrderClose");
                message.Body = message.Body.Replace("my_hostings", domainName);
                message.Body = message.Body.Replace("CurrentYear", currentYear);
                message.Body = message.Body.Replace("BuyerName", buyerName);
                message.Body = message.Body.Replace("UnsubscribeLink", UnsubscribertURL);

                message.IsBodyHtml = true;                return SendVia(Ioc.Get<IMailSendProvider>(), message, new SettingsDataService().GetEmailSendPropertyComingHelp());            }            catch (Exception ex)            {                Ioc.Get<DbLogger>().LogException(LogSource.SendMail, string.Format("Error when sending closed order message to email: {0}", email), ex);            }            return false;        }
    }
}
