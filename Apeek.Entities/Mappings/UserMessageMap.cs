using Apeek.Entities.Entities;
using Apeek.Entities.Interfaces;
namespace Apeek.Entities.Mappings
{
    public class UserMessageMap : AuditEntityMap<UserMessage>
    {
        public UserMessageMap()
        {
            Table("c_user_message");
            Id(x => x.Id, "id");
            Map(x => x.UserId, "user_id");
            Map(x => x.OpposedUserId, "opposed_user_id");
            HasMany(x => x.UserMessageDetails)
                .KeyColumn("user_message_id")
                .Cascade.All();
            References(x => x.User)
                .Column("user_id")
                .Not.Update()
                .Not.Insert();
            References(x => x.OpposedUser)
                .Column("opposed_user_id")
                .Not.Update()
                .Not.Insert();
        }
    }
}
