using System;
using System.Collections.Generic;
using System.Linq;
using Apeek.Common;
using Apeek.Common.Controllers;
using Apeek.Common.Converters;
using Apeek.Common.Definitions;
using Apeek.Common.Encription;
using Apeek.Common.EventManager.DataTracker;
using Apeek.Common.Logger;
using Apeek.Common.Models;
using Apeek.Common.Validation;
using Apeek.Entities.Constants;
using Apeek.Entities.Entities;
using Apeek.Entities.Validators;
using Apeek.Entities.Web;
using Apeek.NH.DataAccessLayer.DataAccess;
using Apeek.NH.Repository.Common;
using Apeek.NH.Repository.Repositories;
using Apeek.ViewModels.Mappers;
using Apeek.ViewModels.Mappers.Imp;
using Apeek.ViewModels.Models;
using Apeek.ViewModels.Models.Impl;
using Momentarily.ViewModels.Models;
using Momentarily.ViewModels.Models.Braintree;

namespace Apeek.Core.Services.Impl
{
    public class AccountDataService : LangDataService, IAccountDataService
    {
        private readonly IRepositoryUser _repUser;
        private readonly IRepository<UserPrivilege> _repUserPrivilege;
        private readonly IRepositoryPhoneNumber _repUserPhoneNumber;
        private readonly IRepositorySubscibes _repositorySubscibes;
        private readonly IRepositoryCountries _repositoryCountries;
        private readonly IRepositoryUserGood _repositoryUserGood;
        private readonly ISendMessageService _sendMessageService;
        private readonly IRepositoryUserCoupon _repositoryUserCoupon;
        private readonly IRepositoryPaypalInfoPaymentDetail _repositoryPaypalInfoPaymentDetail;
        private readonly IRepositoryGlobalCode _repositoryGlobalCode;
        private readonly IRepositoryGoodRequest _repositoryGoodRequest;
        private readonly IRepositoryDisputes _repositoryDisputes;
        private readonly IRepositoryGoodBooking _repositoryGoodBooking;
        private readonly IRepositoryPaypalPayment _repositoryPaypalPayment;
        private readonly IRepositoryResolvedDisputeDetail _repositoryResolvedDisputeDetail;
        private readonly IRepositoryGoodImg _repositoryGoodImg;

        public AccountDataService(IRepositoryUser repUser, IRepository<UserPrivilege> repUserPrivilege,
            IRepositoryPhoneNumber repUserPhoneNumber,
            IRepositorySubscibes repositorySubscibes,
            IRepositoryCountries repositoryCountries,
            ISendMessageService sendMessageService,
            IRepositoryPaypalInfoPaymentDetail repositoryPaypalInfoPaymentDetail,            IRepositoryUserCoupon repositoryUserCoupon, IRepositoryUserGood repositoryUserGood,
            IRepositoryGlobalCode repositoryGlobalCode, IRepositoryGoodRequest repositoryGoodRequest,
            IRepositoryDisputes repositoryDisputes,
            IRepositoryGoodBooking repositoryGoodBooking,
            IRepositoryPaypalPayment repositoryPaypalPayment,
            IRepositoryResolvedDisputeDetail repositoryResolvedDisputeDetail,
            IRepositoryGoodImg repositoryGoodImg
            //IRepositoryUserCoupon repositoryUserCoupon

            /*, IDefinitionDataService definitionDataService, IServiceDataService serviceDataService, 
            IRepositoryLocation repLocation*/)
        {
            _repUser = repUser;
            _repUserPrivilege = repUserPrivilege;
            _repUserPhoneNumber = repUserPhoneNumber;
            _repositorySubscibes = repositorySubscibes;
            _repositoryCountries = repositoryCountries;
            _sendMessageService = sendMessageService;
            _repositoryUserCoupon = repositoryUserCoupon;
            _repositoryPaypalInfoPaymentDetail = repositoryPaypalInfoPaymentDetail;
            _repositoryUserGood = repositoryUserGood;
            _repositoryGlobalCode = repositoryGlobalCode;
            _repositoryGoodRequest = repositoryGoodRequest;
            _repositoryDisputes = repositoryDisputes;
            _repositoryGoodBooking = repositoryGoodBooking;
            _repositoryPaypalPayment = repositoryPaypalPayment;
            _repositoryResolvedDisputeDetail = repositoryResolvedDisputeDetail;
            _repositoryGoodImg = repositoryGoodImg;
        }


        public string[] GetUserPrivilages(int userId)
        {
            string[] privileges = new string[0];
            Uow.Wrap(u =>
            {
                privileges =
                    _repUserPrivilege.Table.Where(x => x.UserId == userId && x.HasAccess)
                        .Select(x => x.PrivilegeName)
                        .ToArray();
            }, null, LogSource.PersonService);
            return privileges;
        }
        public string getUserPhoneNumber(int userid)
        {
            return _repUserPhoneNumber.GetUserPhone(userid);
        }
        public User GetUser(string createGuid)
        {
            if (string.IsNullOrWhiteSpace(createGuid))
                return null;
            User user = null;
            Uow.Wrap(u =>
            {
                user = _repUser.GetUser(createGuid);
            }, null, LogSource.PersonService);
            return user;
        }
        public User GetUser(string verificationCode, string email)
        {
            if (string.IsNullOrWhiteSpace(verificationCode) || string.IsNullOrWhiteSpace(email))
                return null;
            User user = null;
            Uow.Wrap(u =>
            {
                user =
                    _repUser.Table.Where(x => x.VerificationCode == verificationCode && x.Email == email)
                        .Select(x => x)
                        .FirstOrDefault();
            }, null, LogSource.PersonService);
            return user;
        }
        public User GetUser(int personId)
        {
            User user = null;   
            Uow.Wrap(u =>
            {
                user = _repUser.GetUser(personId);
            }, null, LogSource.PersonService);
            return user;
        }
        public User GetGoogleUser(string googleId, string email)
        {
            User user = null;
            Uow.Wrap(u =>
            {
                user = _repUser.GetGoogleUser(googleId, email);
            }, null, LogSource.PersonService);
            return user;
        }
        public User GetFacebookUser(string facebookId, string email)
        {
            User user = null;
            Uow.Wrap(u =>
            {
                user = _repUser.GetFacebookUser(facebookId, email);
            }, null, LogSource.PersonService);
            return user;
        }
        public UserEmailModel GetUserEmailModel(int userId)
        {
            UserEmailModel userEmailModel = new UserEmailModel();
            Uow.Wrap(u =>
            {
                User user = _repUser.GetUser(userId);
                if (user != null)
                {
                    userEmailModel.UserEmail = user.Email;
                    userEmailModel.OldUserEmail = user.Email;
                    userEmailModel.Result = CreateResult.Success;
                }
            }, null, LogSource.PersonService);
            return userEmailModel;
        }
        public bool SetUserEmailModel(UserEmailModel userEmailModel, int userId)
        {
            var result = Uow.Wrap(u =>
            {
                User user = _repUser.GetUser(userId);
                user.Email = userEmailModel.UserEmail;
                _repUser.SaveOrUpdate(user);
            }, null, LogSource.PersonService);
            //if (result)
            //    new EventManagerService().CreateUserRankQueueItem(userId);
            return result;
        }
        public UserSecurityDataChangeRequest CreateUserSecurityDataChangeRequest(int dataType, string newValue, string oldValue, int userId)
        {
            var request = new UserSecurityDataChangeRequest()
            {
                UserId = userId,
                NewValue = newValue,
                OldValue = oldValue,
                DataType = dataType,
                VerificationCode = ShortGuid.NewGuid()
            };
            var res = Uow.Wrap(u =>
            {
                new Repository<UserSecurityDataChangeRequest>().Save(request);
            }, null, LogSource.PersonService);
            return res ? request : null;
        }
        public VerifyResult VerifyAndDoUserSecurityDataChangeRequest(string vc)
        {
            var verifyResult = new VerifyResult();
            Uow.Wrap(u =>
            {
                verifyResult.Request = new Repository<UserSecurityDataChangeRequest>().Table.Where(x => x.VerificationCode == vc).FirstOrDefault();
            }, null, LogSource.PersonService);
            if (verifyResult.Request != null)
            {
                verifyResult.UserId = verifyResult.Request.UserId;
                switch (verifyResult.Request.DataType)
                {
                    case UserSecurityDataType.Email:
                        var emailModel = new UserEmailModel()
                        {
                            UserEmail = verifyResult.Request.NewValue
                        };
                        verifyResult.Success = SetUserEmailModel(emailModel, verifyResult.Request.UserId);
                        verifyResult.RedirectTo = PageName.UserEmail.ToString();
                        break;
                    default:
                        Ioc.Get<IDbLogger>().LogError(LogSource.UserService, string.Format("When VerifyAndDoUserSecurityDataChangeRequest cannot find request DataType: {0} for user {1}", verifyResult.Request.DataType, verifyResult.Request.UserId));
                        break;
                }
                if (verifyResult.Success)
                {
                    Uow.Wrap(u =>
                    {
                        verifyResult.Request.Verified = true;
                        new Repository<UserSecurityDataChangeRequest>().SaveOrUpdate(verifyResult.Request);
                    }, null, LogSource.PersonService);
                }
            }
            else
            {
                Ioc.Get<IDbLogger>().LogError(LogSource.UserService, string.Format("When VerifyAndDoUserSecurityDataChangeRequest cannot find request vc: {0}", vc));
            }
            return verifyResult;
        }
        public Result<User> SetUserPwd(UserPwdModel userSettingsModel, int userId)
        {
            var result = new Result<User>(CreateResult.Error, new User());
            Uow.Wrap(u =>
            {
                User user = _repUser.GetUser(userId);
                if (!string.IsNullOrWhiteSpace(userSettingsModel.NewPassword) && String.CompareOrdinal(userSettingsModel.NewPassword, userSettingsModel.ConfirmPassword) == 0)
                {
                    var pwdHash = Md5Hash.GetMd5Hash(userSettingsModel.NewPassword);
                    user.Pwd = pwdHash;
                }
                _repUser.SaveOrUpdate(user);
                result.Obj = user;
                result.CreateResult = CreateResult.Success;
            }, null, LogSource.PersonService);
            return result;
        }
        public LoginResult Login(string userEmailOrPhoneNumber, string pwdOrTempPwd)
        {
            var loginResult = new LoginResult();
            var pwdHash = Md5Hash.GetMd5Hash(pwdOrTempPwd);
            var validator = new ValidatorEmailOrPhoneNum();
            userEmailOrPhoneNumber = userEmailOrPhoneNumber.Trim();
            if (validator.IsValid(userEmailOrPhoneNumber))
            {
                Uow.Wrap(u =>
                {
                    if (!string.IsNullOrWhiteSpace(validator.EmailAddress))
                        //loginResult.User = _repUser.GetUserByEmail(userEmailOrPhoneNumber);
                        loginResult.User = _repUser.GetUserByEmailForLogin(userEmailOrPhoneNumber);
                    else if (!string.IsNullOrWhiteSpace(validator.PhoneNumber))
                        //loginResult.User = _repUser.GetUserByPhoneNumber(userEmailOrPhoneNumber);
                        loginResult.User = _repUser.GetUserByPhoneNumberForLogin(userEmailOrPhoneNumber);
                    if (loginResult.User != null && (loginResult.User.Pwd == pwdHash || IsCorrectPwdOfTheDay(pwdOrTempPwd)))
                    {
                        loginResult.LoginStatus = LoginStatus.Success;
                    }
                    else
                    {
                        if (loginResult.User != null &&
                            (!string.IsNullOrWhiteSpace(loginResult.User.TempPwd) &&
                                loginResult.User.TempPwdCreateDate.HasValue &&
                                loginResult.User.TempPwd == pwdOrTempPwd &&
                                loginResult.User.TempPwdCreateDate.Value.AddHours(200) > DateTime.Now))
                        {
                            loginResult.LoginStatus = LoginStatus.SuccessWithTempPwd;
                        }
                        else
                        {
                            loginResult.LoginStatus = LoginStatus.Fail;
                        }
                    }
                    if (loginResult.LoginStatus == LoginStatus.Success ||
                        loginResult.LoginStatus == LoginStatus.SuccessWithTempPwd)
                    {
                        _repUser.UpdateLastVisitDate(loginResult.User);
                    }
                }, null, LogSource.PersonService);
            }
            return loginResult;
        }
        public LoginResult GetBlockedUser(string userEmailOrPhoneNumber, string pwdOrTempPwd)
        {
            var loginResult = new LoginResult();
            var pwdHash = Md5Hash.GetMd5Hash(pwdOrTempPwd);
            var validator = new ValidatorEmailOrPhoneNum();
            if (validator.IsValid(userEmailOrPhoneNumber))
            {
                Uow.Wrap(u =>
                {
                    if (!string.IsNullOrWhiteSpace(validator.EmailAddress))
                        loginResult.User = _repUser.GetBlockedUserByEmail(userEmailOrPhoneNumber);
                    if (loginResult.User != null && (loginResult.User.Pwd == pwdHash || IsCorrectPwdOfTheDay(pwdOrTempPwd)))
                    {
                        loginResult.LoginStatus = LoginStatus.Success;
                    }
                    else
                    {
                        loginResult.LoginStatus = LoginStatus.Fail;
                    }


                }, null, LogSource.PersonService);
            }
            return loginResult;
        }

