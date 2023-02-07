using Apeek.Entities.Entities;
using Apeek.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.Entities.Mappings
{
  public class UserCouponMap : AuditEntityMap<UserCoupon>
    {
        public UserCouponMap()
        {
            Table("c_user_coupon");
            Id(x => x.Id, "id");
            Map(x => x.CouponType, "coupon_type");
            Map(x => x.CouponCode, "coupon_code");
            Map(x => x.CouponDiscountType, "coupon_discount_type");
            Map(x => x.CouponDiscount, "coupon_discount");
            Map(x => x.ExpiryDate, "expiry_date");
            Map(x => x.NoExpiryDateStatus, "no_expiry_date_status");
            Map(x => x.Status, "status");
        }
    }
}
