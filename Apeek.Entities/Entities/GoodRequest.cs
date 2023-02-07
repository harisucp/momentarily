using Apeek.Entities.Interfaces;
using System;
namespace Apeek.Entities.Entities
{
    public class GoodRequest : AuditEntity
    {
        public static string _TableName = "c_good_request";
        public override int Id { get; set; }
        public override string TableName { get { return _TableName; } }
        public virtual int UserId { get; set; }
        public virtual int GoodId { get; set; }
        public virtual int StatusId { get; set; }
        public virtual double Price { get; set; }
        public virtual int Days { get; set; }                
        public virtual double DaysCost { get; set; }
        public virtual double CustomerCost { get; set; }
        public virtual double CustomerServiceFee { get; set; }
        public virtual double CustomerServiceFeeCost { get; set; }
        public virtual double CustomerCharity { get; set; }
        public virtual double CustomerCharityCost { get; set; }
        public virtual double SharerCost { get; set; }
        public virtual double SharerServiceFee { get; set; }
        public virtual double SharerServiceFeeCost { get; set; }
        public virtual double SharerCharity { get; set; }
        public virtual double SharerCharityCost { get; set; }
        public virtual double DiliveryCost { get; set; }
        public virtual double ShippingDistance { get; set; }
        public virtual double DiliveryPrice { get; set; }
        public virtual double SecurityDeposit { get; set; }        
        public virtual User User { get; set; }
        public virtual Good Good { get; set; }
        public virtual GoodBooking GoodBooking { get; set; }
        public virtual RequestStatus RequestStatus { get; set; }
        public virtual double DiscountAmount { get; set; }
        public virtual string CouponCode { get; set; }
        public virtual double PendingAmount { get; set; }
        public virtual bool IsUsedCoupon { get; set; }

    }
}
