using Apeek.Entities.Interfaces;
namespace Momentarily.Entities.Entities
{
    public class PinPaymentCustomer : AuditEntity
    {
        public override int Id { get; set; }
        public virtual string CustomerToken { get; set; }
        public static string _TableName = "p_pinpayment_customer";
        public override string TableName
        {
            get { return _TableName; }
        }
    }
}