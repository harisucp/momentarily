using Apeek.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.Entities.Entities
{
 public class PaypalInfoPaymentDetail : AuditEntity
    {
        public static string _TableName = "p_paypal_info_payment_detail";
        public override string TableName { get { return _TableName; } }
        public override int Id { get; set; }
        public virtual int UserId { get; set; }
        public virtual string AccountNumber { get; set; }
        public virtual string RoutingNumber { get; set; }
        public virtual string Locality { get; set; }
        public virtual string PostalCode { get; set; }
        public virtual string Region { get; set; }
        public virtual string StreetAddress { get; set; }
        public virtual string PaypalBusinessEmail { get; set; }
        public virtual string PhoneNumber { get; set; }
        public virtual int PaymentType { get; set; }
    }
}
