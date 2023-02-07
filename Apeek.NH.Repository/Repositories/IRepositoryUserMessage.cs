using System.Collections.Generic;
using Apeek.Common.Interfaces;
using Apeek.Entities.Entities;
using Apeek.NH.Repository.Common;
namespace Apeek.NH.Repository.Repositories
{
    public interface IRepositoryUserMessage : IRepositoryAudit<UserMessage>, IDependency
    {
        IEnumerable<UserMessage> GetMessageHeaders(int receiverId, int authorId);
        IEnumerable<UserMessageDetail> GetLastMessages(int receiverId);
        IEnumerable<UserMessageDetail> GetCorrespondence(int authorId, int receiverId);
        IEnumerable<UserMessageDetail> GetUnReadMessages(int authorId, int receiverId);
    }
}
