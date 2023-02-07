using Apeek.Common.Interfaces;
using Apeek.Entities.Entities;
using Apeek.NH.Repository.Common;
using System.Collections.Generic;

namespace Apeek.NH.Repository.Repositories
{
    public interface IRepositoryGoodCategory : IRepositoryAudit<GoodCategory>, IDependency
    {
        int GetGoodCategoryId(int goodId);
        string GetGoodCategorylist(int goodId);
    }
}
