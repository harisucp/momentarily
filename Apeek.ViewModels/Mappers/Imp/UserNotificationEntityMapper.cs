using Apeek.Entities.Entities;
using Apeek.ViewModels.Models;
namespace Apeek.ViewModels.Mappers.Imp
{
    public class UserNotificationMapper : IUserNotificationMapper
    {
        public UserNotification Map(UserNotificationCreateModel source, UserNotification dest)
        {
            dest.UserId = source.UserId;
            dest.Text = source.Text;
            dest.Url = source.Url;
            return dest;
        }
        public UserNotificationViewModel Map(UserNotification source, UserNotificationViewModel dest)
        {
            dest.Text = source.Text;
            dest.Url = source.Url;
            return dest;
        }
    }
}
