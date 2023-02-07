using Apeek.Entities.Entities;
using Apeek.Entities.Interfaces;
namespace Apeek.Entities.Mappings
{
    public class UserNotificationMap : AuditEntityMap<UserNotification>
    {
        public UserNotificationMap()
        {
            Table("c_user_notification");
            Id(x => x.Id, "id");
            Map(x => x.UserId, "user_id");
            Map(x => x.Text, "text");
            Map(x => x.Url, "url");
            Map(x => x.IsViewed, "is_viewed");
        }
    }
}
