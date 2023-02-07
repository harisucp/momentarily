using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Momentarily.ViewModels.Models
{
    public class DisputeViewModel
    {
        public  int RequestId { get; set; }
        public int DisputeId { get; set; }
        public int DisputeCreatedBy { get; set; }
        public string DisputeCreatedByName { get; set; }
        public int LastStatus { get; set; }
        public string LastStatusName { get; set; }
        public int Reason { get; set; }
        public string ReasonName { get; set; }
        public  string Description { get; set; }
        public DateTime DisputeCreatedDate { get; set; }
        public DateTime DisputeModDateDate { get; set; }
        public int BorrowerId { get; set; }
        public string BorrowerName { get; set; }
        public int OwnerId { get; set; }
        public string OwnerName { get; set; }
        public int GoodId { get; set; }
        public  double Price { get; set; }
        public  int Days { get; set; }
        public  double DaysCost { get; set; }
        public  double CustomerCost { get; set; }
        public  double CustomerServiceFeeCost { get; set; }
        public  double CustomerCharityCost { get; set; }
        public  double SharerCost { get; set; }
        public  double SharerServiceFeeCost { get; set; }
        public  double SharerCharityCost { get; set; }
        public  double DiliveryCost { get; set; }
        public  double ShippingDistance { get; set; }
        public  double DiliveryPrice { get; set; }
        public  double SecurityDeposit { get; set; }
        public  double DiscountAmount { get; set; }
        public  string CouponCode { get; set; }
        public  double PendingAmount { get; set; }
        public  DateTime RentalStartDate { get; set; }
        public  DateTime RentalEndDate { get; set; }
        public string RentalStartTime { get; set; }
        public string RentalEndTime { get; set; }
       
    }

    public class ResolvedDisputeViewModel
    {
        public int RequestId { get; set; }
        public int DisputeId { get; set; }
        public double TotalPaidAmount { get; set; }
        public double BorrowerShare { get; set; }
        public double OwnerShare { get; set; }
        public double MomentarilyShare { get; set; }
        public double AmountLimitToPay { get; set; }
        public int FinalRentalReason { get; set; }
        public string Description { get; set; }
    }
}
