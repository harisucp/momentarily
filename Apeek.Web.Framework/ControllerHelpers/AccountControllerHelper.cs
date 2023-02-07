using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Apeek.Common;
using Apeek.Common.Controllers;
using Apeek.Common.Logger;
using Apeek.Common.Models;
using Apeek.Common.UrlHelpers;
using Apeek.Core.Services.Impl;
using Apeek.Entities.Entities;
using Apeek.Entities.Validators;
using Apeek.Entities.Web;
using Apeek.Messaging.Implementations.Email;
using Apeek.Messaging.Implementations.Turbosms;
using Apeek.Messaging.Interfaces;
using Apeek.ViewModels.Models;
using Sendy.Net;

namespace Apeek.Web.Framework.ControllerHelpers
{
    public class AccountControllerHelper<TRegisterModel> : BaseControllerHelper where TRegisterModel : class, IRegisterModel
    {
        Subscription _subscriptionService;
        Campaign _campaignService;
       
        public bool ExternalLoginError { get { return ContextService.FromQueryString.ExternalLoginError; } }

        public User Register(Shape<TRegisterModel> shape, QuickUrl quickUrl, ModelStateDictionary modelState, HttpRequestBase request)
        {
            //if (shape.ViewModel.IsExternal == true)
            //{
            //    if (modelState.ContainsKey("ViewModel.Password"))
            //        modelState["ViewModel.Password"].Errors.Clear();

            //    if (modelState.ContainsKey("ViewModel.ConfirmPassword"))
            //        modelState["ViewModel.ConfirmPassword"].Errors.Clear();
            //}
            if (!modelState.IsValid) return null;
            Ioc.Get<IDbLogger>().LogMessage(LogSource.Account, "Trying to register using regular method");
            var model = shape.ViewModel as IRegisterModel;
            if (model == null) return null;
            if (model.Address != null)
            {
                model.Address.PhoneNumberRecords.Add(new PhoneNumber
                {
                    CountryCode = model.CountryCode,
                    PhoneNum = model.PhoneNumber
                });
            }
            var registerResult = AccountDataService.Register(shape.ViewModel as IRegisterModel, () => new User()
            {
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                FullName=model.FirstName+" "+model.LastName,
                Url = new UrlGenerator().CreateUrl(model.FirstName + " " + model.LastName, null, LanguageController.CurLang),
                DateOfBirth = model.DateOfBirthday,
                GoogleId = model.GoogleId,
                FacebookId = model.FacebookId,
                IgnoreMarketingEmails = model.IgnoreMarketingEmails,
                IsMobileVerified=model.IsMobileVerified,
                IsAdmin = model.IsAdmin,
                CreatedDate = DateTime.Now
            });
            if (registerResult.CreateResult == CreateResult.Duplicate || registerResult.CreateResult == CreateResult.Error)
            {

                if (registerResult.Message == "That email address is already in use" || registerResult.Message == "Please, enter valid e-mail address")
                {
                    modelState.AddModelError("ViewModel.Email", registerResult.Message);

                }
                else
                {
                    modelState.AddModelError("ViewModel.PhoneNumber", registerResult.Message);
                }
            }

            if (registerResult.CreateResult == CreateResult.Success)
            {
                AccountDataService.RegenerateVerificationCode(registerResult.Obj);
                var result = AccountDataService.AddPhoneNumber(registerResult.Obj.Id, shape.ViewModel.PhoneNumber,Convert.ToString(shape.ViewModel.CountryId));
                SendEmailAccountActivationMessage(registerResult.Obj, quickUrl, request);
                return registerResult.Obj;
            }
            return null;
        }

