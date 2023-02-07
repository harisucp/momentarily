using Apeek.Entities.Interfaces;
namespace Apeek.Entities.Entities
{
    public class PaymentTransaction : AuditEntity
    {
        public static string _TableName = "c_payment_transaction";
        public override int Id { get; set; }
        public override string TableName { get { return _TableName; } }
        public virtual string TransactionId { get; set; }
        public virtual string PayerId { get; set; }
        public virtual string CapureId { get; set; }
        public virtual int GoodRequestId { get; set; }
        public virtual string Type { get; set; }
        public virtual double Cost { get; set; }
        public virtual double Commision { get; set; }
        public virtual int StatusId { get; set; }
        public virtual GoodRequest GoodRequest { get; set; }
    }
}
