using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Apeek.Entities.Interfaces;


namespace Apeek.Entities.Entities
{
   public class PaypalPayment : AuditEntity
    {
        public static string _TableName = "p_paypal_payment_detail";
        public override int Id { get; set; }
        public override string TableName { get { return _TableName; } }
        public virtual int GoodRequestId { get; set; }
        public virtual string PayerId { get; set; }
        public virtual string PayeeId { get; set; }
        public virtual string PaymentCart { get; set; }
        public virtual DateTime PaymentCreatedDate { get; set; }
        public virtual string PaymentPayId { get; set; }
        public virtual string PaymentIntent { get; set; }
        public virtual string PaymentStatus { get; set; }
        public virtual DateTime PaymentUpdateDate { get; set; }
        public virtual string PaymentInvoiceNumber{ get; set; }
        public virtual DateTime AuthorizationCreatedDate { get; set; }
        public virtual string AuthorizationId { get; set; }
        public virtual int AuthorizationCount { get; set; }
        public virtual string AuthorizationStatus { get; set; }
        public virtual DateTime AuthorizationUpdateDate { get; set; }
        public virtual DateTime AuthorizationValidUntill { get; set; }
        public virtual DateTime CaptureCreatedDate { get; set; }
        public virtual string CaptureId { get; set; }
        public virtual bool CaptureIsFinal { get; set; }
        public virtual string CaptureStatus { get; set; }
        public virtual DateTime CaptureUpdateDate { get; set; }

    }
}
