using System;
using System.Collections.Generic;
using System.Linq;
using Apeek.Common;
using Apeek.Common.Logger;
using Apeek.Common.Models;
using Apeek.Common.UrlHelpers;
using Apeek.Entities.Entities;
using Apeek.NH.DataAccessLayer.DataAccess;
using Apeek.NH.Repository.Repositories;
using Apeek.ViewModels.Mappers;
using Apeek.ViewModels.Mappers.Imp;
using Apeek.ViewModels.Models;
namespace Apeek.Core.Services.Impl
{
    public class UserNotificationService : IUserNotificationService
    {
        private readonly IRepositoryUserNotification _repUserNotification;
        public UserNotificationService(IRepositoryUserNotification repUserNotification)
        {
            _repUserNotification = repUserNotification;
        }
        public Result<UserNotificationViewModel> AddNotification(UserNotificationCreateModel notificationModel)
        {
            var result = new Result<UserNotificationViewModel>(CreateResult.Error, new UserNotificationViewModel());
            try
            {
                Uow.Wrap(u =>
                {
                    var userNotification =EntityMapper<IUserNotificationMapper>.Mapper().Map(notificationModel, new UserNotification());
                    _repUserNotification.SaveOrUpdateAudit(userNotification, notificationModel.UserId);
                    result.CreateResult = CreateResult.Success;
                },
                null,
                LogSource.UserNotificationService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.UserNotificationService, string.Format("Add notification fail. Ex: {0}.", ex));
            }
            return result;
        }
        public Result<IEnumerable<UserNotificationViewModel>> GetNotifications(int userId)
        {
            var result = new Result<IEnumerable<UserNotificationViewModel>>(CreateResult.Error, new List<UserNotificationViewModel>());
            try
            {
                Uow.Wrap(u =>
                {
                    result.Obj = _repUserNotification.Table.Where(p => p.UserId == userId && p.IsViewed != true)
                    .OrderByDescending(p => p.CreateDate)
                    .Select(p => EntityMapper<IUserNotificationMapper>.Mapper().Map(p, new UserNotificationViewModel()))
                    .ToList();
                    result.CreateResult = CreateResult.Success;
                },
                null,
                LogSource.UserNotificationService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.UserNotificationService, string.Format("Get notifications fail. Ex: {0}.", ex));
            }
            return result;
        }
        public bool SetIsViewedNotification(int userId, int notificationId)
        {
            var result = false;
            try
            {
                Uow.Wrap(u =>
                {
                    var userNotificaton = _repUserNotification.Get(notificationId);
                    if (userNotificaton != null && userNotificaton.UserId == userId)
                    {
                        userNotificaton.IsViewed = true;
                        _repUserNotification.SaveOrUpdateAudit(userNotificaton, userId);
                        result = true;
                    }
                },
                null,
                LogSource.UserNotificationService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.UserNotificationService, string.Format("Set is viewed notification fail. Ex: {0}.", ex));
            }
            return result;
        }
        public Result<UserNotificationViewModel> AddReceiveMessageNotification(int userId, int authorId, QuickUrl quickUrl)
        {
            return AddNotification(new UserNotificationCreateModel
            {
                UserId = userId,
                Text = NotificationTemplates.ReceiveMessageNotification,
                Url = quickUrl.UserMessageConversationUrl(authorId)
            });
        }

        public bool SetIsViewedForAllNotification(int userId)
        {
            var result = false;
            try
            {
                Uow.Wrap(u =>
                {
                    var notifications = _repUserNotification.Table.Where(x => x.UserId == userId && x.IsViewed == false).ToList();
                    if(notifications.Count() > 0)
                    {
                        foreach (var item in notifications)
                        {
                            if (item != null)
                            {
                                item.IsViewed = true;
                                _repUserNotification.SaveOrUpdateAudit(item, userId);
                                result = true;
                            }
                        }
                    }
                },
                null,
                LogSource.UserNotificationService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.UserNotificationService, string.Format("Set is viewed notification fail. Ex: {0}.", ex));
            }
            return result;
        }
        public Result<List<UserNotification>> GetNotificationsList(int userId)
        {
            var result = new Result<List<UserNotification>>(CreateResult.Error, new List<UserNotification>());
            try
            {
                Uow.Wrap(u =>
                {
                    result.Obj = _repUserNotification.Table.Where(p => p.UserId == userId && p.IsViewed != true)
                    .OrderByDescending(p => p.CreateDate)
                    .ToList();
                    result.CreateResult = CreateResult.Success;
                },
                null,
                LogSource.UserNotificationService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.UserNotificationService, string.Format("Get notifications fail. Ex: {0}.", ex));
            }
            return result;
        }

        public Result<List<UserNotification>> GetReadNotifications(int userId)
        {
            var result = new Result<List<UserNotification>>(CreateResult.Error, new List<UserNotification>());
            try
            {
                Uow.Wrap(u =>
                {
                    result.Obj = _repUserNotification.Table.Where(p => p.UserId == userId && p.IsViewed == true)
                    .OrderByDescending(p => p.CreateDate)
                    .ToList();
                    result.CreateResult = CreateResult.Success;
                },
                null,
                LogSource.UserNotificationService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.UserNotificationService, string.Format("Get notifications fail. Ex: {0}.", ex));
            }
            return result;
        }
    }
}
