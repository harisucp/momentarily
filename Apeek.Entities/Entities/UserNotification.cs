using Apeek.Entities.Interfaces;
namespace Apeek.Entities.Entities
{
    public class UserNotification : AuditEntity
    {
        public static string _TableName = "c_user_notification";
        public override int Id { get; set; }
        public override string TableName { get { return _TableName; } }
        public virtual int UserId { get; set; }
        public virtual string Text { get; set; }
        public virtual string Url { get; set; }
        public virtual bool IsViewed { get; set; }
    }
}
