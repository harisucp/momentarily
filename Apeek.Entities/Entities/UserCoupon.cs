using Apeek.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.Entities.Entities
{
   public class UserCoupon : AuditEntity
    {
        public static string _TableName = "c_user_coupon";
        public override string TableName { get { return _TableName; } }
        public override int Id { get; set; }
        public virtual int CouponType { get; set; }
        public virtual string CouponCode { get; set; }
        public virtual int CouponDiscountType { get; set; }
        public virtual float CouponDiscount { get; set; }
        public virtual DateTime ExpiryDate { get; set; }
        public virtual bool NoExpiryDateStatus { get; set; }
        public virtual bool Status { get; set; }
    }
}