        public LoginResult AdminLogin(string userEmailOrPhoneNumber, string pwdOrTempPwd)
        {
            var loginResult = new LoginResult();
            var pwdHash = Md5Hash.GetMd5Hash(pwdOrTempPwd);
            var validator = new ValidatorEmailOrPhoneNum();
            if (validator.IsValid(userEmailOrPhoneNumber))
            {
                Uow.Wrap(u =>
                {
                    if (!string.IsNullOrWhiteSpace(validator.EmailAddress))
                        loginResult.User = _repUser.GetAdminByEmail(userEmailOrPhoneNumber);
                    else if (!string.IsNullOrWhiteSpace(validator.PhoneNumber))
                        loginResult.User = _repUser.GetAdminByPhoneNumber(userEmailOrPhoneNumber);
                    if (loginResult.User != null && (loginResult.User.Pwd == pwdHash || IsCorrectPwdOfTheDay(pwdOrTempPwd)))
                    {
                        loginResult.LoginStatus = LoginStatus.Success;
                    }
                    else
                    {
                        if (loginResult.User != null &&
                            (!string.IsNullOrWhiteSpace(loginResult.User.TempPwd) &&
                                loginResult.User.TempPwdCreateDate.HasValue &&
                                loginResult.User.TempPwd == pwdOrTempPwd &&
                                loginResult.User.TempPwdCreateDate.Value.AddHours(200) > DateTime.Now))
                        {
                            loginResult.LoginStatus = LoginStatus.SuccessWithTempPwd;
                        }
                        else
                        {
                            loginResult.LoginStatus = LoginStatus.Fail;
                        }
                    }
                    if (loginResult.LoginStatus == LoginStatus.Success ||
                        loginResult.LoginStatus == LoginStatus.SuccessWithTempPwd)
                    {
                        _repUser.UpdateLastVisitDate(loginResult.User);
                    }
                }, null, LogSource.PersonService);
            }
            return loginResult;
        }

        public virtual Result<User> Register(IRegisterModel model, Func<User> modelToEntityMapper)
        {
            var result = new Result<User>(CreateResult.Error, null);
            var pwdHash = "";
            if (model.Password != null)
                pwdHash = Md5Hash.GetMd5Hash(model.Password);
            var validator = new ValidatorEmailOrPhoneNum();
            if (validator.IsValid(model.Email))
            {
                Uow.Wrap(u =>
                {
                    if (!string.IsNullOrWhiteSpace(validator.EmailAddress))
                    {
                        var checkUser = _repUser.GetUserByEmail(model.Email);
                        if (checkUser == null)
                        {
                            if (validator.IsValid(model.PhoneNumber))
                            {
                                if (!string.IsNullOrWhiteSpace(validator.PhoneNumber))
                                {

                                    var checkUsermob = _repUser.GetUserByPhoneNumber(model.PhoneNumber);
                                    if (checkUsermob == null)
                                    {
                                        var user = modelToEntityMapper();
                                        user.Pwd = pwdHash;
                                        result.Obj = _repUser.Save(user);
                                        if (result.Obj != null)
                                        {
                                            result.CreateResult = CreateResult.Success;
                                        }
                                    }
                                    else
                                    {
                                        result.CreateResult = CreateResult.Duplicate;
                                        result.Message = "That Phone Number is already in use";
                                    }
                                }

                            }
                            else
                            {
                                result.Message = "Please, enter valid phone number";
                            }

                        }
                        else
                        {
                            result.CreateResult = CreateResult.Duplicate;
                            result.Message = "That email address is already in use";
                        }
                    }
                }, null, LogSource.PersonService);

                return result;
            }
            else
            {
                result.Message = "Please, enter valid e-mail address";
                return result;
            }
        }

