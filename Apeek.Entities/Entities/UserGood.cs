using Apeek.Entities.Interfaces;
namespace Apeek.Entities.Entities
{
    public class UserGood : AuditEntity
    {
        public static string _TableName = "c_user_good";
        public override int Id { get; set; }
        public override string TableName { get { return _TableName; } }
        public virtual int UserId { get; set; }
        public virtual int GoodId { get; set; }
        public virtual User User { get; set; }
    }
}
