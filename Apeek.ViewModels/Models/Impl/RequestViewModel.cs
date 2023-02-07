using System;
namespace Apeek.ViewModels.Models
{
    public class RequestViewModel
    {
        public int UserId { get; set; }
        public int GoodId { get; set; }
        public string GoodName { get; set; }
        public string GoodDescription { get; set; }
        public string GoodType { get; set; }
        public string GoodLocation { get; set; }
        public string GoodImageUrl { get; set; }
        public string FirstName { get; set; }
        public string UserName { get; set; }
        public string UserImageUrl { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string StartTime { get; set; }        public string EndTime { get; set; }
        public int Days { get; set; }
        public double Price { get; set; }
        public double PricePerWeek { get; set; }
        public double PricePerMonth { get; set; }
        public double PerDayCost { get; set; }
        public double DaysCost { get; set; }        
        public double Cost { get; set; }
        public double SecurityDeposit { get; set; }
        public double ServiceFee { get; set; }
        public double ServiceFeeCost { get; set; }        
        public double Charity { get; set; }
        public double CharityCost { get; set; }        
        public double DiliveryCost { get; set; }
        public double DiliveryPrice { get; set; }
        public string ShippingAddress { get; set; }
        public double ShippingDistance { get; set; }
        public bool ApplyForDelivery { get; set; }
        public bool SharerAgreeToShareImmediately { get; set; }
        public bool CurrentUserIsOwner { get; set; }
        public double Deposit { get; set; }
        public double CouponDiscount { get; set; }
        public string CouponCode { get; set; }
        public virtual bool IsUsedCoupon { get; set; }
    }
}
