using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Apeek.Common;
using Apeek.Common.Controllers;
using Apeek.Common.Extensions;
using Apeek.Common.Interfaces;
using Apeek.Common.Logger;
using Apeek.Common.Models;
using Apeek.Common.UrlHelpers;
using Apeek.Core.Services;
using Apeek.Core.Services.Impl;
using Apeek.Entities.Constants;
using Apeek.Entities.Entities;
using Apeek.ViewModels.Mappers;
using Apeek.ViewModels.Mappers.Imp;
using Apeek.ViewModels.Models;
using Apeek.ViewModels.Models.Impl;
using Momentarily.ViewModels.Models.Braintree;

namespace Apeek.Web.Framework.ControllerHelpers
{
    public class UserControllerHelper : BaseControllerHelper
    {
        public Shape<UserPwdModel> UserPwd(Shape<UserPwdModel> shape, ModelStateDictionary modelState)
        {
            if (modelState.IsValid)
            {
                var result = AccountDataService.SetUserPwd(shape.ViewModel, UserId.Value);
                if (result.CreateResult == CreateResult.Success)
                {
                    SendEmailChangePasswordMessage(result.Obj);

                    shape.IsError = false;
                    shape.Message = ViewErrorText.UserPwdChanged;
                }
                else
                {
                    Ioc.Get<IDbLogger>().LogError(LogSource.UserService, string.Format("Cannot Update User Pwd: {0}", shape.ViewModel));

                    shape.IsError = true;
                    shape.Message = ViewErrorText.UserPwdNoChanged;

                    modelState.AddModelError("ViewModel.NewPassword", "Cannot update");
                }
            }

            shape.PageName = PageName.UserPwd.ToString();

            return shape;
        }

        private bool SendEmailChangePasswordMessage(User user)
        {
            Ioc.Get<IDbLogger>().LogMessage(LogSource.User, string.Format("Trying to send change password email: {0}", user.Email));

            var sms = new SendMessageService();

            var res = sms.SendEmailChangePasswordMessage(user.Email,user.FirstName);

            if (res)
            {
                Ioc.Get<IDbLogger>().LogMessage(LogSource.User, string.Format("Change password mail was sent to address: email {0}; user id: {1}.", user.Email, user.Id));
            }
            else
            {
                Ioc.Get<IDbLogger>().LogError(LogSource.UserController, string.Format("Cannot send change password email when creating user: userid={0}", user.Id));
            }
            return res;
        }

