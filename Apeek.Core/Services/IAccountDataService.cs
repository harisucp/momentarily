using System;
using System.Collections.Generic;
using Apeek.Common;
using Apeek.Common.Interfaces;
using Apeek.Common.Models;
using Apeek.Entities.Entities;
using Apeek.Entities.Web;
using Apeek.NH.DataAccessLayer.DataAccess;
using Apeek.ViewModels.Models;
using Apeek.ViewModels.Models.Impl;
using Momentarily.ViewModels.Models;
using Momentarily.ViewModels.Models.Braintree;

namespace Apeek.Core.Services
{
    public interface IAccountDataService : IDependency
    {
        LoginResult Login(string userEmailOrPhoneNumber, string pwdOrTempPwd);
        LoginResult GetBlockedUser(string userEmailOrPhoneNumber, string pwdOrTempPwd);

        LoginResult AdminLogin(string userEmailOrPhoneNumber, string pwdOrTempPwd);
        User GetUser(string verificationCode, string email);
        User GetUserByEmail(string email);
        void RegenerateVerificationCode(User user);
        User GetUserByPhoneNumber(string phoneNum, IUnitOfWork unitOfWork = null);
        void GenerateTempPwd(User user);
        User GetUser(string verificationCode);
        User GetUser(int personId);
        User GetGoogleUser(string googleId, string email);
        User GetFacebookUser(string facebookId, string email);
        string[] GetUserPrivilages(int userId);
        //CreateResult Update(PersonViewModel personViewModel, int modBy);
        /*List<Address> GetAddressViewModelsFor(User person);*/
        //List<PersonBrowseModel> GetUserBrowseModels(int? serviceId, int? locationId, int langId, bool includeRejectedServices, int? offset = 0, int? count = 1, int? createBy = null);
        //PersonBrowseModel GetUserBrowseModel(int personid, int langId, bool verifiedOnly = true, bool includeRejectedServices = false, IUnitOfWork unitOfWork = null);
        //LocationBrowseModel GetLocationBrowseModel(int personId, int langId);
        /*int GetUsersCount(int serviceId, int locationId, int? createBy = null);*/
        User UpdateEmailAndGetUser(string email, string verificationCode);
        bool VerifyUser(string verificationCode, out User personToReturn);
        bool VerifyMobile(string otp, string vc);
        bool VerifyMobileLink(string vc);
        UserEmailModel GetUserEmailModel(int userId);
        bool SetUserEmailModel(UserEmailModel userEmailModel, int userId);
        Result<User> SetUserPwd(UserPwdModel userSettingsModel, int userId);
        /*bool ExistsPhoneNumber(string phoneNum, IUnitOfWork unitOfWork = null);*/
        /*bool ExistsHistoryPhoneNumber(string phoneNum, IUnitOfWork unitOfWork = null);*/
        /*HistoryUser GetHistoryUserbyPhoneNumber(string phoneNum);*/
        /*void DeleteHistoryUser(int historyUserId);*/
        User GetUserByName(string personeName);
        PersonFreeText GetUserFreeText(int userId);
        void SetUserFreeText(PersonFreeText personFreeText);
        /*AccountCompletenessViewModel GetAccountCompletenessViewModel(int userId);*/
        VerifyUserEmail CreateVerifyUserEmail(string email, bool onlyNewMails = false);
        VerifyUserEmail GetVerifyUserEmail(string vc, string email);
        /*bool DeleteUser(int id);*/
        //PersonViewModel CreateUserViewModel();
        void SaveUserAccountAssociation(UserAccountAssociation association);
        void UpdateUserAccountAssociation(UserAccountAssociation association);
        UserAccountAssociation GetAccountAssociation(int userId, string providerName);
        User GetUserByExternalId(string externalId, string providerName);
        List<UserAccountAssociation> GetUserAccountAssociations(int userId);
        bool DeleteUserAccountAssociations(int userId, string providerName);
        /*LocationLang GetLocationLang(int personId);*/
        //List<PersonBrowseModel> GetUserBrowseModelsForService(int personId, int serviceId, int locationId, int langId, int offset, int count);
        //List<PersonBrowseModel> GetSimilarUserBrowseModels(int personId, int locationId, int langId, int count = 5);
        /*void TrySetUserResponce(int userId);*/
        void CreateUserInvitationHistory(int userId, NotificationType notificationType, string contact);
        bool IsInvitationSentToContact(string contact);
        string GetUserVerificationCode(int userId);
        VerifyResult VerifyAndDoUserSecurityDataChangeRequest(string vc);
        UserSecurityDataChangeRequest CreateUserSecurityDataChangeRequest(int dataType, string newValue, string oldValue, int userId);
        //TODO: need to detele
        Result<User> Register(string userEmail, string pwd, DateTime dateOfBirthday);
        Result<User> Update(int userId, IUserUpdateModel userUpdateModel);
        Result<User> Update(IUserUpdateModel model, int modBy);
        Result<User> UpdateGeneralUpdateColumn(int userId);
        Result<User> Register(IRegisterModel model, Func<User> modelToEntityMapper);
        Result<User> ValidatePhonenumber(IRegisterModel model, Func<User> modelToEntityMapper);
        Result<UserUpdateModel> GetUserUpdateModel(int userId);
        string GetUserPhone(int userId);
        Result<User> AddPhoneNumber(int userId, string phoneNumber, string countrycode);
        Result<User> SetAdminPwd(AdminPwdModel userSettingsModel, int userId);


