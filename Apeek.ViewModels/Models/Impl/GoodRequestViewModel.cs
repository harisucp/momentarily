using Apeek.Entities.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace Apeek.ViewModels.Models
{
    public class GoodRequestViewModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int OwnerId { get; set; }
        public string OwnerName { get; set; }
        public string OwnerEmail { get; set; }
        public string OwnerPhone { get; set; }
        public string PickUpLocation { get; set; }
        public int GoodId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserPhone { get; set; }
        public string Location { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }        
        public string ShippingAddress { get; set; }
        public string GoodName { get; set; }
        public string GoodDescription { get; set; }
        public int StatusId { get; set; }
        public string StatusName { get; set; }        
        public double SecurityDeposit { get; set; }        
        public string PaymentUrl { get; set; }
        public string DepositUrl { get; set; }
        public string Message { get; set; }
        public bool CanCancel { get; set; }
        public bool CanReview { get; set; }
        public bool CanRelease { get; set; }
        public bool CanReceive { get; set; }
        public bool CanReturn { get; set; }
        public bool CanConfirmReturn { get; set; }
        public bool CanStartDispute { get; set; }
        public bool CanRefund { get; set; }
        public DateTime CreateDate { get; set; }
        public string GoodImageUrl { get; set; }
        public double DaysCost { get; set; }
        public double CustomerServiceFee { get; set; }
        public double CustomerCharity { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string StartTime { get; set; }        public string EndTime { get; set; }
        public int Days { get; set; }
        public double Price { get; set; }        
        public double CustomerCost { get; set; }        
        public double CustomerServiceFeeCost { get; set; }        
        public double CustomerCharityCost { get; set; }
        public double SharerCost { get; set; }
        public double SharerServiceFee { get; set; }
        public double SharerServiceFeeCost { get; set; }
        public double SharerCharity { get; set; }
        public double SharerCharityCost { get; set; }
        public double DiliveryCost { get; set; }
        public double ShippingDistance { get; set; }
        public double DiliveryPrice { get; set; }
        public bool ApplyForDelivery { get; set; }
        public double CouponDiscount { get; set; }
        public string CouponCode { get; set; }
        public bool IsUsedCoupon { get; set; }
        public int ReasonId { get; set; }
        public bool IsViewed { get; set; }
        public FinalFeedbackVM finalFeedbackVM { get; set; }
    }

    public class FinalFeedbackVM    {        public int RequestId { get; set; }        public bool NoIssue { get; set; }        public bool Late { get; set; }        public bool Damaged { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime ReturnDate { get; set; }        public string ReturnTime { get; set; }        public double Claim { get; set; }        public string Description { get; set; }    }
}
