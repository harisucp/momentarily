using Apeek.Entities.Interfaces;
namespace Momentarily.Entities.Entities
{
    public class BraintreeRecipient : AuditEntity
    {
        public override int Id { get; set; }
        public virtual int UserId { get; set; }
        public virtual string MerchantAccountId { get; set; }
        public static string _TableName = "p_braintree_recipient";
        public override string TableName
        {
            get { return _TableName; }
        }
    }
}