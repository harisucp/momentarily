using System.Collections.Generic;
using System.Linq;
using Apeek.Common.Interfaces;
using Apeek.Entities.Entities;
using Apeek.NH.Repository.Common;
namespace Apeek.NH.Repository.Repositories
{
    public interface IRepositoryGoodPropertyValue : IRepositoryAudit<GoodPropertyValue>, IDependency
    {
        Dictionary<string, GoodPropertyValue> GetGoodProperties(int goodId);
        IQueryable<GoodPropertyValue> GetGoodPropertiesValue(int goodId);
    }
}