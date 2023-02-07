using System.Collections.Generic;
using System.Linq;
using Apeek.Entities.Entities;
using Apeek.NH.Repository.Common;
namespace Apeek.NH.Repository.Repositories.Impl
{
    public class RepositoryUserMessage : RepositoryAudit<UserMessage>, IRepositoryUserMessage
    {
        public IEnumerable<UserMessage> GetMessageHeaders(int receiverId, int authorId)
        {
            return Table.Where(p => (p.UserId == receiverId && p.OpposedUserId == authorId)
                || (p.UserId == authorId && p.OpposedUserId == receiverId));
        }
        public IEnumerable<UserMessageDetail> GetLastMessages(int receiverId)
        {
            return Session.CreateSQLQuery(@"select umd.* 
                        from c_user_message_detail as umd 
                        join c_user_message as um on umd.user_message_id = um.id 
                        where um.user_id = :userId 
                        and umd.id = (SELECT TOP(1) umd1.id from c_user_message_detail as umd1 where umd1.user_message_id=umd.user_message_id and (umd1.is_system=0 or (umd1.is_system=1 and umd1.author_id<>:userId)) order by umd1.create_date desc) 
                        order by umd.create_date desc")
                   .AddEntity(typeof(UserMessageDetail))
                   .SetParameter("userId", receiverId)
                   .List<UserMessageDetail>();
        }
        public IEnumerable<UserMessageDetail> GetCorrespondence(int authorId, int receiverId)
        {
            return TableFor<UserMessageDetail>()
                   .Where(p => ((p.AuthorId == authorId
                   && p.ReceiverId == receiverId)
                   || (p.AuthorId == receiverId
                   && p.ReceiverId == authorId)) && p.UserMessage.UserId == authorId).OrderBy(o => o.CreateDate);
        }
        public IEnumerable<UserMessageDetail> GetUnReadMessages(int authorId, int receiverId)
        {
            return TableFor<UserMessageDetail>()
                .Where(p => p.AuthorId == authorId && p.ReceiverId == receiverId &&
                            p.UserMessage.UserId == receiverId && p.IsRead == false);
        }
    }
}