        public UserUpdateModel GetUserUpdateModel()
        {
            if (UserId == null) return null;
            var userUpdateModel = AccountDataService.GetUserUpdateModel(UserId.Value);
            return userUpdateModel.CreateResult == CreateResult.Success ? userUpdateModel.Obj : null;
        }
        public List<Entities.Entities.Countries> getAllCountries()
        {
            List<Entities.Entities.Countries> countries = new List<Entities.Entities.Countries>(); ;
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
        public Shape<UserUpdateModel> UserProfilePost(Shape<UserUpdateModel> shape, ModelStateDictionary modelState)
        {
            shape.PageName = PageName.UserProfile.ToString();

            if (!modelState.IsValid)
            {
                shape.IsError = true;
                return shape;
            }
            if (!string.IsNullOrWhiteSpace(shape.ViewModel.Description))
                shape.ViewModel.Description = Regex.Replace(shape.ViewModel.Description, @"\r\n?|\n", "");

            var result = AccountDataService.Update(UserId.Value, shape.ViewModel);

            if (result.CreateResult == CreateResult.Success)
            {
                var imageModel = shape.ViewModel.UserImage;

                string userLogoImage = null;

                if (imageModel != null && imageModel.FileName != null &&
                    imageModel.FileName.TypeImage() == ImageType.Original.ToString())
                {
                    var refreshUserImageModel = new RefreshUserImageModel
                    {
                        Id = imageModel.Id,
                        UserId = UserId.Value,
                        FileName = imageModel.FileName,
                        ImgSettings = Ioc.Get<IImageSettings>().UserImageSizes
                    };

                    var resultImageUpdate = ImageDataService.RefreshUserImage(refreshUserImageModel);
                    if (resultImageUpdate.CreateResult != CreateResult.Success)
                    {
                        shape.IsError = true;
                        shape.Message = ViewErrorText.UserProfileNoChanged;

                        return shape;
                    }

                    var userImages = ImageDataService.GetUserImages(result.Obj.Id);

                    userLogoImage = userImages.MainImageUrlThumb(false);

                    shape.ViewModel.UserProfileImageUrl = userImages.MainImageUrlNormal(false);
                    shape.ViewModel.BigUserProfileImageUrl = userImages.MainImageUrlLarge(false);
                    shape.ViewModel.UserImage = EntityMapper<IUserImageMapper>.Mapper()
                        .Map(userImages.FirstOrDefault(p => p.Type == (int)ImageType.Normal), new UserImageModel());
                }
                else
                {
                    var userImages = ImageDataService.GetUserImages(result.Obj.Id);

                    shape.ViewModel.UserImage = EntityMapper<IUserImageMapper>.Mapper()
                        .Map(userImages.FirstOrDefault(p => p.Type == (int)ImageType.Normal), new UserImageModel());

                    if (!userImages.Any())
                    {
                        shape.ViewModel.UserProfileImageUrl = null;
                        shape.ViewModel.BigUserProfileImageUrl = null;
                    }
                        
                }

                UserAccess.UpdateUser(result.Obj.Email, result.Obj.FirstName, result.Obj.LastName, userLogoImage);

                shape.IsError = false;
                shape.Message = ViewErrorText.UserProfileChanged;
            }
            else
            {
                shape.IsError = true;
                shape.Message = ViewErrorText.UserProfileNoChanged;

                Ioc.Get<IDbLogger>().LogError(LogSource.UserService, string.Format("Cannot Update User Info: {0}", shape.ViewModel));
            }

            return shape;
        }

        public User VerifyUser(string vc)
        {
            Ioc.Get<IDbLogger>().LogMessage(LogSource.VerifyPerson, string.Format("Trying to verify person email, verification code: {0};", vc));

            User user;

            var res = AccountDataService.VerifyUser(vc, out user) ? CreateResult.Success : CreateResult.Error;

            if (res == CreateResult.Success)
            {
                var sms = new SendMessageService();
                var emailRes = false;
                if (user.Verified==true && user.IsMobileVerified==true)
                {
                    string ItemListURL = "/Search/Map?Location=&Latitude=0&Longitude=0&SearchByMap=false&NeLatitude=0&NeLongitude=0&SwLatitude=0&SwLongitude=0&DateStart=%2FDate(1575969961000)%2F&DateEnd=%2FDate(1577179561000)%2F&Page=1&PageSize=21&Radius=25&Keyword=&RentPeriod=1&SortBy=1";
                    emailRes = sms.SendEmailWelcomeTemplate(user.Email, user.FullName, ItemListURL);
                }
                //var emailRes = sms.SendEmailFollowUpMessage(user.Email,user.FirstName);

                if (emailRes)
                {
                    Ioc.Get<IDbLogger>().LogMessage(LogSource.User, string.Format("Follow up mail was sent to address: email {0}; user id: {1}.", user.Email, user.Id));
                }
                else
                {
                    Ioc.Get<IDbLogger>().LogError(LogSource.UserController, string.Format("Cannot send Follow up email when creating user: userid={0}", user.Id));
                }

                Ioc.Get<IDbLogger>().LogMessage(LogSource.VerifyPerson, string.Format("User was successfully verified, verification code: {0}", vc));

                return user;
            }

            Ioc.Get<IDbLogger>().LogWarning(LogSource.VerifyPerson, string.Format("User was not verified, verification code: {0};", vc));

            return null;
        }

        public bool VerifyMobile(string otp, string vc)
        {
            Ioc.Get<IDbLogger>().LogMessage(LogSource.VerifyPerson, string.Format("Trying to verify person email, verification code: {0};", vc));
            var sms = new SendMessageService();
            var res = AccountDataService.VerifyMobile(otp,vc);
            var user = AccountDataService.GetUser(vc);
            UserAccess.AuthenticateUser(user.Id);
            if (user.Verified == true && user.IsMobileVerified == true)
                {
                    string ItemListURL = "/Search/Map?Location=&Latitude=0&Longitude=0&SearchByMap=false&NeLatitude=0&NeLongitude=0&SwLatitude=0&SwLongitude=0&DateStart=%2FDate(1575969961000)%2F&DateEnd=%2FDate(1577179561000)%2F&Page=1&PageSize=21&Radius=25&Keyword=&RentPeriod=1&SortBy=1";
                    var emailRes = sms.SendEmailWelcomeTemplate(user.Email, user.FullName, ItemListURL);
                }
            return res;
        }

        public bool VerifyMobileLink(string vc)
        {
            var sms = new SendMessageService();
            var res = AccountDataService.VerifyMobileLink(vc);
            var user = AccountDataService.GetUser(vc);
            UserAccess.AuthenticateUser(user.Id);
            if (user.Verified == true && user.IsMobileVerified == true)
            {
                string ItemListURL = "/Search/Map?Location=&Latitude=0&Longitude=0&SearchByMap=false&NeLatitude=0&NeLongitude=0&SwLatitude=0&SwLongitude=0&DateStart=%2FDate(1575969961000)%2F&DateEnd=%2FDate(1577179561000)%2F&Page=1&PageSize=21&Radius=25&Keyword=&RentPeriod=1&SortBy=1";
                var emailRes = sms.SendEmailWelcomeTemplate(user.Email, user.FullName, ItemListURL);
            }
            return res;
        }


        public UserEmailModel GetUserEmailModel()
        {
            return UserId == null ? null : AccountDataService.GetUserEmailModel(UserId.Value);
        }

        public Shape<UserEmailModel> UserEmail(Shape<UserEmailModel> shape, ModelStateDictionary modelState)
        {
            shape.PageName = PageName.UserEmail.ToString();
            shape.ViewModel.Result = CreateResult.EmailChangeNotificationSendError;

            shape.IsError = true;
            shape.Message = ViewErrorText.UserEmailNoChanged;

            if (modelState.IsValid)
            {
                if (string.Compare(shape.ViewModel.OldUserEmail, shape.ViewModel.UserEmail,
                        StringComparison.OrdinalIgnoreCase) == 0)
                {
                    shape.IsError = true;
                    shape.Message = ViewErrorText.UserEmailNoChangedSameEmails;

                    return shape;
                }

                var userData = AccountDataService.GetUser(UserId.Value);
                var user = AccountDataService.GetUserByEmail(shape.ViewModel.UserEmail);

                if (user == null || user.Id == UserId.Value)
                {
                    if (modelState.IsValid)
                    {
                        var request =
                            AccountDataService.CreateUserSecurityDataChangeRequest(
                                UserSecurityDataType.Email, shape.ViewModel.UserEmail, shape.ViewModel.OldUserEmail, UserId.Value);

                        if (request != null)
                        {
                            var sms = Ioc.Get<ISendMessageService>();

                            if (sms.SendEmailVerifySecurityDataMessage(request, userData.FirstName + " " + userData.LastName))
                            {
                                shape.ViewModel.Result = CreateResult.EmailChangeNotificationSendSuccess;

                                shape.IsError = false;
                                shape.Message = string.Format(ViewErrorText.UserEmailSendChangeMessage, shape.ViewModel.OldUserEmail);
                            }
                            else
                            {
                                Ioc.Get<IDbLogger>().LogError(LogSource.UserService, string.Format("Cannot Send Email VerifySecurityData  Email: {0}; User: {1}", shape.ViewModel.UserEmail, UserId.Value));
                            }
                        }
                        else
                        {
                            Ioc.Get<IDbLogger>().LogError(LogSource.UserService, string.Format("Cannot Update Email: {0} for User: {1}", shape.ViewModel.UserEmail, UserId.Value));

                            modelState.AddModelError("ViewModel.UserEmail", "Cannot update");
                        }
                    }
                }
                else
                {
                    modelState.AddModelError("ViewModel.UserEmail", "Email exists");
                }
            }
            return shape;
        }

        public VerifyResult VerifySecurityRequest(string vc)
        {
            return AccountDataService.VerifyAndDoUserSecurityDataChangeRequest(vc);
        }

        public bool ResendEmailActivation(int userId, QuickUrl quickUrl, HttpRequestBase request)        {            var user = AccountDataService.GetUser(userId);            if (user != null)            {                AccountDataService.RegenerateVerificationCode(user);                Ioc.Get<IDbLogger>()                    .LogMessage(LogSource.User, string.Format("Trying to send verification email: {0}", user.Email));                var service = new SendMessageService();                var verificationUrl = quickUrl.VerifyUserUrl(user.VerificationCode, request.Url.Scheme);                var res = service.SendEmailQuickAccountActivationMessage(user.Email, verificationUrl, user.FirstName);                if (res)                {                    Ioc.Get<IDbLogger>()                        .LogMessage(LogSource.User,                            string.Format("Verification mail was resent to address: email {0}; person id: {1}.",                                user.Email, user.Id));                }                else                {                    Ioc.Get<IDbLogger>()                        .LogError(LogSource.UserController,                            string.Format("Cannot resend verification email: personid={0}", user.Id));                }                return res;            }            return false;        }        public MessageSentModel ResendOTPActivation(int userId, QuickUrl quickUrl, HttpRequestBase request)        {            var user = AccountDataService.GetUser(userId);            MessageSentModel model = new MessageSentModel();            if (user != null)            {                AccountDataService.RegenerateVerificationCode(user);                Ioc.Get<IDbLogger>()                    .LogMessage(LogSource.User, string.Format("Trying to send OTP: {0}", user.Email));                var service = new SendMessageService();                var verificationUrl = quickUrl.VerifyUserUrl(user.VerificationCode, request.Url.Scheme);                var res = service.SendEmailQuickAccountActivationMessage(user.Email, verificationUrl, user.FirstName);                model.IsExternal = false;                model.VC = user.VerificationCode;                model.PhoneNumber = AccountDataService.GetUserPhone(userId);            }            return model;        }        public MessageSentModel ResendActivation(int userId, QuickUrl quickUrl, HttpRequestBase request)
        {
            var olduser = AccountDataService.GetUser(userId);
            MessageSentModel model = new MessageSentModel();
            if (olduser != null)            {                AccountDataService.RegenerateVerificationCode(olduser);
            var user = AccountDataService.GetUser(userId);
                if (user.Verified==false)
                {
                    var service = new SendMessageService();
                    var verificationUrl = quickUrl.VerifyUserUrl(user.VerificationCode, request.Url.Scheme);
                    var res = service.SendEmailQuickAccountActivationMessage(user.Email, verificationUrl, user.FirstName);
                }                model.VC = user.VerificationCode;                model.IsVerified = user.Verified;                model.IsMobileVerified = user.IsMobileVerified;                model.Email = user.Email;
                model.PhoneNumber = AccountDataService.GetUserPhone(userId);                if (user.GoogleId==null && user.FacebookId==null)
                {
                    model.IsExternal = false;
                }                else
                {
                    model.IsExternal = true;
                }                return model;            }
            return null;
        }        public User GetUser()        {            return UserId == null ? null : AccountDataService.GetUser(UserId.Value);        }

        public User GetUserByVC(string code)        {            var user= AccountDataService.GetUser(code);
            return user;        }

        public bool DeleteAccount()        {            bool result = false;            int userid = 0;            int.TryParse(Convert.ToString(UserId.Value), out userid);            if (userid != 0)            {                var user = AccountDataService.DeleteUser(userid);                if (user != null)                {                    result = true;                }            }            return result;        }        public bool DeleteUserAccount(int userid)        {            bool result = false;            var user = AccountDataService.DeleteUser(userid);            if (user != null)            {                result = true;            }            return result;        }

        public Result<PayoutDetailsModel> CreatePaypalInfoPaymentDetail(Shape<PayoutDetailsModel> shape)        {
            //PaypalInfoPaymentDetail result = null;
            var result = new Result<PayoutDetailsModel>(CreateResult.Error, new PayoutDetailsModel());            result = AccountDataService.CreatePaypalInfoPaymentDetail(shape);

            return result;        }        public Result<PayoutDetailsModel> GetCurrentPaypalInfoPaymentDetail(int userID)        {
            //PaypalInfoPaymentDetail result = null;
            var result = new Result<PayoutDetailsModel>(CreateResult.Error, new PayoutDetailsModel());            result = AccountDataService.GetCurrentPaypalInfoPaymentDetail(userID);            return result;        }

        public List<Entities.Entities.GlobalCodes> getPaymentTypes()        {            List<Entities.Entities.GlobalCodes> types = new List<Entities.Entities.GlobalCodes>(); ;            try            {                types = AccountDataService.GetPaymentTypes();                return types;            }            catch (Exception ex)            {                throw ex;            }        }

        public void LogOff()
        {
            Ioc.Get<IDbLogger>().LogMessage(LogSource.Account, "Tring to log off");
            FormsAuthentication.SignOut();
            UserAccess.SignOutUser();
            Ioc.Get<IDbLogger>().LogMessage(LogSource.Account, "Log off performed successfully. Redirecting ...");
        }
    }
}