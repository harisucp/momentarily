using System;
using Apeek.Common;
using Apeek.Common.Logger;
using Apeek.Common.UrlHelpers;
using Apeek.Core.Services.Impl;
using Apeek.Entities.Entities;
using Apeek.NH.Repository.Common;
using Apeek.NH.Repository.Repositories;
using Apeek.ViewModels.Models;
using Momentarily.Common.Definitions;
using Momentarily.ViewModels.Mappers;
using Momentarily.ViewModels.Models;
using Apeek.ViewModels.Mappers.Imp;
namespace Momentarily.UI.Service.Services.Impl
{
    public class MomentarilyUserMessageService : UserMessageService, IMomentarilyUserMessageService
    {
        public MomentarilyUserMessageService(IRepositoryUser repUser, IRepositoryUserMessage repUserMessage,
            RepositoryAudit<UserMessageDetail> repUserMessageDetail,IRepositoryMessageImages repositoryMessageImages)
            : base(repUser, repUserMessage, repUserMessageDetail, repositoryMessageImages)
        {
        }
        public bool SendSystemMessage(UserSystemMessageCreateModel messageModel)
        {
            try
            {
                if (messageModel.AuthorId == messageModel.ReceiverId)
                    return false;
                var systemMessage = GetSystemMessage(messageModel);
                if (!string.IsNullOrWhiteSpace(systemMessage))
                {
                    var userMessage = EntityMapper<IMomentarilyUserMessageMapper>.Mapper().Map(messageModel, new UserMessageCreateModel());
                    userMessage.Message = systemMessage;
                    if (!string.IsNullOrWhiteSpace(userMessage.Message))
                        userMessage.Message = userMessage.Message.Replace(MessageCharactersMap.Quote, "\\\"");
                    userMessage.IsSystem = true;
                    var messageAddResult = AddMessage(userMessage);
                    if (messageAddResult.CreateResult == CreateResult.Success)
                        return true;
                }
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.ImageProcessor, string.Format("Add system message fail. Ex: {0}.", ex));
            }
            return false;
        }
        public bool SendBookingGoodRequestMessage(int authorId, int receiverId, int requestId, QuickUrl quickUrl)
        {
            return SendSystemMessage(new UserSystemMessageCreateModel
            {
                AuthorId = authorId,
                ItemId = requestId,
                MessageType = SystemMessageType.BookingRequest,
                ReceiverId = receiverId,
                ItemUrl = quickUrl.GoodRequestUrl(requestId)
            });
        }
        public bool SendApproveGoodRequestMessage(int authorId, int receiverId, int requestId, QuickUrl quickUrl)
        {
            return SendSystemMessage(new UserSystemMessageCreateModel
            {
                AuthorId = authorId,
                ItemId = requestId,
                MessageType = SystemMessageType.ApproveRequest,
                ReceiverId = receiverId,
                ItemUrl = quickUrl.UserRequestUrl(requestId),
                PaymentUrl = quickUrl.RequestPaymentUrl(requestId),
                //DepositUrl = quickUrl.DepositPaymentAbsoluteUrl(requestId)
            });
        }
        public bool SendDeclineGoodRequestMessage(int authorId, int receiverId, int requestId, string message, QuickUrl quickUrl)
        {
            return SendSystemMessage(new UserSystemMessageCreateModel
            {
                AuthorId = authorId,
                ItemId = requestId,
                MessageType = SystemMessageType.DeclineRequest,
                ReceiverId = receiverId,
                ItemUrl = quickUrl.UserRequestUrl(requestId),
                Message = message
            });
        }
        public bool SendPayGoodRequestMessage(int authorId, int receiverId, int requestId, QuickUrl quickUrl)
        {
            return SendSystemMessage(new UserSystemMessageCreateModel
            {
                AuthorId = authorId,
                ItemId = requestId,
                MessageType = SystemMessageType.PayRequest,
                ReceiverId = receiverId,
                ItemUrl = quickUrl.GoodRequestUrl(requestId)
            });
        }
        public bool SendDepositGoodRequestMessage(int authorId, int receiverId, int requestId, QuickUrl quickUrl)
        {
            return SendSystemMessage(new UserSystemMessageCreateModel
            {
                AuthorId = authorId,
                ItemId = requestId,
                MessageType = SystemMessageType.DepositRequest,
                ReceiverId = receiverId,
                ItemUrl = quickUrl.UserRequestUrl(requestId)
            });
        }
        public bool SendSahrerStartDisputeMessage(int authorId, int receiverId, int requestId, QuickUrl quickUrl)
        {
            return SendSystemMessage(new UserSystemMessageCreateModel
            {
                AuthorId = authorId,
                ItemId = requestId,
                MessageType = SystemMessageType.SharerDispute,
                ReceiverId = receiverId,
                ItemUrl = quickUrl.UserRequestUrl(requestId)
            });
        }
        public bool SendSeekerStartDisputeMessage(int authorId, int receiverId, int requestId, QuickUrl quickUrl)
        {
            return SendSystemMessage(new UserSystemMessageCreateModel
            {
                AuthorId = authorId,
                ItemId = requestId,
                MessageType = SystemMessageType.SeekerDispute,
                ReceiverId = receiverId,
                ItemUrl = quickUrl.GoodRequestUrl(requestId)
            });
        }
        public bool SendReviewMessageForSeeker(int authorId, int receiverId, int requestId, QuickUrl quickUrl)
        {
            return SendSystemMessage(new UserSystemMessageCreateModel
            {
                AuthorId = authorId,
                ItemId = requestId,
                MessageType = SystemMessageType.SeekerNeedReview,
                ReceiverId = receiverId,
                ItemUrl = quickUrl.UserRequestUrl(requestId)
            });
        }
        public bool SendReviewMessageForSharer(int authorId, int receiverId, int requestId, QuickUrl quickUrl)
        {
            return SendSystemMessage(new UserSystemMessageCreateModel
            {
                AuthorId = authorId,
                ItemId = requestId,
                MessageType = SystemMessageType.SharerNeedReview,
                ReceiverId = receiverId,
                ItemUrl = quickUrl.GoodRequestUrl(requestId)
            });
        }
        private string GetSystemMessage(UserSystemMessageCreateModel messageModel)
        {
            switch (messageModel.MessageType)
            {
                case SystemMessageType.BookingRequest:
                    {
                        return string.Format(MomentarilySystemMessages.BookingRequest, messageModel.ItemId, messageModel.ItemUrl);
                    }
                case SystemMessageType.ApproveRequest:
                    {
                        return string.Format(MomentarilySystemMessages.ApproveRequest, messageModel.ItemId, messageModel.ItemUrl,
                            messageModel.PaymentUrl);
                    }
                case SystemMessageType.DeclineRequest:
                    {
                        return string.Format(MomentarilySystemMessages.DeclineRequest, messageModel.ItemId, messageModel.ItemUrl,
                            messageModel.Message);
                    }
                case SystemMessageType.PayRequest:
                    {
                        return string.Format(MomentarilySystemMessages.PayRequest, messageModel.ItemId, messageModel.ItemUrl);
                    }
                case SystemMessageType.DepositRequest:
                    {
                        return string.Format(MomentarilySystemMessages.DepositRequest, messageModel.ItemId, messageModel.ItemUrl);
                    }
                case SystemMessageType.SharerDispute:
                    {
                        return string.Format(MomentarilySystemMessages.SharerDispute, messageModel.ItemId, messageModel.ItemUrl);
                    }
                case SystemMessageType.SeekerDispute:
                    {
                        return string.Format(MomentarilySystemMessages.SeekerDispute, messageModel.ItemId, messageModel.ItemUrl);
                    }
                case SystemMessageType.SeekerNeedReview:
                    {
                        return string.Format(MomentarilySystemMessages.SeekerNeedReview, messageModel.ItemId, messageModel.ItemUrl);
                    }
                case SystemMessageType.SharerNeedReview:
                {
                    return string.Format(MomentarilySystemMessages.SeekerNeedReview, messageModel.ItemId, messageModel.ItemUrl);
                }
            }
            return null;
        }
   }
}
