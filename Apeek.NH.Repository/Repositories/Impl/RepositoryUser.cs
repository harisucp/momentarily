using System;
using System.Collections.Generic;
using System.Linq;
using Apeek.Common;
using Apeek.Common.Logger;
using Apeek.Entities.Entities;
using Apeek.NH.Repository.Common;
namespace Apeek.NH.Repository.Repositories.Impl
{
    public class RepositoryUser : RepositoryAudit<User>, IRepositoryUser
    {
        public User GetUser(string verificationCode)
        {
            return Table.Where(x => x.VerificationCode == verificationCode).Select(x => x).FirstOrDefault();
        }
        public void VerifyUser(User user)
        {
            Session.CreateSQLQuery("Update c_person set verified=1 where email=:email and verification_code=:vc")
                .SetString("email", user.Email)
                .SetString("vc", user.VerificationCode)
                .ExecuteUpdate();
        }
        public void VerifyUserById(int userId)
        {
            Session.CreateSQLQuery("Update c_person set verified=1 where id=:userId")
                .SetInt32("userId", userId)
                .ExecuteUpdate();
        }
        public User GetUserByEmail(string email)
        {
            var persons = Table.Where(x => x.Email == email && x.IsAdmin == false && x.IsBlocked == false && x.IsRemoved==false).Select(x => x).ToList();
            if (persons.Count > 1)
                throw new ApeekException(string.Format("Multiple users with the same email found: {0}", email));
            return persons.FirstOrDefault();
        }
        public User GetBlockedUserByEmail(string email)
        {
            var persons = Table.Where(x => x.Email == email && x.IsAdmin == false && x.IsBlocked == true).Select(x => x).ToList();
            return persons.FirstOrDefault();
        }

        public User GetAdminByEmail(string email)
        {
            var persons = Table.Where(x => x.Email == email && x.IsAdmin == true && x.IsBlocked == false).Select(x => x).ToList();
            if (persons.Count > 1)
                throw new ApeekException(string.Format("Multiple admin's with the same email found: {0}", email));
            return persons.FirstOrDefault();
        }
        public User GetUser(int personId)
        {
            return Table.Where(x => x.Id == personId).Select(x => x).FirstOrDefault();
        }
        public User GetGoogleUser(string googleId, string email)
        {
            
            return Table.Where(x => x.GoogleId == googleId || x.Email == email).Select(x => x).FirstOrDefault();
        }
        public User GetFacebookUser(string facebookId, string email)
        {
            return Table.Where(x => x.FacebookId == facebookId || x.Email == email).Select(x => x).FirstOrDefault();
        }
        public User GetUserByPhoneNumber(string phoneNum)
        {
            var persons = (from phn in TableFor<PhoneNumber>()
                    join adr in TableFor<Address>() on phn.Address.Id equals adr.Id
                      join p in Table on adr.User.Id equals p.Id
                      where phn.PhoneNum == phoneNum && p.IsAdmin == false && p.IsBlocked == false  && p.IsRemoved == false
                           select p).ToList();
            if(persons.Count > 1)
                Ioc.Get<IDbLogger>().LogError(LogSource.RepositoryUser, string.Format("Multiple users with the same phone number found: {0}", phoneNum));
            return persons.FirstOrDefault();
        }

        public User GetAdminByPhoneNumber(string phoneNum)
        {
            var persons = (from phn in TableFor<PhoneNumber>()
                           join adr in TableFor<Address>() on phn.Address.Id equals adr.Id
                           join p in Table on adr.User.Id equals p.Id
                           where phn.PhoneNum == phoneNum && p.IsAdmin == true && p.IsBlocked == false
                           select p).ToList();
            if (persons.Count > 1)
                Ioc.Get<IDbLogger>().LogError(LogSource.RepositoryUser, string.Format("Multiple admin's with the same phone number found: {0}", phoneNum));
            return persons.FirstOrDefault();
        }
        public void UpdateLastVisitDate(User user)
        {
            var sql = "Update c_user set last_visit_date=:lvd where id=:personId";
            Session.CreateSQLQuery(sql)
                .SetDateTime("lvd", DateTime.Now)
                .SetInt32("personId", user.Id)
                .ExecuteUpdate();
        }
        public int GetUserCount()
        {
            return Table.Where(x => x.IsAdmin != true && x.Verified == true&& x.IsRemoved != true).Select(x => x.Id).Count();
        }

        public int GetAvailableItemsCount()
        {
            var items = (from g in TableFor<Good>()
                         where g.IsArchive == false
                         select g);

            return 0;
        }
       public List<User> GetAllUser()
        {
            return Table.Where(x => x.IsAdmin == false && x.Verified == true&& x.IsRemoved != true).Select(x => x).ToList();
        }
        public int UserBlockedStatusChanged(int userId, bool checkedValue)
        {
            int result = 0;
            int checkedIntValue =0;
            if (checkedValue == false)
                checkedIntValue = 0;
            else
                checkedIntValue = 1;

            if (userId == 0)
            {
                result = 0;
            }
            else
            {
                result = Session.CreateSQLQuery("Update c_user set is_blocked='"+ checkedIntValue + "' where id=:userId")
                 .SetInt32("userId", userId)
                 .ExecuteUpdate();
            }
            return result;

        }
        public string GetCountryCodeByPhoneNumber(string number)        {            var code = Convert.ToString((from phone in TableFor<PhoneNumber>() where phone.PhoneNum == number select phone.CountryCode).FirstOrDefault());            return code;        }
        public User GetUserByPhoneNumberForLogin(string phoneNum)        {            var persons = (from phn in TableFor<PhoneNumber>()                           join adr in TableFor<Address>() on phn.Address.Id equals adr.Id                           join p in Table on adr.User.Id equals p.Id                           where phn.PhoneNum == phoneNum && p.IsAdmin == false && p.IsRemoved == false                           select p).ToList();            if (persons.Count > 1)                Ioc.Get<IDbLogger>().LogError(LogSource.RepositoryUser, string.Format("Multiple users with the same phone number found: {0}", phoneNum));            return persons.FirstOrDefault();        }
        public User GetUserByEmailForLogin(string email)        {            var persons = Table.Where(x => x.Email == email && x.IsAdmin == false && x.IsRemoved == false).Select(x => x).ToList();            if (persons.Count > 1)                throw new ApeekException(string.Format("Multiple users with the same email found: {0}", email));            return persons.FirstOrDefault();        }       //public bool GetPaymentProcessExceptionControl(string message, int requestId, string excpMess)
       // {
       //     bool result = false;
       //     if(message !="" && requestId!=0)
       //     {
       //         Ioc.Get<IDbLogger>().LogMessage(LogSource.PayPalPaymentService, string.Format(message + requestId));
       //         result = true;
       //     }
       //     else if(message != "" && requestId != 0 && excpMess != "")
       //     {
       //         Ioc.Get<IDbLogger>().LogWarning(LogSource.PayPalPaymentService, string.Format(message + requestId + ". Ex: {0}.", excpMess));
       //     }

       //     return result;
       // }
    }
}