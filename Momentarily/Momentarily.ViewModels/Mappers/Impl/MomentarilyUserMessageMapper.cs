using Apeek.ViewModels.Models;
using Momentarily.ViewModels.Models;
namespace Momentarily.ViewModels.Mappers.Impl
{
    public class MomentarilyUserMessageMapper : IMomentarilyUserMessageMapper
    {
        public UserMessageCreateModel Map(UserSystemMessageCreateModel source, UserMessageCreateModel dest)
        {
            dest.AuthorId = source.AuthorId;
            dest.ReceiverId = source.ReceiverId;
            return dest;
        }
    }
}