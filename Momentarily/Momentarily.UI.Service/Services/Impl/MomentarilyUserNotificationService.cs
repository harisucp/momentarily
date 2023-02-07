using Apeek.Core.Services.Impl;
using Apeek.NH.Repository.Repositories;
namespace Momentarily.UI.Service.Services.Impl
{
    public class MomentarilyUserNotificationService : UserNotificationService, IMomentarilyUserNotificationService
    {
        public MomentarilyUserNotificationService(IRepositoryUserNotification repUserNotification)
            : base(repUserNotification)
        {
        }
    }
}
