using FluentNHibernate.Mapping;
using Apeek.Entities.Entities;
using Apeek.Entities.Interfaces;

namespace Apeek.Entities.Mappings
{
   public class PaypalPaymentMap : AuditEntityMap<PaypalPayment>
    {
        public PaypalPaymentMap()
        {
            Table("p_paypal_payment_detail");
            Id(x => x.Id, "id");
            Map(x => x.GoodRequestId, "good_request_id");
            Map(x => x.PayerId, "payer_user_id");
            Map(x => x.PayeeId, "payee_user_id");
            Map(x => x.PaymentCart, "payment_cart").Nullable();
            Map(x => x.PaymentCreatedDate, "payment_created_date");
            Map(x => x.PaymentPayId, "payment_payid");
            Map(x => x.PaymentIntent, "payment_intent");
            Map(x => x.PaymentStatus, "payment_status");
            Map(x => x.PaymentUpdateDate, "payment_update_date");
            Map(x => x.PaymentInvoiceNumber, "payment_invoice_number");
            Map(x => x.AuthorizationCreatedDate, "authorization_create_date");
            Map(x => x.AuthorizationId, "authorization_id");
            Map(x => x.AuthorizationCount, "authorization_count");
            Map(x => x.AuthorizationStatus, "authorization_status");
            Map(x => x.AuthorizationUpdateDate, "authorization_update_date");
            Map(x => x.AuthorizationValidUntill, "authorization_valid_untill");
            Map(x => x.CaptureCreatedDate, "capture_create_date");
            Map(x => x.CaptureId, "capture_id");
            Map(x => x.CaptureIsFinal, "capture_is_final");
            Map(x => x.CaptureStatus, "capture_status");
            Map(x => x.CaptureUpdateDate, "capture_update_date");
          
        }
    }
}