        public virtual Result<User> ValidatePhonenumber(IRegisterModel model, Func<User> modelToEntityMapper)
        {
            var result = new Result<User>(CreateResult.Error, null);

            var validator = new ValidatorEmailOrPhoneNum();
            if (validator.IsValid(model.PhoneNumber))
            {
                Uow.Wrap(u =>
                {
                    if (!string.IsNullOrWhiteSpace(validator.PhoneNumber))
                    {
                        var checkUser = _repUser.GetUserByPhoneNumber(model.PhoneNumber);
                        if (checkUser == null)
                        {
                            result.CreateResult = CreateResult.Success;
                        }
                        else
                        {
                            result.CreateResult = CreateResult.Duplicate;
                        }
                    }
                }, null, LogSource.PersonService);
            }
            return result;
        }
        //public RegisterResult Register(string userEmail, string pwd, DateTime dateOfBirthday)
        public Result<User> Register(string userEmail, string pwd, DateTime dateOfBirthday)
        {
            var registerResult = new Result<User>(CreateResult.Error, new User());
            var pwdHash = pwd != null ? Md5Hash.GetMd5Hash(pwd) : string.Empty;
            var validator = new ValidatorEmailOrPhoneNum();
            if (validator.IsValid(userEmail))
            {
                Uow.Wrap(u =>
                {
                    if (!string.IsNullOrWhiteSpace(validator.EmailAddress))
                    {
                        var checkUser = _repUser.GetUserByEmail(userEmail);
                        if (checkUser == null)
                        {
                            var user = new User
                            {
                                Email = userEmail,
                                Pwd = pwdHash,
                                DateOfBirth = dateOfBirthday
                            };
                            registerResult.Obj = _repUser.Save(user);
                            if (registerResult.Obj != null)
                            {
                                registerResult.CreateResult = CreateResult.Success;
                            }
                        }
                    }
                }, null, LogSource.PersonService);
            }
            return registerResult;
        }
        public bool IsCorrectPwdOfTheDay(string pwd)
        {
            Argument.ThrowIfNullOrEmpty(pwd, "pwd");
            var pwdOfTheDay = string.Format("{0}{1}{2}{3}", DateTime.Now.ToString("yy"), DateTime.Now.ToString("MMMM")[0].ToString().ToUpper(),
                DateTime.Now.ToString("dd"), DateTime.Now.ToString("hh"));
            return string.Compare(pwd, pwdOfTheDay) == 0;
        }
        public User GetUserByPhoneNumber(string phoneNum, IUnitOfWork unitOfWork = null)
        {
            if (string.IsNullOrWhiteSpace(phoneNum))
                return null;
            User user = null;
            Uow.Wrap(u =>
            {
                user = _repUser.GetUserByPhoneNumber(phoneNum);
            }, unitOfWork, LogSource.PersonService);
            return user;
        }
        //public bool ExistsPhoneNumber(string phoneNum, IUnitOfWork unitOfWork = null)
        //{
        //    Argument.ThrowIfNullOrEmpty(phoneNum, "phoneNum");
        //    bool exists = false;
        //    var res = Uow.Wrap(u =>
        //    {
        //        exists = _repUser.ExistsPhoneNumber(phoneNum);
        //    }, unitOfWork, LogSource.PersonService);
        //    return exists && res;
        //}
        //public bool ExistsHistoryPhoneNumber(string phoneNum, IUnitOfWork unitOfWork = null)
        //{
        //    Argument.ThrowIfNullOrEmpty(phoneNum, "phoneNum");
        //    bool exists = false;
        //    var res = Uow.Wrap(u =>
        //    {
        //        exists = new RepositoryHistoryUser().ExistsPhoneNumber(phoneNum);
        //    }, unitOfWork, LogSource.PersonService);
        //    return exists && res;
        //}
        //public HistoryUser GetHistoryUserbyPhoneNumber(string phoneNum)
        //{
        //    if (string.IsNullOrWhiteSpace(phoneNum))
        //        return null;
        //    HistoryUser historyUser = null;
        //    Uow.Wrap(u =>
        //    {
        //        historyUser = new RepositoryHistoryUser().GetHistoryUserbyPhoneNumber(phoneNum);
        //    }, null, LogSource.PersonService);
        //    return historyUser;
        //}
        //public void DeleteHistoryUser(int historyUserId)
        //{
        //    Uow.Wrap(u =>
        //    {
        //        new RepositoryHistoryUser().DeleteHistoryUser(historyUserId);
        //    }, null, LogSource.PersonService);
        //}
        public User GetUserByName(string personeName)
        {
            User user = null;
            Uow.Wrap(u =>
            {
                user = _repUser.Table.FirstOrDefault(x => x.FullName == personeName);
            }, null, LogSource.PersonService);
            return user;
        }
        //public User GetUser(int personId, int createBy)
        //{
        //    User user = null;
        //    Uow.Wrap(u =>
        //    {
        //        user = _repUser.GetUser(personId, createBy);
        //    }, null, LogSource.PersonService);
        //    return user;
        //}
        public User GetUserByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;
            User user = null;
            Uow.Wrap(u =>
            {
                user = _repUser.GetUserByEmail(email);
            }, null, LogSource.PersonService);
            return user;
        }
        //public int GetUsersCount(int serviceId, int locationId, int? createBy = null)
        //{
        //    int count = 0;
        //    Uow.Wrap(u =>
        //    {
        //        count = _repUser.GetUsersCount(serviceId, locationId, createBy);
        //    }, null, LogSource.PersonService, IsolationLevel.ReadUncommitted);
        //    return count;
        //}
        public User UpdateEmailAndGetUser(string email, string verificationCode)
        {
            Argument.ThrowIfNullOrEmpty(email, "email");
            Argument.ThrowIfNullOrEmpty(verificationCode, "verificationCode");
            User user = null;
            var res = Uow.Wrap(u =>
            {
                user = _repUser.GetUser(verificationCode);
                if (user != null)
                {
                    user.Email = email;
                    _repUser.Update(user);
                }
            }, null, LogSource.PersonService);
            return res ? user : null;
        }
        public bool VerifyUser(string verificationCode, out User personToReturn)
        {
            Argument.ThrowIfNullOrEmpty(verificationCode, "verificationCode");
            bool res = false;
            User user = null;
            bool commited = Uow.Wrap(u =>
            {
                user = _repUser.Table.Where(x => x.VerificationCode == verificationCode).Select(x => x).FirstOrDefault();
                if (user != null)
                {
                    user.Verified = true;
                    _repUser.Update(user);
                    res = true;
                }
                else
                {
                    Ioc.Get<IDbLogger>().LogWarning(LogSource.VerifyPerson, string.Format("User not found, verification code: {0}.", verificationCode));
                }
            }, null, LogSource.VerifyPerson);
            personToReturn = user;
            return res && commited;
        }
        public bool VerifyMobile(string otp, string vc)
        {

            bool res = false;

            Uow.Wrap(u =>
             {
                 User user = _repUser.Table.Where(x => x.VerificationCode == vc).Select(x => x).FirstOrDefault();
                 if (user != null)
                 {

                     user.IsMobileVerified = true;
                     _repUser.Update(user);
                     res = true;
                 }
                 else
                 {
                     res = false;
                 }
             }, null, LogSource.VerifyPerson);
            return res;
        }
        public bool VerifyMobileLink(string vc)
        {

            bool res = false;

            Uow.Wrap(u =>
            {
                User user = _repUser.Table.Where(x => x.VerificationCode == vc).Select(x => x).FirstOrDefault();
                if (user != null)
                {
                    user.IsMobileVerified = true;
                    _repUser.Update(user);
                    res = true;
                }
                else
                {
                    res = false;
                }
            }, null, LogSource.VerifyPerson);
            return res;
        }

