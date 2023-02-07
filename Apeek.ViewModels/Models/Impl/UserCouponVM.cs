using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.ViewModels.Models.Impl
{
   public class UserCouponVM
    {
        public int Id { get; set; }
        public int CouponType { get; set; }
        public string CouponTypeName { get; set; }
        public string CouponCode { get; set; }
        public int CouponDiscountType { get; set; }
        public string CouponDiscountTypeInPercentage { get; set; }
        public string CouponDiscountTypeInAmount { get; set; }
        public float CouponDiscount { get; set; }
        //[DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime ExpiryDate { get; set; }
        public bool NoExpiryDateStatus { get; set; }
        public string NoExpiryDateStatusInString { get; set; }
        public bool Status { get; set; }
        public string StatusString { get; set; }
        public List<UserCouponVM> userCouponVMsList { get; set; }
    }

    public class CouponType
    {
        public int Id { get; set; }
        public string CouponName { get; set; }
    }
}
