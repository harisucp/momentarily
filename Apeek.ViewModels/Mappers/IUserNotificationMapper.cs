using Apeek.Common.Interfaces;
using Apeek.Entities.Entities;
using Apeek.ViewModels.Models;
namespace Apeek.ViewModels.Mappers
{
    public interface IUserNotificationMapper : IDependency, IMapper<UserNotificationCreateModel, UserNotification>,
        IMapper<UserNotification, UserNotificationViewModel>
    {
    }
}