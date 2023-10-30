using System.Collections.Generic;
using Apeek.Common.Interfaces;
using Apeek.Common.Models;
using Apeek.Common.UrlHelpers;
using Apeek.Entities.Entities;
using Apeek.ViewModels.Models;
namespace Apeek.Core.Services
{
    public interface IUserNotificationService : IDependency
    {
        Result<UserNotificationViewModel> AddNotification(UserNotificationCreateModel notificationModel);
        Result<IEnumerable<UserNotificationViewModel>> GetNotifications(int userId);
        bool SetIsViewedNotification(int userId, int notificationId);
        Result<UserNotificationViewModel> AddReceiveMessageNotification(int userId, int authorId, QuickUrl quickUrl);
        bool SetIsViewedForAllNotification(int userId);
        Result<List<UserNotification>> GetNotificationsList(int userId);

    }
}
