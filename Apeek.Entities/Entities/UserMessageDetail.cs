using Apeek.Entities.Interfaces;
namespace Apeek.Entities.Entities
{
    public class UserMessageDetail : AuditEntity
    {
        public static string _TableName = "c_user_message_detail";
        public override int Id { get; set; }
        public override string TableName { get { return _TableName; } }
        public virtual int UserMessageId { get; set; }
        public virtual int ReceiverId { get; set; }
        public virtual int AuthorId { get; set; }
        public virtual string Message { get; set; }
        public virtual bool IsRead { get; set; }
        public virtual bool IsSystem { get; set; }
        public virtual UserMessage UserMessage { get; set; }
    }
}
