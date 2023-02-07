using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Momentarily.ViewModels.Models
{
    public class CovidGoodViewModel
    {
        public int CovidGoodId { get; set; }
        public string GoodName { get; set; }
        public string GoodDescription { get; set; }
        [RegularExpression(@"^\d+\.\d{0,2}$")] //this line
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public double GoodPrice { get; set; }
        public string GoodImage { get; set; }
        public int Quantity { get; set; }
        public StockMasterDetailVM stockMasterDetail { get; set; }
        public double TotalPrice { get; set; }
        public string BuyerEmailId { get; set; }
        public string OrderDescription { get; set; }
        public int StatusId { get; set; }
        public double Tax { get; set; }
        public double DeliveryCharge { get; set; }
        public string DeliveryAddress1 { get; set; }
        public string DeliveryAddress2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
        public string FullName { get; set; }
        
        public bool IgnoreMarketingMails { get; set; }

    }
    public class CovidGoodOrderViewModel
    {
        public int Id { get; set; }
        public int CovidGoodId { get; set; }
        public string CovidGoodName { get; set; }
        public double OrderPrice { get; set; }
        public int Quantity { get; set; }
        public int QuantityLeft { get; set; }
        public double TotalPrice { get; set; }
        public string BuyerEmailId { get; set; }
        public string Description { get; set; }
        public int StatusId { get; set; }
        public string StatusName { get; set; }
        public double Tax { get; set; }
        public double DeliveryCharge { get; set; }
        public string DeliveryAddress1 { get; set; }
        public string DeliveryAddress2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
        public string FullName { get; set; }
        public bool IgnoreMarketingMails { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime Moddate { get; set; }
    }
    public class CovidOrderPaymentDetailViewModel
    {
        public int CovidOrderId { get; set; }
        public string PayId { get; set; }
        public string Intent { get; set; }
        public string State { get; set; }
        public string Amount { get; set; }
        public string Description { get; set; }
        public string InvoiceNumber { get; set; }
        public string CreateTime { get; set; }
        public string UpdateTime { get; set; }
        public string PayerEmail { get; set; }
        public string PayerId { get; set; }
        public string Cart { get; set; }
    }
}
