using System.Collections.Generic;
using Apeek.Entities.Interfaces;
namespace Apeek.Entities.Entities
{
    public class UserMessage : AuditEntity
    {
        public UserMessage()
        {
            UserMessageDetails = new List<UserMessageDetail>();
        }
        public static string _TableName = "c_user_message";
        public override int Id { get; set; }
        public override string TableName { get { return _TableName; } }
        public virtual int UserId { get; set; }
        public virtual int OpposedUserId { get; set; }
        public virtual IEnumerable<UserMessageDetail> UserMessageDetails { get; set; }
        public virtual User User { get; set; }
        public virtual User OpposedUser { get; set; }
    }
}
