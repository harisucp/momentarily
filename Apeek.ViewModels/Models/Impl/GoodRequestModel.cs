using System;
namespace Apeek.ViewModels.Models
{
    public class RequestModel
    {
        public int GoodId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string StartTime { get; set; }        public string EndTime { get; set; }
        public string ShippingAddress { get; set; }
        public double ShippingDistance { get; set; }        
        public bool ApplyForDelivery { get; set; }
        public bool AgreeToShareImmediately { get; set; }

        public double CouponDiscount { get; set; }
        public string CouponCode { get; set; }




    }
}
