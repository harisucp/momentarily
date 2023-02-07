using Apeek.Entities.Entities;
using Apeek.Entities.Interfaces;
namespace Apeek.Entities.Mappings
{
    public class UserMessageDetailMap : AuditEntityMap<UserMessageDetail>
    {
        public UserMessageDetailMap()
        {
            Table("c_user_message_detail");
            Id(x => x.Id, "id");
            Map(x => x.UserMessageId, "user_message_id");
            Map(x => x.ReceiverId, "receiver_id");
            Map(x => x.AuthorId, "author_id");
            Map(x => x.Message, "message");
            Map(x => x.IsRead, "is_read");
            Map(x => x.IsSystem, "is_system");
            References(x => x.UserMessage)
                .Column("user_message_id")
                .Not.Update()
                .Not.Insert();
        }
    }
}