        bool SubscriberAdd(string email);
        bool SubscriberUpdate(string email);
        bool SubscriberUnsubscribedUpdate(string email,bool status);
        bool SubscriberAlreadyExsist(string email);
        bool SubscriberAlreadyExsistOnlyEmail(string email);

        string SubscriberNameGetIfExsist(string email);


        List<Entities.Entities.Countries> GetAllCountries();

        bool SendLaunchSoonEmail(string email,DateTime launchingdate);

        bool SendLaunchedEmail(string email);

        UserCoupon GetDetailThankYouCoupon();
        UserCoupon GetDetailThankYouForSubscriberCoupon();

        bool UserBlockedMessage(int userId, bool checkedValue);
        bool UserBlocked(int userid);

        User DeleteUser(int userid);
        string GetCountryCodeByPhoneNumber(string number);
        Result<PayoutDetailsModel> CreatePaypalInfoPaymentDetail(Shape<PayoutDetailsModel> shape);
        Result<PayoutDetailsModel> GetCurrentPaypalInfoPaymentDetail(int userId);
        int GetOwnerIdbyGoodId(int goodId);
        string getUserPhoneNumberForTemplate(int userid);
        List<Entities.Entities.GlobalCodes> GetPaymentTypes();
        List<DisputeViewModel> GetAllDisputes();
        DisputeViewModel GetDispute(int requestId);
        ResolvedDisputeViewModel GetResolvedDisputeInfo(int requestId);
        bool SaveResolvedDisputeDetail(ResolvedDisputeViewModel model);
        Result<GoodRequest> GetRequest(int requestId);
        int getUserIdByGoodId(int goodId);
        PayoutDetailsModel getPaypalDetail(int userId);
        bool SaveDisputeDetail(RequestChangeStatusViewModel model, int UserId, IUnitOfWork unitOfWork = null);
        bool UpdateGoodRequest(GoodRequest request);
        List<ResolvedDisputeViewModel> GetResolvedDisputes();
        List<GoodRequestViewModel> GetRentedItems(int userid);
        bool getExsistPaypalInfoOrNotInDb(int userId);

        User GetGoodBasedUser(int goodId);
        Result<User> ManageUserOTP(int userId,int AllowedOTP);
        bool UpdateOTPRequests(int userId);
    }
}