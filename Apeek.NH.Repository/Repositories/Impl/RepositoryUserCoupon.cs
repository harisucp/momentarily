using Apeek.Entities.Entities;
using Apeek.NH.Repository.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.NH.Repository.Repositories.Impl
{
   public class RepositoryUserCoupon : RepositoryAudit<UserCoupon>, IRepositoryUserCoupon
    {

        public int CouponBlockedStatusChanged(int Id, bool checkedValue)
        {
            int result = 0;
            int checkedIntValue = 0;
            if (checkedValue == false)
                checkedIntValue = 0;
            else
                checkedIntValue = 1;

            if (Id == 0)
            {
                result = 0;
            }
            else
            {
                result = Session.CreateSQLQuery("Update c_user_coupon set status='" + checkedIntValue + "' where id=:Id")
                 .SetInt32("Id", Id)
                 .ExecuteUpdate();
            }
            return result;

        }
    }
}
