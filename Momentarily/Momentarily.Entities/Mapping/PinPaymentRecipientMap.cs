using Apeek.Entities.Interfaces;
namespace Momentarily.Entities.Entities
{
    public class PinPaymentRecipientMap : AuditEntityMap<PinPaymentRecipient>
    {
        public PinPaymentRecipientMap()
        {
            Table("p_pinpayment_recipient");
            Id(x => x.Id, "user_id").GeneratedBy.Assigned();
            Map(x => x.RecipientToken, "recipient_token");
        }
    }
}