using Apeek.Common.Interfaces;
using Apeek.ViewModels.Mappers;
using Apeek.ViewModels.Models;
using Momentarily.ViewModels.Models;
namespace Momentarily.ViewModels.Mappers
{
    public interface IMomentarilyUserMessageMapper : IDependency,
        IMapper<UserSystemMessageCreateModel, UserMessageCreateModel>
    {
    }
}
