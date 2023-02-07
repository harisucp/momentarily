using System.Collections.Generic;
using Apeek.Common.Interfaces;
using Apeek.Common.Models;
using Apeek.NH.DataAccessLayer.DataAccess;
using Apeek.ViewModels.Models;
namespace Apeek.Core.Services
{
    public interface IUserMessageService : IDependency
    {
        Result<UserMessageViewModel> AddMessage(UserMessageCreateModel messageModel);
        Result<IEnumerable<UserMessageViewModel>> GetLastMessages(int receiverId);
        Result<IEnumerable<UserMessageViewModel>> GetCorrespondence(int authorId, int receiverId, IUnitOfWork unitOfWork = null);
        Result<UserConversationViewModel> GetConversation(int authorId, int receiverId);
        bool SetIsRead(int authorId, int receiverId);
        int GetUnreadMessageCount(int userId);
        bool SaveMessageImagesPath(List<string> imagePath);

    }
}