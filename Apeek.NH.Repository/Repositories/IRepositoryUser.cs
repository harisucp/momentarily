using Apeek.Common.Interfaces;
using Apeek.Entities.Entities;
using Apeek.NH.Repository.Common;
using System.Collections.Generic;

namespace Apeek.NH.Repository.Repositories
{
    public interface IRepositoryUser : IRepositoryAudit<User>, IDependency
    {
        User GetUser(string createGuid);
        User GetUser(int userId);
        User GetGoogleUser(string googleId, string email); 
        User GetFacebookUser(string facebookId, string email); 
         User GetUserByEmail(string userEmail);
        User GetBlockedUserByEmail(string userEmail);

        User GetUserByPhoneNumber(string phoneNumber);
        void UpdateLastVisitDate(User user);

        User GetAdminByEmail(string userEmail);
        User GetAdminByPhoneNumber(string phoneNumber);
        int GetUserCount();
        int GetAvailableItemsCount();
        List<User> GetAllUser();
        int UserBlockedStatusChanged(int userId, bool checkedValue);
        string GetCountryCodeByPhoneNumber(string number);
        User GetUserByEmailForLogin(string email);        User GetUserByPhoneNumberForLogin(string phoneNum);

        //bool GetPaymentProcessExceptionControl(string message,int requestId,string excpMess);
        

    }
}