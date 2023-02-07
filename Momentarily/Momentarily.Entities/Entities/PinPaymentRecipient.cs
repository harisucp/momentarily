using Apeek.Entities.Interfaces;
namespace Momentarily.Entities.Entities
{
    public class PinPaymentRecipient : AuditEntity
    {
        public override int Id { get; set; }
        public virtual string RecipientToken { get; set; }
        public static string _TableName = "p_pinpayment_recipient";
        public override string TableName
        {
            get { return _TableName; }
        }
    }
}