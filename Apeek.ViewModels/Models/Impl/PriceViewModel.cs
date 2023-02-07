using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Apeek.ViewModels.Models
{
    public class PriceViewModel
    {       
        public int GoodId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Days { get; set; }
        public double Price { get; set; }
        public double PricePerWeek { get; set; }
        public double PricePerMonth { get; set; }
        public double PerDayCost { get; set; }
        public double DaysCost { get; set; }
        public double CustomerCost { get; set; }        
        public double CustomerServiceFee { get; set; }
        public double CustomerServiceFeeCost { get; set; }
        public double CustomerCharity { get; set; }
        public double CustomerCharityCost { get; set; }
        public double SharerCost { get; set; }
        public double SharerServiceFee { get; set; }
        public double SharerServiceFeeCost { get; set; }
        public double SharerCharity { get; set; }
        public double SharerCharityCost { get; set; }
        public double DiliveryCost { get; set; }       
        public double ShippingDistance { get; set; }        
        public double DiliveryPrice { get; set; }
        public double CouponDiscount { get; set; }
    }
}
