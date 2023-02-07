using Apeek.Common.Interfaces;
using Apeek.Common.Models;
using Apeek.Entities.Entities;
namespace Apeek.Core.Services
{
    public interface IGoodService<T, U> : IDependency where T : Good where U : class
    {
        Result<T> GetMyGood(int userId, int goodId);
        Result<T> SaveGood(T good, int userId);
        bool DeleteGood(int goodId);
        bool DeleteGood(T good);
        

    }
}