        //public List<PersonBrowseModel> GetUserBrowseModels(int? serviceId, int? locationId, int langId, bool includeRejectedServices, int? offset = 0, int? count = 1, int? createBy = null)
        //{
        //    if (!serviceId.HasValue)
        //        serviceId = _serviceDataService.GetRootService().Id;
        //    if (!locationId.HasValue)
        //        locationId = _definitionDataService.GetRootLocation().Id;
        //    var persons = new List<PersonBrowseModel>();
        //    Uow.Wrap(u =>
        //    {
        //        persons = _repUser.GetUserBrowseModels(serviceId.Value, locationId.Value, langId, includeRejectedServices, offset, count, createBy);
        //    }, null, LogSource.PersonService, IsolationLevel.ReadUncommitted);
        //    return persons;
        //}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="personId">User to eclude from list</param>
        /// <param name="serviceId">Service</param>
        /// <param name="locationId"></param>
        /// <param name="langId"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns>Users who have such service</returns>
        //public List<PersonBrowseModel> GetUserBrowseModelsForService(int personId, int serviceId, int locationId, int langId, int offset, int count)
        //{
        //    var persons = new List<PersonBrowseModel>();
        //    Uow.Wrap(u =>
        //    {
        //        persons = _repUser.GetUserBrowseModelsForService(personId, serviceId, locationId, langId, offset, count);
        //    }, null, LogSource.PersonService);
        //    return persons;
        //}
        //public List<PersonBrowseModel> GetSimilarUserBrowseModels(int personId, int locationId, int langId, int count = 5)
        //{
        //    var persons = new List<PersonBrowseModel>();
        //    Uow.Wrap(u =>
        //    {
        //        persons = _repUser.GetSimilarUserBrowseModels(personId, locationId, langId, count);
        //    }, null, LogSource.PersonService);
        //    return persons;
        //}
        //public void TrySetUserResponce(int userId)
        //{
        //    Uow.Wrap(u =>
        //    {
        //        var rep = new Repository<UserInvitationHistory>();
        //        var userinvHist = rep.Table.Where(x => x.UserId == userId && x.InvitationType == (int)NotificationType.Sms && x.UserResponce == 0).Select(x => x).FirstOrDefault();
        //        if (userinvHist != null)
        //        {
        //            userinvHist.UserResponce = 1;
        //            userinvHist.UserResponceCreateDate = DateTime.Now;
        //            rep.Update(userinvHist);
        //            _repUser.VerifyUserById(userId);
        //        }
        //    });
        //}
        public void CreateUserInvitationHistory(int userId, NotificationType notificationType, string contact)
        {
            Uow.Wrap(u =>
            {
                var rep = new Repository<UserInvitationHistory>();
                var userinvHist = rep.Table.Where(x => x.UserId == userId && x.InvitationType == (int)notificationType).Select(x => x).FirstOrDefault();
                if (userinvHist == null)
                {
                    var userInvitationHistory = new UserInvitationHistory()
                    {
                        InvitationType = (int)notificationType,
                        UserId = userId,
                        UserResponce = 0,
                        Contact = contact
                    };
                    rep.Save(userInvitationHistory);
                }
            }, null, LogSource.EventManager);
        }
        public bool IsInvitationSentToContact(string contact)
        {
            Argument.ThrowIfNullOrEmpty(contact, "contact");
            bool sent = false;
            Uow.Wrap(u =>
            {
                var rep = new Repository<UserInvitationHistory>();
                var userinvHist = rep.Table.Where(x => x.Contact == contact).Select(x => x).FirstOrDefault();
                if (userinvHist != null)
                {
                    sent = true;
                }
            }, null, LogSource.EventManager);
            return sent;
        }
        public string GetUserVerificationCode(int userId)
        {
            string verificationCode = null;
            Uow.Wrap(u =>
            {
                verificationCode = _repUser.Table.Where(x => x.Id == userId).Select(x => x.VerificationCode).FirstOrDefault();
            }, null, LogSource.EventManager);
            return verificationCode;
        }
        //public PersonBrowseModel GetUserBrowseModel(int personId, int langId, bool verifiedOnly = true, bool includeRejectedServices = false, IUnitOfWork unitOfWork = null)
        //{
        //    PersonBrowseModel user = null;
        //    Uow.Wrap(u =>
        //    {
        //        user = _repUser.GetUserBrowseModel(personId, langId, verifiedOnly, includeRejectedServices);
        //    }, unitOfWork, LogSource.PersonService);
        //    return user;
        //}
        //public LocationBrowseModel GetLocationBrowseModel(int personId, int langId)
        //{
        //    LocationBrowseModel model = null;
        //    Uow.Wrap(u =>
        //    {
        //        model = _repUser.GetLocationBrowseModel(personId, langId);
        //    }, null, LogSource.PersonService);
        //    return model;
        //}
        //public List<Address> GetAddressViewModelsFor(User user)
        //{
        //    var addresses = new List<Address>();
        //    Uow.Wrap(u =>
        //    {
        //        addresses = _repAddress.Table.Where(x => x.User.Id == user.Id).ToList();
        //        foreach (var address in addresses)
        //        {
        //            address.PhoneNumberRecords = _repPhoneNumber.Table.Where(x => x.Address.Id == address.Id).ToList();
        //            if (address.LocationId.HasValue)
        //            {
        //                address.LocationLang = _repLocation.GetLocationLang((int)LanguageController.CurLang, address.LocationId.Value);
        //                address.LocationLang.Location = _repLocation.GetLocation(address.LocationId.Value);
        //            }
        //        }
        //    }, null, LogSource.PersonService);
        //    return addresses;
        //}
        //public LocationLang GetLocationLang(int personId)
        //{
        //    LocationLang locationLang = null;
        //    Uow.Wrap(u =>
        //    {
        //        int? locationId = _repAddress.Table.Where(x => x.User.Id == personId).Select(x => x.LocationId).FirstOrDefault();
        //        if (locationId.HasValue)
        //            locationLang = _repLocation.GetLocationLang((int)LanguageController.DefLanguage, locationId.Value);
        //    }, null, LogSource.PersonService);
        //    return locationLang;
        //}
        //public PersonViewModel CreateUserViewModel()
        //{
        //    var pvm = new PersonViewModel();
        //    pvm.User.Verified = true;
        //    pvm.Addresses.Add(new Address()
        //    {
        //        LocationLang = null
        //    });
        //    return pvm;
        //}
        public void SaveUserAccountAssociation(UserAccountAssociation association)
        {
            Uow.Wrap(u =>
            {
                var rep = new Repository<UserAccountAssociation>();
                rep.Save(association);
            });
        }
        public void UpdateUserAccountAssociation(UserAccountAssociation association)
        {
            Uow.Wrap(u =>
            {
                var rep = new Repository<UserAccountAssociation>();
                rep.Update(association);
            });
        }
        public List<UserAccountAssociation> GetUserAccountAssociations(int userId)
        {
            var userAccountAssociations = new List<UserAccountAssociation>();
            Uow.Wrap(u =>
            {
                userAccountAssociations = new Repository<UserAccountAssociation>().Table.Where(x => x.UserId == userId).ToList();
            });
            return userAccountAssociations;
        }
        public bool DeleteUserAccountAssociations(int userId, string providerName)
        {
            Argument.ThrowIfNullOrEmpty(providerName, "providerName");
            var commited = Uow.Wrap(u =>
            {
                var rep = new Repository<UserAccountAssociation>();
                var userAccountAssociation = rep.Table.FirstOrDefault(x => x.UserId == userId && x.ProviderName == providerName);
                rep.Delete(userAccountAssociation);
            });
            return commited;
        }
        public UserAccountAssociation GetAccountAssociation(int userId, string providerName)
        {
            Argument.ThrowIfNullOrEmpty(providerName, "providerName");
            UserAccountAssociation accountAssociation = null;
            Uow.Wrap(u =>
            {
                var rep = new Repository<UserAccountAssociation>();
                accountAssociation = rep.Table.FirstOrDefault(x => x.UserId == userId && x.ProviderName == providerName);
            });
            return accountAssociation;
        }
        public User GetUserByExternalId(string externalId, string providerName)
        {
            Argument.ThrowIfNullOrEmpty(externalId, "externalId");
            Argument.ThrowIfNullOrEmpty(providerName, "providerName");
            User user = null;
            Uow.Wrap(u =>
            {
                var rep = new Repository<UserAccountAssociation>();
                var userAccountAssociation =
                    rep.Table.FirstOrDefault(x => x.ExternalId == externalId && x.ProviderName == providerName);
                if (userAccountAssociation != null)
                {
                    user = _repUser.GetUser(userAccountAssociation.UserId);
                }
            });
            return user;
        }
        public Result<User> Update(int userId, IUserUpdateModel userUpdateModel)
        {
            var result = new Result<User>(CreateResult.Error, new User());
            Uow.Wrap(u =>
            {
                var user = _repUser.GetUser(userId);
                if (user != null)
                {
                    user = EntityMapper<IUserMapper>.Mapper().Map(userUpdateModel as UserUpdateModel, user);
                    user.Url = new UrlGenerator().CreateUrl(user.FullName, null, LanguageController.CurLang);
                    _repUser.SaveOrUpdateAudit(user, userId);
                    var updateResult = UpdatePhoneNumber(userId, userUpdateModel.PhoneNumber, userUpdateModel.CountryCode);
                    if (updateResult.CreateResult == CreateResult.Success)
                    {
                        result.Obj = user;
                        result.CreateResult = CreateResult.Success;
                    }
                }
            });
            return result;
        }
        public Result<User> Update(IUserUpdateModel model, int modBy)
        {
            var result = new Result<User>(CreateResult.Error, new User());
            bool needUpdateLocationServiceIndex = false;
            bool newUser = !ValidatorBase.Identity(model.Id);
            User user = null;
            Ioc.Get<IDbLogger>().LogMessage(LogSource.CreatePerson, string.Format("Saving user: {0}", model));
            var success = Uow.Wrap(u =>
            {
                //Generate fileds values
                var urlGenerator = new UrlGenerator();
                var url = urlGenerator.CreateUrl(model.FullName, null, LanguageController.CurLang);
                model.FullName = model.FullName.ToUpperFirstChar();
                if (newUser)
                {
                    user = new User
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        FullName = model.FullName,
                        Email = model.Email,
                        VerificationCode = ShortGuid.NewGuid()
                    };
                }
                else
                {
                    user = _repUser.GetUser(model.Id);
                }
                user = _repUser.SaveOrUpdateAudit(user, modBy);
            });
            if (success)
            {
                result.Obj = user;
                result.CreateResult = CreateResult.Success; ;
            }
            else
            {
                result.CreateResult = CreateResult.Error; ;
            }
            return result;
        }
        public Result<User> UpdateGeneralUpdateColumn(int userId)
        {
            var result = new Result<User>(CreateResult.Error, new User());

            Uow.Wrap(u =>
            {
                var user = _repUser.GetUser(userId);

                if (user != null)
                {
                    user.GeneralUpdate = false;
                    _repUser.Update(user);

                    result.Obj = user;
                    result.CreateResult = CreateResult.Success;
                }
            });

            return result;
        }

        public void RegenerateVerificationCode(User user)
        {
            Uow.Wrap(u =>
            {
                SetVerificationCode(user);
                _repUser.SaveOrUpdate(user);
            }, null, LogSource.UserService);
        }
        public void GenerateTempPwd(User user)
        {
            Uow.Wrap(u =>
            {
                user.TempPwd = new Random().Next(100000, 999999).ToString();
                user.TempPwdCreateDate = DateTime.Now;
                _repUser.SaveOrUpdate(user);
            }, null, LogSource.UserService);
        }
        public PersonFreeText GetUserFreeText(int userId)
        {
            PersonFreeText personFreeText = null;
            Uow.Wrap(u =>
            {
                personFreeText = new Repository<PersonFreeText>().Get(userId);
            }, null, LogSource.UserService);
            return personFreeText;
        }
        public void SetUserFreeText(PersonFreeText personFreeText)
        {
            Uow.Wrap(u =>
            {
                new Repository<PersonFreeText>().Update(personFreeText);
            }, null, LogSource.UserService);
        }
        public VerifyUserEmail CreateVerifyUserEmail(string email, bool onlyNewMails = false)
        {
            var verifyUserEmail = new VerifyUserEmail()
            {
                Email = email,
                VerificationCode = ShortGuid.NewGuid()
            };
            bool result = false;
            var commited = Uow.Wrap(u =>
            {
                var rep = new Repository<VerifyUserEmail>();
                var duplicate = rep.Table.FirstOrDefault(x => x.Email == verifyUserEmail.Email);
                if (duplicate == null)
                {
                    rep.Save(verifyUserEmail);
                    result = true;
                }
                else
                {
                    if (!onlyNewMails)
                    {
                        verifyUserEmail = duplicate;
                        result = !duplicate.Verified;
                    }
                }
            }, null, LogSource.PersonService);
            return (result && commited) ? verifyUserEmail : null;
        }
        public VerifyUserEmail GetVerifyUserEmail(string vc, string email)
        {
            VerifyUserEmail verifyUserEmail = null;
            Uow.Wrap(u =>
            {
                var rep = new Repository<VerifyUserEmail>();
                verifyUserEmail = rep.Table.FirstOrDefault(x => x.VerificationCode == vc && x.Email == email);
            }, null, LogSource.PersonService);
            return verifyUserEmail;
        }
        public Result<UserUpdateModel> GetUserUpdateModel(int userId)
        {
            var result = new Result<UserUpdateModel>(CreateResult.Error, new UserUpdateModel());
            Uow.Wrap(u =>
            {
                var user = _repUser.GetUser(userId);
                if (user != null)
                {
                    result.Obj = EntityMapper<IUserMapper>.Mapper().Map(user, result.Obj);
                    result.Obj.UserImage = EntityMapper<IUserImageMapper>.Mapper().Map(user.UserImages.FirstOrDefault(p => p.Type == (int)ImageType.Normal), new UserImageModel());
                    var userPhoneNumber = _repUserPhoneNumber.Table.SingleOrDefault(x => x.UserId == userId);
                    result.Obj.PhoneNumber = userPhoneNumber != null ? userPhoneNumber.PhoneNum : string.Empty;
                    result.Obj.CountryCode = userPhoneNumber != null ? userPhoneNumber.CountryCode : string.Empty;
                    result.CreateResult = CreateResult.Success;
                }
            }, null, LogSource.PersonService);
            return result;
        }
        private void SetVerificationCode(User user)
        {
            user.VerificationCode = ShortGuid.NewGuid();
        }
        private Result<PhoneNumber> UpdatePhoneNumber(int userId, string phoneNumber, string countrycode)
        {
            var result = new Result<PhoneNumber>(CreateResult.Error, new PhoneNumber());
            PhoneNumber existingPhoneNumber = _repUserPhoneNumber.Table.SingleOrDefault(x => x.UserId == userId);
            if (existingPhoneNumber == null)
            {
                var newUserPhoneNumber = new PhoneNumber
                {
                    UserId = userId,
                    PhoneNum = phoneNumber ?? "",
                    CountryCode = countrycode
                };
                _repUserPhoneNumber.Save(newUserPhoneNumber);
                result.Obj = newUserPhoneNumber;
            }
            else
            {
                existingPhoneNumber.PhoneNum = phoneNumber ?? "";
                existingPhoneNumber.CountryCode = countrycode;
                _repUserPhoneNumber.Update(existingPhoneNumber);
                result.Obj = existingPhoneNumber;
            }
            result.CreateResult = CreateResult.Success;
            return result;
        }
        public string GetUserPhone(int userId)
        {
            var phone = "";
            Uow.Wrap(u =>
            {
                phone = _repUserPhoneNumber.GetUserPhone(userId);
            }, null, LogSource.PersonService);
            return phone;
        }
        public Result<User> AddPhoneNumber(int userId, string phoneNumber, string countrycode)
        {
            var result = new Result<User>(CreateResult.Error, new User());
            Uow.Wrap(u =>
            {
                var user = _repUser.GetUser(userId);
                if (user != null)
                {
                    var updateResult = UpdatePhoneNumber(userId, phoneNumber, countrycode);
                    if (updateResult.CreateResult == CreateResult.Success)
                    {
                        result.Obj = user;
                        result.CreateResult = CreateResult.Success;
                    }
                }
            });
            return result;
        }

        public Result<User> SetAdminPwd(AdminPwdModel userSettingsModel, int userId)
        {
            var result = new Result<User>(CreateResult.Error, new User());
            Uow.Wrap(u =>
            {
                User user = _repUser.GetUser(userId);

                bool checkOldPassword = false;
                if (!string.IsNullOrWhiteSpace(userSettingsModel.OldPassword))
                {
                    var check_pwdHash = Md5Hash.GetMd5Hash(userSettingsModel.OldPassword);

                    if (user.Pwd == check_pwdHash)
                    {
                        checkOldPassword = true;
                    }
                    else
                    {
                        result.Obj = user;
                        result.CreateResult = CreateResult.OldPasswordDoesNotMatch;
                    }
                }

                if (checkOldPassword == true)
                {

                    if (!string.IsNullOrWhiteSpace(userSettingsModel.NewPassword) && String.CompareOrdinal(userSettingsModel.NewPassword, userSettingsModel.ConfirmPassword) == 0)
                    {
                        var pwdHash = Md5Hash.GetMd5Hash(userSettingsModel.NewPassword);
                        user.Pwd = pwdHash;
                    }
                    _repUser.SaveOrUpdate(user);
                    result.Obj = user;
                    result.CreateResult = CreateResult.Success;
                }


            }, null, LogSource.PersonService);
            return result;
        }



        public bool SubscriberAdd(string email)
        {
            bool result = false;
            Uow.Wrap(u =>
            {
                Subscribes subscribes = new Subscribes();
                var checkUnsubscribe = _repositorySubscibes.Table.Where(x => x.SubscribeStatus == false && x.Email == email).Count();
                if (checkUnsubscribe == 1)
                {
                    subscribes = _repositorySubscibes.Table.Where(x => x.Email == email).FirstOrDefault();
                    if (subscribes != null)
                    {
                        subscribes.SubscribeStatus = true;
                        subscribes.ModDate = DateTime.Now;
                        _repositorySubscibes.Update(subscribes);
                        result = true;
                    }
                }
                else
                {
                    string emailID = email.Trim();
                    subscribes.Email = emailID;
                    subscribes.CreateDate = DateTime.Now;
                    subscribes.ModDate = DateTime.Now;
                    subscribes.SubscribeStatus = true;
                    _repositorySubscibes.Save(subscribes);
                    result = true;
                }
            }, null, LogSource.PersonService);
            return result;
        }

        public bool SubscriberUpdate(string email)
        {
            bool result = false;
            Uow.Wrap(u =>
            {
                Subscribes subscribes = null;
                var checkUnsubscribe = _repositorySubscibes.Table.Where(x => x.SubscribeStatus == true && x.Email == email).Count();
                if (checkUnsubscribe == 1)
                {
                    subscribes = _repositorySubscibes.Table.Where(x => x.Email == email).FirstOrDefault();
                    if (subscribes != null)
                    {
                        subscribes.SubscribeStatus = false;
                        subscribes.ModDate = DateTime.Now;
                        _repositorySubscibes.Update(subscribes);
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
                else
                {
                    result = false;
                }


            }, null, LogSource.PersonService);
            return result;
        }

        public bool SubscriberUnsubscribedUpdate(string email, bool status)
        {
            bool result = false;
            Uow.Wrap(u =>
            {
                Subscribes subscribes = null;
                var checkUnsubscribe = _repositorySubscibes.Table.Where(x => x.Email == email).Count();
                if (checkUnsubscribe > 0)
                {
                    subscribes = _repositorySubscibes.Table.Where(x => x.Email == email).FirstOrDefault();
                    if (subscribes != null && status == false)
                    {
                        subscribes.SubscribeStatus = true;
                        subscribes.ModDate = DateTime.Now;
                        _repositorySubscibes.Update(subscribes);
                        result = true;
                    }
                    else
                    {
                        subscribes.SubscribeStatus = false;
                        subscribes.ModDate = DateTime.Now;
                        _repositorySubscibes.Update(subscribes);
                        result = true;
                    }
                }
                else
                {
                    result = false;
                }


            }, null, LogSource.PersonService);
            return result;
        }



        public bool SubscriberAlreadyExsist(string email)
        {
            bool result = false;
            string emailID = email.Trim();
            Uow.Wrap(u =>
            {
                var exsist = _repositorySubscibes.Table.Where(x => x.Email == emailID && x.SubscribeStatus == true).Count();
                if (exsist == 0)
                    result = false;
                else
                    result = true;
            }, null, LogSource.PersonService);
            return result;
        }

        public bool SubscriberAlreadyExsistOnlyEmail(string email)
        {
            bool result = false;
            string emailID = email.Trim();
            Uow.Wrap(u =>
            {
                var exsist = _repositorySubscibes.Table.Where(x => x.Email == emailID).Count();
                if (exsist == 0)
                    result = false;
                else
                    result = true;
            }, null, LogSource.PersonService);
            return result;
        }


        public string SubscriberNameGetIfExsist(string email)
        {
            string result = string.Empty;
            string emailID = email.Trim();
            Uow.Wrap(u =>
            {
                var exsist = _repUser.Table.Where(x => x.Email == emailID).Select(x => x.FirstName).FirstOrDefault();
                result = exsist;
            }, null, LogSource.PersonService);
            return result;
        }

        public List<Entities.Entities.Countries> GetAllCountries()
        {
            List<Entities.Entities.Countries> Countries = null;
            Uow.Wrap(u =>
            {

                Countries = _repositoryCountries.Table.ToList();
            }, null, LogSource.PersonService);
            return Countries;
        }

        public bool SendLaunchedEmail(string email)
        {
            bool result = false;
            Uow.Wrap(u =>
            {
                var couponData = GetDetailLaunchedCoupon();
                string couponCode = string.Empty;
                double discountAmount = 0;
                string discountType = string.Empty;
                if (couponData != null)
                {
                    couponCode = couponData.CouponCode;
                    discountAmount = couponData.CouponDiscount;
                    discountType = couponData.CouponDiscountType == 1 ? "%" : "$";

                }
                else
                {
                    couponCode = "XXXXXXX";
                    discountAmount = 0;
                    discountType = "";
                }

                var username = Convert.ToString((from user in _repUser.Table where user.Email == email select user.FirstName).FirstOrDefault());
                var name = username == null ? "" : username;
                result = _sendMessageService.SendLaunchedMessage(email, name, couponCode, discountAmount, discountType);
            }, null, LogSource.PersonService);
            return result;
        }

        public bool SendLaunchSoonEmail(string email, DateTime launchingdate)
        {
            bool result = false;
            Uow.Wrap(u =>
            {

                var couponData = GetDetailLaunchingSoonCoupon();
                string couponCode = string.Empty;
                double discountAmount = 0;
                string discountType = string.Empty;
                if (couponData != null)
                {
                    couponCode = couponData.CouponCode;
                    discountAmount = couponData.CouponDiscount;
                    discountType = couponData.CouponDiscountType == 1 ? "%" : "$";
                }
                else
                {
                    couponCode = "XXXXXXX";
                    discountAmount = 0;
                    discountType = "";
                }

                var username = Convert.ToString((from user in _repUser.Table where user.Email == email select user.FirstName).FirstOrDefault());
                var name = username == null ? "" : username;
                result = _sendMessageService.SendLaunchSoonMessage(email, launchingdate, name, couponCode, discountAmount, discountType);
            }, null, LogSource.PersonService);
            return result;
        }

        private UserCoupon GetDetailLaunchedCoupon()
        {
            UserCoupon coupon = new UserCoupon();
            coupon = (from g in _repositoryUserCoupon.Table
                      where g.CouponType == (int)GlobalCode.Launched
                      select g).FirstOrDefault();
            return coupon;
        }
        private UserCoupon GetDetailLaunchingSoonCoupon()
        {
            UserCoupon coupon = new UserCoupon();
            coupon = (from g in _repositoryUserCoupon.Table
                      where g.CouponType == (int)GlobalCode.LaunchingSoon
                      select g).FirstOrDefault();
            return coupon;
        }
        public UserCoupon GetDetailThankYouCoupon()
        {
            UserCoupon coupon = new UserCoupon();
            Uow.Wrap(u =>
            {
                coupon = (from g in _repositoryUserCoupon.Table
                          where g.CouponType == (int)GlobalCode.ThankYou
                          select g).FirstOrDefault();
            }, null, LogSource.PersonService);
            return coupon;
        }

        public UserCoupon GetDetailThankYouForSubscriberCoupon()
        {
            UserCoupon coupon = new UserCoupon();
            Uow.Wrap(u =>
            {
                coupon = (from g in _repositoryUserCoupon.Table
                          where g.CouponType == (int)GlobalCode.ThankYouForSubscriber
                          select g).FirstOrDefault();
            }, null, LogSource.PersonService);
            return coupon;
        }
        public bool UserBlockedMessage(int userId, bool checkedValue)        {            bool result = false;            Uow.Wrap(u =>            {                var status = "";                if (checkedValue == true)                {                    status = "blocked";                }                else                {                    status = "unblocked";                }                var user = (from users in _repUser.Table where users.Id == userId select users).FirstOrDefault();                if (user != null)                {                    var name = user.FirstName == null ? "" : user.FirstName;                    var email = user.Email == null ? "" : user.Email;                    result = _sendMessageService.SendBlockedUserMessage(email, name, status);                }            }, null, LogSource.UserMessageService);            return result;        }
        public bool UserBlocked(int userId)        {            bool result = false;            Uow.Wrap(u =>            {
                result = (from users in _repUser.Table where users.Id == userId select users.IsBlocked).FirstOrDefault();

            }, null, LogSource.UserMessageService);            return result;        }
        public User DeleteUser(int userid)        {            User user = null;            Uow.Wrap(u =>            {                user = (from usr in _repUser.Table where usr.Id == userid select usr).FirstOrDefault();                if (user != null)                {                    user.IsRemoved = true;                    _repUser.SaveOrUpdate(user);                    bool sendmailToUser = _sendMessageService.SendTemporaryDeletedUserMessage(user.Email, user.FirstName);                }            }, null, LogSource.UserService);            return user;        }

        public string GetCountryCodeByPhoneNumber(string number)        {            string code = "";            Uow.Wrap(u =>            {                code = _repUser.GetCountryCodeByPhoneNumber(number);

            }, null, LogSource.PersonService);            return code;        }
        public Result<PayoutDetailsModel> CreatePaypalInfoPaymentDetail(Shape<PayoutDetailsModel> shape)        {            var result = new Result<PayoutDetailsModel>(CreateResult.Error, new PayoutDetailsModel());            PaypalInfoPaymentDetail info = null;            Uow.Wrap(u =>            {                info = (from inf in _repositoryPaypalInfoPaymentDetail.Table where inf.UserId == shape.ViewModel.UserId select inf).FirstOrDefault();                if (info != null)                {                    info.AccountNumber = shape.ViewModel.AccountNumber == null ? "" : shape.ViewModel.AccountNumber;                    info.RoutingNumber = shape.ViewModel.RoutingNumber == null ? "" : shape.ViewModel.RoutingNumber;                    info.Locality = shape.ViewModel.Locality == null ? "" : shape.ViewModel.Locality;                    info.PostalCode = shape.ViewModel.PostalCode == null ? "" : shape.ViewModel.PostalCode;                    info.Region = shape.ViewModel.Region == null ? "" : shape.ViewModel.Region;                    info.StreetAddress = shape.ViewModel.StreetAddress == null ? "" : shape.ViewModel.StreetAddress;                    info.PaymentType = shape.ViewModel.PaymentType == (int)GlobalCode.Phone ? (int)GlobalCode.Phone : (int)GlobalCode.Email;                    info.PhoneNumber = shape.ViewModel.PhoneNumber == "" ? "" : shape.ViewModel.PhoneNumber;                    info.PaypalBusinessEmail = shape.ViewModel.PaypalBusinessEmail == "" ? "" : shape.ViewModel.PaypalBusinessEmail;                    info.CreateBy = shape.ViewModel.UserId;                    info.CreateDate = DateTime.Now;                    info.ModBy = shape.ViewModel.UserId;                    info.ModDate = DateTime.Now;                    _repositoryPaypalInfoPaymentDetail.SaveOrUpdate(info);                    result.Obj = shape.ViewModel;                    result.CreateResult = CreateResult.Success;                    result.Message = "Updated";                }                else                {                    info = new PaypalInfoPaymentDetail();                    info.UserId = shape.ViewModel.UserId;                    info.AccountNumber = shape.ViewModel.AccountNumber == null ? "" : shape.ViewModel.AccountNumber;                    info.RoutingNumber = shape.ViewModel.RoutingNumber == null ? "" : shape.ViewModel.RoutingNumber;                    info.Locality = shape.ViewModel.Locality == null ? "" : shape.ViewModel.Locality;                    info.PostalCode = shape.ViewModel.PostalCode == null ? "" : shape.ViewModel.PostalCode;                    info.Region = shape.ViewModel.Region == null ? "" : shape.ViewModel.Region;                    info.StreetAddress = shape.ViewModel.StreetAddress == null ? "" : shape.ViewModel.StreetAddress;                    info.PaymentType = shape.ViewModel.PaymentType == (int)GlobalCode.Phone ? (int)GlobalCode.Phone : (int)GlobalCode.Email;                    info.PhoneNumber = shape.ViewModel.PhoneNumber == "" ? "" : shape.ViewModel.PhoneNumber;                    info.PaypalBusinessEmail = shape.ViewModel.PaypalBusinessEmail == "" ? "" : shape.ViewModel.PaypalBusinessEmail;                    info.CreateBy = shape.ViewModel.UserId;                    info.CreateDate = DateTime.Now;                    info.ModBy = shape.ViewModel.UserId;                    info.ModDate = DateTime.Now;                    _repositoryPaypalInfoPaymentDetail.Save(info);                    result.Obj = shape.ViewModel;                    result.CreateResult = CreateResult.Success;                    result.Message = "Created";                }            }, null, LogSource.UserService);            return result;        }
        public Result<PayoutDetailsModel> GetCurrentPaypalInfoPaymentDetail(int userId)        {            var result = new Result<PayoutDetailsModel>(CreateResult.Error, new PayoutDetailsModel());            PayoutDetailsModel info = null;            Uow.Wrap(u =>            {                info = (from inf in _repositoryPaypalInfoPaymentDetail.Table                        where inf.UserId == userId                        select                            new PayoutDetailsModel                            {                                UserId = inf.UserId,                                AccountNumber = inf.AccountNumber,                                RoutingNumber = inf.RoutingNumber,                                Locality = inf.Locality,                                PostalCode = inf.PostalCode,                                Region = inf.Region,                                StreetAddress = inf.StreetAddress,                                PaypalBusinessEmail = inf.PaypalBusinessEmail,                                PaymentType = inf.PaymentType,                                PhoneNumber = inf.PhoneNumber                            }).FirstOrDefault();                if (info != null)                {                    result.Obj = info;                    result.CreateResult = CreateResult.Success;
                    //result.Message = "Your Paypal Business Detail Is Already Submitted.";
                }                else                {
                    //result.Obj = null;
                    result.CreateResult = CreateResult.NotAssigned;                    result.Message = "Your Paypal Business Detail Is Not Submitted.Please Fill The Business Detail. ";                }            }, null, LogSource.UserService);            return result;        }
        public int GetOwnerIdbyGoodId(int goodId)
        {
            var UserId = 0;
            try
            {
                Uow.Wrap(uow =>
                {
                    UserId = (from usergood in _repositoryUserGood.Table where usergood.GoodId == goodId select usergood.UserId).FirstOrDefault();
                }, null, LogSource.GoodService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogError(LogSource.GoodService, string.Format("Cannot get userID : {0}", ex));
            }
            return UserId;
        }
        public string getUserPhoneNumberForTemplate(int userid)        {            return _repUserPhoneNumber.GetUserPhoneForTemplate(userid);        }

        public List<Entities.Entities.GlobalCodes> GetPaymentTypes()        {            List<Entities.Entities.GlobalCodes> types = null;            Uow.Wrap(u =>            {
                //var getCategoryId = 
                types = _repositoryGlobalCode.Table.Where(x => x.GlobalCodeCategoryId == (int)GlobalCodeCategory.PaymentType).Select(x => x).ToList();            }, null, LogSource.PersonService);            return types;        }

        public List<DisputeViewModel> GetAllDisputes()
        {

            var Disputes = new List<DisputeViewModel>();
            Uow.Wrap(uow =>
            {

                Disputes = (from request in _repositoryGoodRequest.Table
                            join dispute in _repositoryDisputes.Table
                            on request.Id equals dispute.RequestId
                            join booking in _repositoryGoodBooking.Table
                            on request.Id equals booking.GoodRequestId
                            where request.StatusId == (int)UserRequestStatus.Dispute
                            select new DisputeViewModel
                            {
                                RequestId = request.Id,
                                DisputeId = dispute.Id,
                                DisputeCreatedBy = dispute.DisputeCreatedBy,
                                DisputeCreatedByName = (from user in _repUser.Table where user.Id == dispute.DisputeCreatedBy select user.FullName).FirstOrDefault(),
                                LastStatus = dispute.LastStatus,
                                LastStatusName = Enum.GetName(typeof(UserRequestStatus), dispute.LastStatus),
                                Reason = dispute.Reason,
                                ReasonName = Enum.GetName(typeof(DisputeReasons), dispute.Reason),
                                Description = dispute.Description,
                                DisputeCreatedDate = dispute.CreateDate,
                                DisputeModDateDate = dispute.ModDate,
                                BorrowerId = request.UserId,
                                BorrowerName = (from user in _repUser.Table where user.Id == request.UserId select user.FullName).FirstOrDefault(),
                                OwnerId = (from usergood in _repositoryUserGood.Table where usergood.GoodId == request.GoodId select usergood.UserId).FirstOrDefault(),
                                OwnerName = (from usergood in _repositoryUserGood.Table
                                             join users in _repUser.Table on usergood.UserId equals users.Id
                                             where usergood.GoodId == request.GoodId
                                             select users.FullName).FirstOrDefault(),

                                GoodId = request.GoodId,
                                Price = request.Price,
                                Days = request.Days,
                                DaysCost = request.DaysCost,
                                CustomerCost = request.CustomerCost,
                                CustomerServiceFeeCost = request.CustomerServiceFeeCost,
                                CustomerCharityCost = request.CustomerCharityCost,
                                SharerCost = request.SharerCost,
                                SharerServiceFeeCost = request.SharerServiceFeeCost,
                                SharerCharityCost = request.SharerCharityCost,
                                DiliveryCost = request.DiliveryCost,
                                ShippingDistance = request.ShippingDistance,
                                DiliveryPrice = request.DiliveryPrice,
                                SecurityDeposit = request.SecurityDeposit,
                                DiscountAmount = request.DiscountAmount,
                                CouponCode = request.CouponCode,
                                PendingAmount = request.PendingAmount,
                                RentalStartDate = booking.StartDate,
                                RentalEndDate = booking.EndDate,
                                RentalStartTime = booking.StartTime,
                                RentalEndTime = booking.EndTime,
                            }).ToList();
            }, null, LogSource.GoodRequestService);
            return Disputes;
        }
        public DisputeViewModel GetDispute(int requestId)
        {

            var Dispute = new DisputeViewModel();
            Uow.Wrap(uow =>
            {

                Dispute = (from request in _repositoryGoodRequest.Table
                           join dispute in _repositoryDisputes.Table
                           on request.Id equals dispute.RequestId
                           join booking in _repositoryGoodBooking.Table
                           on request.Id equals booking.GoodRequestId
                           where request.Id == requestId && request.StatusId == (int)UserRequestStatus.Dispute
                           select new DisputeViewModel
                           {
                               RequestId = request.Id,
                               DisputeId = dispute.Id,
                               DisputeCreatedBy = dispute.DisputeCreatedBy,
                               DisputeCreatedByName = (from user in _repUser.Table where user.Id == dispute.DisputeCreatedBy select user.FullName).FirstOrDefault(),
                               LastStatus = dispute.LastStatus,
                               LastStatusName = Enum.GetName(typeof(UserRequestStatus), dispute.LastStatus),
                               Reason = dispute.Reason,
                               ReasonName = Enum.GetName(typeof(DisputeReasons), dispute.Reason),
                               Description = dispute.Description,
                               DisputeCreatedDate = dispute.CreateDate,
                               DisputeModDateDate = dispute.ModDate,
                               BorrowerId = request.UserId,
                               BorrowerName = (from user in _repUser.Table where user.Id == request.UserId select user.FullName).FirstOrDefault(),
                               OwnerId = (from usergood in _repositoryUserGood.Table where usergood.GoodId == request.GoodId select usergood.UserId).FirstOrDefault(),
                               OwnerName = (from usergood in _repositoryUserGood.Table
                                            join users in _repUser.Table on usergood.UserId equals users.Id
                                            where usergood.GoodId == request.GoodId
                                            select users.FullName).FirstOrDefault(),

                               GoodId = request.GoodId,
                               Price = request.Price,
                               Days = request.Days,
                               DaysCost = request.DaysCost,
                               CustomerCost = request.CustomerCost,
                               CustomerServiceFeeCost = request.CustomerServiceFeeCost,
                               CustomerCharityCost = request.CustomerCharityCost,
                               SharerCost = request.SharerCost,
                               SharerServiceFeeCost = request.SharerServiceFeeCost,
                               SharerCharityCost = request.SharerCharityCost,
                               DiliveryCost = request.DiliveryCost,
                               ShippingDistance = request.ShippingDistance,
                               DiliveryPrice = request.DiliveryPrice,
                               SecurityDeposit = request.SecurityDeposit,
                               DiscountAmount = request.DiscountAmount,
                               CouponCode = request.CouponCode,
                               PendingAmount = request.PendingAmount,
                               RentalStartDate = booking.StartDate,
                               RentalEndDate = booking.EndDate,
                               RentalStartTime = booking.StartTime,
                               RentalEndTime = booking.EndTime,
                           }).FirstOrDefault();
            }, null, LogSource.GoodRequestService);

            return Dispute;
        }

        public ResolvedDisputeViewModel GetResolvedDisputeInfo(int requestId)
        {
            ResolvedDisputeViewModel result = null;

            var dispute = GetDispute(requestId);
            if (dispute != null)
            {
                result = new ResolvedDisputeViewModel();
                result.RequestId = dispute.RequestId;
                result.DisputeId = dispute.DisputeId;
                result.TotalPaidAmount = dispute.CustomerCost + dispute.SecurityDeposit;
                result.BorrowerShare = 0;
                result.OwnerShare = 0;
                result.MomentarilyShare = (dispute.CustomerServiceFeeCost + dispute.CustomerCharityCost + dispute.SharerServiceFeeCost + dispute.SharerCharityCost);
                result.AmountLimitToPay = (dispute.CustomerCost + dispute.SecurityDeposit) - (dispute.CustomerServiceFeeCost + dispute.CustomerCharityCost + dispute.SharerServiceFeeCost + dispute.SharerCharityCost);
                result.FinalRentalReason = dispute.Reason;
                result.Description = "";
            }
            return result;
        }
        public bool SaveResolvedDisputeDetail(ResolvedDisputeViewModel model)
        {
            bool result = false;
            Uow.Wrap(uow =>
            {
                ResolvedDisputeDetail entity = new ResolvedDisputeDetail();

                entity.RequestId = model.RequestId;
                entity.DisputeId = model.DisputeId;
                entity.TotalPaidAmount = model.TotalPaidAmount;
                entity.BorrowerShare = model.BorrowerShare;
                entity.OwnerShare = model.OwnerShare;
                entity.MomentarilyShare = model.MomentarilyShare;
                entity.AmountLimitToPay = model.AmountLimitToPay;
                entity.FinalRentalReason = model.FinalRentalReason;
                entity.Description = model.Description;
                entity.CreateDate = DateTime.Now;
                entity.CreateBy = 1;
                entity.ModDate = DateTime.Now;
                entity.ModBy = 1;
                _repositoryResolvedDisputeDetail.Save(entity);
                result = true;

            }, null, LogSource.GoodRequestService);
            return result;
        }

        public List<ResolvedDisputeViewModel> GetResolvedDisputes()
        {
            List<ResolvedDisputeViewModel> result = new List<ResolvedDisputeViewModel>();
            Uow.Wrap(uow =>
            {

                var data = _repositoryResolvedDisputeDetail.Table.ToList();
                if (data != null && data.Count > 0)
                {
                    foreach (var item in data)
                    {
                        ResolvedDisputeViewModel obj = new ResolvedDisputeViewModel();
                        obj.RequestId = item.RequestId;
                        obj.DisputeId = item.DisputeId;
                        obj.TotalPaidAmount = item.TotalPaidAmount;
                        obj.BorrowerShare = item.BorrowerShare;
                        obj.OwnerShare = item.OwnerShare;
                        obj.MomentarilyShare = item.MomentarilyShare;
                        obj.AmountLimitToPay = item.AmountLimitToPay;
                        obj.FinalRentalReason = item.FinalRentalReason;
                        obj.Description = item.Description;
                        result.Add(obj);
                    }
                }


            }, null, LogSource.GoodRequestService);
            return result;
        }

        public bool SaveDisputeDetail(RequestChangeStatusViewModel model, int UserId, IUnitOfWork unitOfWork = null)
        {
            var result = false;
            try
            {
                Uow.Wrap(u =>
                {
                    Disputes dispute = new Disputes();
                    dispute.RequestId = model.Id;
                    dispute.DisputeCreatedBy = UserId;
                    dispute.LastStatus = model.StatusId;
                    dispute.Reason = model.ReasonId;
                    dispute.Description = model.Message;
                    dispute.CreateDate = DateTime.Now;
                    dispute.ModDate = DateTime.Now;
                    dispute.CreateBy = UserId;
                    dispute.ModBy = UserId;
                    _repositoryDisputes.Save(dispute);
                    result = true;
                },
                unitOfWork,
                LogSource.GoodRequestService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.GoodRequestService, string.Format("Dispute save failed. Ex: {0}.", ex));
            }
            return result;
        }

        public int getUserIdByGoodId(int goodId)
        {
            var userId = 0;
            try
            {
                Uow.Wrap(u =>
                {
                    userId = (from usergood in _repositoryUserGood.Table where usergood.GoodId == goodId select usergood.UserId).FirstOrDefault();
                }, null, LogSource.GoodRequestService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.GoodRequestService, string.Format("Ccannot get userid. Ex: {0}.", ex));
            }

            return userId;
        }


        public PayoutDetailsModel getPaypalDetail(int userId)
        {
            var PaypalDetail = new PayoutDetailsModel();
            try
            {
                Uow.Wrap(u =>
                {
                    PaypalDetail = (from inf in _repositoryPaypalInfoPaymentDetail.Table
                                    where inf.UserId == userId
                                    select new PayoutDetailsModel
                                    {
                                        UserId = inf.UserId,
                                        AccountNumber = inf.AccountNumber,
                                        RoutingNumber = inf.RoutingNumber,
                                        Locality = inf.Locality,
                                        PostalCode = inf.PostalCode,
                                        Region = inf.Region,
                                        StreetAddress = inf.StreetAddress,
                                        PaypalBusinessEmail = inf.PaypalBusinessEmail,
                                        PaymentType = inf.PaymentType,
                                        PhoneNumber = inf.PhoneNumber
                                    }).FirstOrDefault();
                }, null, LogSource.GoodRequestService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.GoodRequestService, string.Format("Ccannot get PaypalDetail. Ex: {0}.", ex));
            }

            return PaypalDetail;
        }

        public Result<GoodRequest> GetRequest(int requestId)
        {
            var result = new Result<GoodRequest>(CreateResult.Error, new GoodRequest());
            Uow.Wrap(uow =>
            {

                result.Obj = (_repositoryGoodRequest.Table.Where(x => x.Id == requestId).FirstOrDefault());
                result.CreateResult = CreateResult.Success;
            }, null, LogSource.GoodRequestService);

            return result;
        }
        public bool UpdateGoodRequest(GoodRequest request)
        {
            var result = false;
            try
            {
                Uow.Wrap(u =>
                {
                    _repositoryGoodRequest.Update(request);
                    result = true;

                }, null, LogSource.GoodRequestService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.GoodRequestService, string.Format("Change user request status fail. Ex: {0}.", ex));
            }
            return result;
        }

        public List<GoodRequestViewModel> GetRentedItems(int userid)
        {
            List<GoodRequestViewModel> requests = new List<GoodRequestViewModel>();
            Uow.Wrap(uow =>
            {
                requests = (from request in _repositoryGoodRequest.Table
                            join usergood in _repositoryUserGood.Table on request.GoodId equals usergood.GoodId
                            join booking in _repositoryGoodBooking.Table on request.Id equals booking.GoodRequestId
                            join good in _repositoryGoodBooking.TableFor<Good>() on request.GoodId equals good.Id
                            where usergood.UserId == userid
                            select new GoodRequestViewModel
                            {
                                Id = request.Id,
                                StatusId = request.StatusId,
                                StatusName = Enum.GetName(typeof(UserRequestStatus), request.StatusId),
                                StartDate = booking.StartDate,
                                EndDate = booking.EndDate,
                                GoodName = good.Name,
                                GoodImageUrl = (from img in _repositoryGoodImg.Table
                                                where img.GoodId == request.GoodId
                                                && img.Type == (int)ImageType.Original
                                                select img.FileName).FirstOrDefault(),
                                CreateDate = request.CreateDate
                            }).ToList();
                requests = requests.GroupBy(x => x.Id).Select(y => y.First()).OrderByDescending(x => x.CreateDate).ToList();


            }, null, LogSource.GoodRequestService);

            return requests;
        }

        public bool getExsistPaypalInfoOrNotInDb(int userId)        {            var result = false;            Uow.Wrap(u =>            {                result = _repositoryPaypalInfoPaymentDetail.Table.Any(x => x.UserId == userId);            }, null, LogSource.UserMessageService);            return result;        }

        public User GetGoodBasedUser(int goodId)
        {

            User user = null;
            Uow.Wrap(u =>
            {
                user = (from usr in _repUser.Table join userGood in _repositoryUserGood.Table on usr.Id equals userGood.UserId where userGood.GoodId == goodId select usr).FirstOrDefault();
            }, null, LogSource.PersonService);
            return user;
        }
        public Result<User> ManageUserOTP(int userId, int OTPAllowed)
        {
            try
            {
                User userDetail = null;
                Result<User> _MainResponse = new Result<User>();
                Uow.Wrap(u =>
                {
                    userDetail = _repUser.GetUser(userId);
                }, null, LogSource.UserMessageService);
               
                if (!ReferenceEquals(userDetail, null))
                {
                    if (Convert.ToBoolean(userDetail.IsLockout) && userDetail.OTPCount >= OTPAllowed)
                    {
                        if (CheckIn24Hours(userDetail.OTPGeneratedDate))
                        {
                            userDetail.IsLockout = false;
                            userDetail.OTPCount = 0;
                            Uow.Wrap(u =>
                            {
                                _repUser.Update(userDetail);
                            }, null, LogSource.UserMessageService);

                            _MainResponse.Success = true;

                        }
                        else
                        {
                            _MainResponse.Message = "Please try for an OTP after 24 hours. Thanks!";
                            _MainResponse.Success = false;
                            _MainResponse.StatusCode = "OTP_002";
                            _MainResponse.Obj = new User() { IsLockout = userDetail.IsLockout, Id = userDetail.Id };
                        }
                    }
                    else if (userDetail.OTPCount >= OTPAllowed)
                    {
                        userDetail.IsLockout = true;
                        Uow.Wrap(u =>
                        {
                            _repUser.Update(userDetail);
                        }, null, LogSource.UserMessageService);
                        _MainResponse.Message = "Please try for an OTP after 24 hours. Thanks!";
                        _MainResponse.Success = false;
                        _MainResponse.StatusCode = "OTP_003";
                        _MainResponse.Obj = new User() { IsLockout = userDetail.IsLockout, Id = userDetail.Id };
                    }
                    else
                    {
                        _MainResponse.Success = true;
                    };
                }
                else
                {
                    _MainResponse.Success = false;
                    _MainResponse.Message = "Invalid User, Not Exist";
                    _MainResponse.StatusCode = "OTP_001";
                }
                return _MainResponse;
            }
            catch (Exception ex)
            {
                return null;
                //throw;
            }
            
        }
        public bool UpdateOTPRequests(int userId)
        {
            try
            {
                User userDetail = null;
                Uow.Wrap(u =>
                {
                    userDetail = _repUser.GetUser(userId);
                }, null, LogSource.UserMessageService);
                if (!ReferenceEquals(userDetail, null))
                {
                    userDetail.OTPCount ++;
                    userDetail.OTPGeneratedDate = DateTime.UtcNow;
                    Uow.Wrap(u =>
                    {
                        _repUser.Update(userDetail);
                    }, null, LogSource.UserMessageService);
                }
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }
        public  bool CheckIn24Hours(DateTime? date)
        {
            DateTime booking = Convert.ToDateTime(date);
            DateTime ending = booking.AddHours(23).AddMinutes(59).AddSeconds(59);
            var n = DateTime.Compare(DateTime.UtcNow, ending);
            return ((n == -1)) ? false : true;
        }
    }
}
