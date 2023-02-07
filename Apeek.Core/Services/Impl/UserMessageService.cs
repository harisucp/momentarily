using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Apeek.Common;
using Apeek.Common.Extensions;
using Apeek.Common.Logger;
using Apeek.Common.Models;
using Apeek.Entities.Entities;
using Apeek.NH.DataAccessLayer.DataAccess;
using Apeek.NH.Repository.Common;
using Apeek.NH.Repository.Repositories;
using Apeek.ViewModels.Models;
namespace Apeek.Core.Services.Impl
{
    public class UserMessageService : IUserMessageService
    {
        private readonly IRepositoryUser _repUser;
        private readonly IRepositoryUserMessage _repUserMessage;
        private readonly RepositoryAudit<UserMessageDetail> _repUserMessageDetail;
        private readonly IRepositoryMessageImages _repMessageImages;
        public UserMessageService(IRepositoryUser repUser, IRepositoryUserMessage repUserMessage, 
            RepositoryAudit<UserMessageDetail> repUserMessageDetail, IRepositoryMessageImages repMessageImages)
        {
            _repUser = repUser;
            _repUserMessage = repUserMessage;
            _repUserMessageDetail = repUserMessageDetail;
            _repMessageImages = repMessageImages;
        }
        public Result<UserMessageViewModel> AddMessage(UserMessageCreateModel messageModel)
        {
            var result = new Result<UserMessageViewModel>(CreateResult.Error, new UserMessageViewModel());
            try
            {
                Uow.Wrap(u =>
                {
                    var messageHeaders = _repUserMessage.GetMessageHeaders(messageModel.ReceiverId, messageModel.AuthorId).ToList();
                    if (messageHeaders.Any())
                    {
                        var messagesDetails = new List<UserMessageDetail>();
                        var authorHeader = messageHeaders.FirstOrDefault(p => p.UserId == messageModel.AuthorId);
                        if (authorHeader != null)
                            messagesDetails.Add(CreateUserMessageDetail(authorHeader.Id,
                                messageModel.AuthorId, messageModel.ReceiverId, messageModel.Message, true, messageModel.IsSystem));
                        var receiverHeader = messageHeaders.FirstOrDefault(p => p.UserId != messageModel.AuthorId);
                        if (receiverHeader != null)
                            messagesDetails.Add(CreateUserMessageDetail(receiverHeader.Id,
                                messageModel.AuthorId, messageModel.ReceiverId, messageModel.Message, false, messageModel.IsSystem));
                        _repUserMessageDetail.SaveOrUpdateAudit(messagesDetails, messageModel.ReceiverId);
                    }
                    else
                    {
                        var messages = new List<UserMessage>
                        {
                            CreateUserMessageForAuthor(messageModel.AuthorId, messageModel.ReceiverId, messageModel.Message, true, messageModel.IsSystem),
                            CreateUserMessageForReceiver(messageModel.AuthorId, messageModel.ReceiverId, messageModel.Message, false, messageModel.IsSystem)
                        };
                        _repUserMessage.SaveOrUpdateAudit(messages, messageModel.ReceiverId);
                    }
                    result.CreateResult = CreateResult.Success;
                },
                    null,
                    LogSource.UserMessageService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.ImageProcessor, string.Format("Add message fail. Ex: {0}.", ex));
            }
            return result;
        }
        public Result<IEnumerable<UserMessageViewModel>> GetLastMessages(int receiverId)
        {
            var result = new Result<IEnumerable<UserMessageViewModel>>(CreateResult.Error, new List<UserMessageViewModel>());
            try
            {
                Uow.Wrap(u =>
                {
                    result.Obj = _repUserMessage.GetLastMessages(receiverId).Select(p => new UserMessageViewModel
                    {
                        Id = p.Id,
                        AuthorId = p.AuthorId != receiverId ? p.AuthorId : p.ReceiverId,
                        AuthorUserName = p.UserMessage.OpposedUser.FirstName,
                        AuthorProfilePictureUrl = p.UserMessage.OpposedUser.UserImages.MainMessageImageUrlThumb(),
                        Message = p.Message,
                        DateCreated = p.CreateDate,
                        IsRead = p.IsRead,
                        IsSystem = p.IsSystem
                    }).ToList();
                    result.CreateResult = CreateResult.Success;
                }, null, LogSource.UserMessageService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.UserMessageService, string.Format("Get last messages fail. Ex: {0}.", ex));
            }
            return result;
        }
        public Result<IEnumerable<UserMessageViewModel>> GetCorrespondence(int authorId, int receiverId, IUnitOfWork unitOfWork = null)
        {
            var result = new Result<IEnumerable<UserMessageViewModel>>(CreateResult.Error, new List<UserMessageViewModel>());
            try
            {
                Uow.Wrap(u =>
                {
                    result.Obj = _repUserMessage.GetCorrespondence(authorId, receiverId).Select(p => new UserMessageViewModel
                    {
                        Id = p.Id,
                        AuthorId = p.AuthorId,
                        AuthorUserName = p.AuthorId != receiverId ?
                            p.UserMessage.User.FirstName : p.UserMessage.OpposedUser.FirstName,
                        AuthorProfilePictureUrl = p.AuthorId != receiverId ?
                            p.UserMessage.User.UserImages.MainImageUrlThumb() : p.UserMessage.OpposedUser.UserImages.MainImageUrlThumb(),
                        Message = p.Message,
                        DateCreated = p.CreateDate,
                        IsRead = p.IsRead,
                        IsSystem = p.IsSystem,
                        PathList = (from path in _repMessageImages.Table where path.MessageDetailId == p.Id select path.ImagePath).ToList()
                    }).ToList();
                    result.CreateResult = CreateResult.Success;
                }, unitOfWork, LogSource.UserMessageService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.UserMessageService, string.Format("Get correspondence fail. Ex: {0}.", ex));
            }
            return result;
        }
        public bool SetIsRead(int authorId, int receiverId)
        {
            try
            {
                Uow.Wrap(u =>
                {
                    var unReadMessages = _repUserMessage.GetUnReadMessages(authorId, receiverId).ToList();
                    if (unReadMessages.Any())
                    {
                        unReadMessages.ForEach(p => p.IsRead = true);
                        _repUserMessageDetail.SaveOrUpdateAudit(unReadMessages, receiverId);
                    }
                }, null, LogSource.UserMessageService);
                return true;
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.UserMessageService, string.Format("Set message read fail. Ex: {0}.", ex));
            }
            return false;
        }
        private UserMessage CreateUserMessageForAuthor(int authorId, int receiverId, string message,
            bool isRead, bool isSystem)
        {
            return new UserMessage
            {
                UserId = authorId,
                OpposedUserId = receiverId,
                UserMessageDetails = CreateUserMessageDetail(authorId, receiverId, message, isRead, isSystem)
            };
        }
        private UserMessage CreateUserMessageForReceiver(int authorId, int receiverId, string message,
            bool isRead, bool isSystem)
        {
            return new UserMessage
            {
                UserId = receiverId,
                OpposedUserId = authorId,
                UserMessageDetails = CreateUserMessageDetail(authorId, receiverId, message, isRead, isSystem)
            };
        }
        private UserMessageDetail CreateUserMessageDetail(int userMessageId, int authorId, int receiverId, string message, bool isRead, bool isSystem)
        {
            return new UserMessageDetail
            {
                UserMessageId = userMessageId,
                ReceiverId = receiverId,
                AuthorId = authorId,
                IsRead = isRead,
                Message = message,
                IsSystem = isSystem
            };
        }
        private IEnumerable<UserMessageDetail> CreateUserMessageDetail(int authorId, int receiverId, string message, bool isRead, bool isSystem)
        {
            return new List<UserMessageDetail>
            {
                new UserMessageDetail
                {
                    ReceiverId = receiverId,
                    AuthorId = authorId,
                    IsRead = isRead,
                    Message = message,
                    CreateDate = DateTime.Now,
                    ModDate = DateTime.Now,
                    CreateBy = receiverId,
                    ModBy = receiverId,
                    IsSystem = isSystem
                }
            };
        }
        public Result<UserConversationViewModel> GetConversation(int authorId, int receiverId)
        {
            var result = new Result<UserConversationViewModel>(CreateResult.Error, new UserConversationViewModel());
            try
            {
                Uow.Wrap(uow =>
                {
                    var domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);                    var fullpath = domainName + "/Content/MessageImages";

                    var author = _repUser.GetUser(authorId);
                    var receiver = _repUser.GetUser(receiverId);
                    result.Obj = new UserConversationViewModel()
                    {
                        AuthorId = authorId,
                        ReceiverId = receiverId,
                        AuthorUserName = author.FirstName,
                        AuthorImageUrl = author.UserImages.MainImageUrlThumb(),
                        ReceiverUserName = receiver.FirstName,
                        ReceiverImageUrl = receiver.UserImages.MainImageUrlThumb(),
                    };
                    var userCorrespondence = GetCorrespondence(authorId, receiverId, uow);
                    if (userCorrespondence.CreateResult == CreateResult.Success)
                    {
                        if (userCorrespondence.Obj.Any())
                        {
                            //select messages without my system
                            var correspondence = userCorrespondence.Obj
                                .Where(m => m.IsSystem == false || (m.IsSystem == true && m.AuthorId != authorId))
                                .OrderBy(x => x.DateCreated).ToList();
                            result.Obj.Messages = correspondence;
                        }
                    }
                    result.Obj.messageUrlPath = fullpath;
                    result.CreateResult = CreateResult.Success;
                }, null, LogSource.UserMessageService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.UserMessageService, string.Format("Get conversation fail. Ex: {0}.", ex));
            }
            return result;
        }
        public int GetUnreadMessageCount(int userId)
        {
            var result = 0;
            try
            {
                Uow.Wrap(uow =>
                {
                    result = _repUserMessageDetail.Table.Where(md => md.IsRead == false && md.ReceiverId == userId)
                        .Select(md => md.Id)
                        .Distinct()
                        .Count();
                }, null, LogSource.UserMessageService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.UserMessageService, string.Format("Get unread message count fail. Ex: {0}.", ex));
            }
            return result;
        }

        public bool SaveMessageImagesPath(List<string> imagePath)
        {
            var result = false;
            try
            {
                Uow.Wrap(uow =>
                {
                    var lastTwoMessageDetailIds = _repUserMessageDetail.Table.OrderByDescending(x=>x.Id).Select(x=>x.Id).Take(2);
                    foreach (var id in lastTwoMessageDetailIds)
                    {
                        foreach(var path in imagePath)
                        {
                            MessageImages entity = new MessageImages();
                            entity.MessageDetailId = id;
                            entity.ImagePath = path;
                            entity.CreateDate = DateTime.Now;
                            entity.ModDate = DateTime.Now;
                            entity.CreateBy = 0;
                            entity.ModBy = 0;
                            _repMessageImages.Save(entity);
                        }
                    }
                }, null, LogSource.UserMessageService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.UserMessageService, string.Format("insert message images fail. Ex: {0}.", ex));
            }

            return result;
        }

    }
}