        public User RegisterWithOTP(Shape<TRegisterModel> shape, QuickUrl quickUrl, ModelStateDictionary modelState, HttpRequestBase request)
        {
            if (shape.ViewModel.IsExternal == true)
            {
                if (modelState.ContainsKey("ViewModel.Password"))
                    modelState["ViewModel.Password"].Errors.Clear();
                if (modelState.ContainsKey("ViewModel.ConfirmPassword"))
                    modelState["ViewModel.ConfirmPassword"].Errors.Clear();
                // modelState.Remove(shape.ViewModel.Password);
                // modelState.Remove(shape.ViewModel.ConfirmPassword);
            }
            if (!modelState.IsValid) return null;
            Ioc.Get<IDbLogger>().LogMessage(LogSource.Account, "Trying to register using regular method");
            var model = shape.ViewModel as IRegisterModel;
            if (model == null) return null;

            if (model.Address != null)
            {
                model.Address.PhoneNumberRecords.Add(new PhoneNumber
                {
                    CountryCode = model.CountryCode,
                    PhoneNum = model.PhoneNumber
                });
            }
            var registerResult = AccountDataService.ValidatePhonenumber(shape.ViewModel as IRegisterModel, () => new User()
            {
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                FullName = model.FirstName + " " + model.LastName,
                DateOfBirth = model.DateOfBirthday,
                GoogleId = model.GoogleId,
                FacebookId = model.FacebookId,
                IgnoreMarketingEmails = model.IgnoreMarketingEmails,
                IsAdmin = model.IsAdmin,
                IsBlocked = model.IsBlocked,
                IsMobileVerified = model.IsMobileVerified
            });
            if (registerResult.CreateResult == CreateResult.Duplicate)
            {
                modelState.AddModelError("ViewModel.PhoneNumber", "The Phone Number  is already in use");
            }
            if (registerResult.CreateResult == CreateResult.Success)
            {
                // AccountDataService.RegenerateVerificationCode(registerResult.Obj);
                return registerResult.Obj;
            }
            return null;
        }
        public Shape<UserRestorePwd> RestorePwd(Shape<UserRestorePwd> shape, QuickUrl quickUrl, ModelStateDictionary modelState)
        {
            if (modelState.IsValid)
            {
                Ioc.Get<IDbLogger>().LogMessage(LogSource.Account, "Trying to restore pwd for: {0}", shape.ViewModel.EmailAddressOrPhoneNum);

                var phoneNumberValidator = new ValidatorEmailOrPhoneNum();
                if (phoneNumberValidator.IsValid(shape.ViewModel.EmailAddressOrPhoneNum))
                {
                    User user = null;
                    IMessageSendProvider messageSendProvider = null;
                    ISendProperty sendProperty = null;
                    IMessage message = null;
                    var sendMessageService = new SendMessageService();
                    var model = new MessageSentModel();
                    if (!string.IsNullOrWhiteSpace(phoneNumberValidator.EmailAddress))
                    {
                        user = AccountDataService.GetUserByEmail(phoneNumberValidator.EmailAddress);
                        if (user != null)
                        {
                            model.Email = phoneNumberValidator.EmailAddress;
                            model.IsEmailMessageSent = true;

                            messageSendProvider = new MailSendProvider();
                            sendProperty = new SettingsDataService().GetEmailSendProperty();
                            user.SendLinkDate = DateTime.Now;
                            AccountDataService.RegenerateVerificationCode(user);
                            //var loginUrl = quickUrl.QuickLoginUrl(user.Email, user.VerificationCode, quickUrl.UserPwdUrl());
                            var loginUrl = quickUrl.QuickLoginUrl2(user.Email, user.VerificationCode, quickUrl.UserPwdUrl2());
                            message = sendMessageService.GetEmailRestorePwdMessage(user, loginUrl);
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(phoneNumberValidator.PhoneNumber))
                    {
                        user = AccountDataService.GetUserByPhoneNumber(phoneNumberValidator.PhoneNumber);
                        if (user != null)
                        {
                            model.PhoneNumber = string.Format("+38{0}", phoneNumberValidator.PhoneNumber);
                            model.IsSmsMessageSent = true;

                            messageSendProvider = new SmsSendProvider();
                            sendProperty = new SettingsDataService().GetSmsSendProperty();
                            AccountDataService.GenerateTempPwd(user);
                            message = sendMessageService.GetSmsRestorePwdMessage(model.PhoneNumber, user.TempPwd);
                        }
                    }

                    if (user != null)
                    {
                        try
                        {
                            if (sendMessageService.SendVia(messageSendProvider, message, sendProperty))
                            {
                                Ioc.Get<IDbLogger>().LogMessage(LogSource.Account, "Tring to restore pwd for: {0}. Restore message has been sent.", shape.ViewModel.EmailAddressOrPhoneNum);

                                shape.IsError = false;

                                shape.Message = shape.ViewModel.EmailAddressOrPhoneNum;


                                //  shape.Message = string.Format("A link to reset your password has been sent to {0} "+

                                //"If you have not received the e - mail after a few minutes, " +
                                //"please check your spam folder.", shape.ViewModel.EmailAddressOrPhoneNum);
                            }
                            else
                            {
                                Ioc.Get<IDbLogger>().LogError(LogSource.Account, "Cannot restore pwd. Cannot send restore message to: {0}.", shape.ViewModel.EmailAddressOrPhoneNum);

                                modelState.AddModelError("ViewModel.EmailAddressOrPhoneNum", "Cannot send email");
                            }
                        }
                        catch (Exception ex)
                        {
                            Ioc.Get<IDbLogger>().LogException(LogSource.Account, string.Format("Error when restoring pwd. User data: {0}", shape.ViewModel.EmailAddressOrPhoneNum), ex);
                            modelState.AddModelError("ViewModel.EmailAddressOrPhoneNum", "Server error");
                        }
                    }
                    else
                    {
                        Ioc.Get<IDbLogger>().LogError(LogSource.Account, "Cannt restore pwd. Cannot find use: {0}.", shape.ViewModel.EmailAddressOrPhoneNum);

                        modelState.AddModelError("ViewModel.EmailAddressOrPhoneNum", "Cannot find user");
                    }
                }
                return shape;
            }
            shape.IsError = true;
            shape.Message = "Error.";
            return shape;
        }

        public LoginResult Login(Shape<LoginModel> shape, string returnUrl, ModelStateDictionary modelState)
        {
            if (modelState.IsValid)
            {
                Ioc.Get<IDbLogger>().LogMessage(LogSource.Account, "Trying to login using regular method");
                var loginResult = AccountDataService.Login(shape.ViewModel.EmailAddressOrPhoneNum, shape.ViewModel.Password);
                return loginResult;
            }
            return null;
        }

        public LoginResult GetBlocked(Shape<LoginModel> shape, string returnUrl, ModelStateDictionary modelState)
        {
            if (modelState.IsValid)
            {
                Ioc.Get<IDbLogger>().LogMessage(LogSource.Account, "Trying to get blocked user");
                var loginResult = AccountDataService.GetBlockedUser(shape.ViewModel.EmailAddressOrPhoneNum, shape.ViewModel.Password);
                return loginResult;
            }
            return null;
        }

        public LoginResult AdminLogin(Shape<LoginModel> shape, string returnUrl, ModelStateDictionary modelState)
        {
            if (modelState.IsValid)
            {
                Ioc.Get<IDbLogger>().LogMessage(LogSource.Account, "Trying to login using regular method");
                var loginResult = AccountDataService.AdminLogin(shape.ViewModel.EmailAddressOrPhoneNum, shape.ViewModel.Password);
                return loginResult;
            }
            return null;
        }

        public User GetGoogleUser(string googleId, string email)
        {
            Ioc.Get<IDbLogger>().LogMessage(LogSource.Account, "Trying to login using Google method");
            var loginResult = AccountDataService.GetGoogleUser(googleId, email);
            return loginResult;
        }
        public User GetFacebookUser(string facebookId, string email)
        {
            Ioc.Get<IDbLogger>().LogMessage(LogSource.Account, "Trying to login using Facebook method");
            var loginResult = AccountDataService.GetFacebookUser(facebookId, email);
            return loginResult;
        }

        public User QuickLogin(string ue, string vc, string redirectTo)
        {
            Ioc.Get<IDbLogger>().LogMessage(LogSource.Account, "Tring to login using quick login method");

            var user = AccountDataService.GetUser(vc, ue);
            if (user != null) return user;

            Ioc.Get<IDbLogger>().LogWarning(LogSource.Account, "Cannot login using quick login method. email:{0}; vc:{1}; redirectto:{2}", ue, vc, redirectTo);
            return null;
        }

        public void LogOff()
        {
            Ioc.Get<IDbLogger>().LogMessage(LogSource.Account, "Tring to log off");

            FormsAuthentication.SignOut();
            UserAccess.SignOutUser();

            Ioc.Get<IDbLogger>().LogMessage(LogSource.Account, "Log off performed successfully. Redirecting ...");
        }

        public bool SendEmailAccountActivationMessage(User user, QuickUrl quickUrl, HttpRequestBase request)
        {
            Ioc.Get<IDbLogger>().LogMessage(LogSource.User, string.Format("Trying to send verification email: {0}", user.Email));

            var service = new SendMessageService();
            //var verificationUrl = quickUrl.VerifyUserUrl(user.VerificationCode, request.Url.Scheme);
            var verificationUrl = quickUrl.WelcomeUserUrl(user.VerificationCode, request.Url.Scheme);

            var res = service.SendEmailQuickAccountActivationMessage(user.Email, verificationUrl, user.FirstName);

            if (res)
            {
                Ioc.Get<IDbLogger>().LogMessage(LogSource.User, string.Format("Verification mail was sent to address: email {0}; person id: {1}.", user.Email, user.Id));
            }
            else
            {
                Ioc.Get<IDbLogger>().LogError(LogSource.UserController, string.Format("Cannot send verification email when creating user: personid={0}", user.Id));
            }

            return res;
        }

        public string subscribeEmail(string listId, string email, string name, bool plaintext)
        {

            try
            {
                _subscriptionService = new Subscription();
                string result = _subscriptionService.Subscribe(listId, email, name, plaintext);
                return result;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public string unsubscribeEmail(string listId, string email, string name, bool plaintext)
        {
            try
            {
                _subscriptionService = new Subscription();
                string result = _subscriptionService.Unsubscribe(listId, email, plaintext);
                return result;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public StatusCode getsubscriberStatus(string listId, string email)
        {
            try
            {
                _subscriptionService = new Subscription();
                StatusCode status = _subscriptionService.Status(listId, email);
                return status;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int getactiveCount(string listId)
        {
            try
            {
                _subscriptionService = new Subscription();
                int count = _subscriptionService.ActiveSubscriberCount(listId);
                return count;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int createAndSendCampaign(string brandEmailAddress, string subject, string fromName, string plainText, string html)
        {

            try
            {
                _campaignService = new Campaign();
                html = MessageTemplate.GetMailTemplate(LanguageController.CurLang, "LaunchingSoonCampiegn");
                //html = MessageTemplate.GetMailTemplate(LanguageController.CurLang, "Thankyou");
               // html = MessageTemplate.GetMailTemplate(LanguageController.CurLang, "invoice");
                int campaignid = _campaignService.Create(brandEmailAddress, subject, fromName, plainText, html,"");
                return campaignid;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string getCampaignList(string brandEmailAddress)
        {

            try
            {
                _campaignService = new Campaign();

                string list = _campaignService.List(brandEmailAddress);
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool saveSubscriber(string email)
        {
            try
            {
                bool data = AccountDataService.SubscriberAdd(email);
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool UpdateSubscriber(string email)
        {
            try
            {
                bool data = AccountDataService.SubscriberUpdate(email);
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool UpdateSubscriberUnsubscriber(string email ,bool status)
        {
            try
            {
                bool data = AccountDataService.SubscriberUnsubscribedUpdate(email, status);
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool ExsistSubscriber(string email)
        {
            try
            {
                bool data = AccountDataService.SubscriberAlreadyExsist(email);
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool ExsistSubscriberOnlyEmail(string email)
        {
            try
            {
                bool data = AccountDataService.SubscriberAlreadyExsistOnlyEmail(email);
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string ExsistSubscriberGetName(string email)
        {
            try
            {
                string data = AccountDataService.SubscriberNameGetIfExsist(email);
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Countries> getAllCountries()
        {
            List<Countries> countries = new List<Countries>(); ;
            try
            {
                countries = AccountDataService.GetAllCountries();
                return countries;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
