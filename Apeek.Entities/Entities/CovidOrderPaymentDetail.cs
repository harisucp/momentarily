using Apeek.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.Entities.Entities
{
    public class CovidOrderPaymentDetail : AuditEntity
    {
        public static string _TableName = "p_paypal_covid_order_detail";
        #region AuditEntity implementation
        public override string TableName { get { return _TableName; } }
        public override int Id { get; set; }
        #endregion
        public virtual int CovidOrderId { get; set; }
        public virtual string PayId { get; set; }
        public virtual string Intent { get; set; }
        public virtual string State { get; set; }
        public virtual string Amount { get; set; }
        public virtual string Description { get; set; }
        public virtual string InvoiceNumber { get; set; }
        public virtual string CreateTime { get; set; }
        public virtual string UpdateTime { get; set; }
        public virtual string PayerEmail { get; set; }
        public virtual string PayerId { get; set; }
        public virtual string Cart { get; set; }
    }
}