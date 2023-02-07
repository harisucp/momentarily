using Apeek.Common.Interfaces;
using Apeek.Entities.Entities;
using Apeek.ViewModels.Models.Impl;
namespace Apeek.ViewModels.Mappers
{
    public interface IUserMapper : IDependency, 
        IMapper<User, UserUpdateModel>, 
        IMapper<UserUpdateModel, User>, 
        IMapper<User, UserViewModel>
    {
    }
}