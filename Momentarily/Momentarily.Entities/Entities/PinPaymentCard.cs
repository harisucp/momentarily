using Apeek.Entities.Interfaces;
namespace Apeek.Entities.Entities
{
    public class PinPaymentCard : AuditEntity
    {
        public override int Id { get; set; }
        public static string _TableName = "c_user_card";
        public override string TableName
        {
            get { return _TableName; }
        }
        public virtual int UserId { get; set; }
        public virtual string CardToken { get; set; }
        public virtual string DisplayNumber { get; set; }
    }
}