using System.Collections.Generic;
using Apeek.Common.Interfaces;
using Apeek.Entities.Entities;
using Apeek.NH.Repository.Common;
namespace Apeek.NH.Repository.Repositories
{
    public interface IRepositoryGoodPropertyValueDefinition : IRepositoryAudit<GoodPropertyValueDefinition>, IDependency
    {
        IList<GoodPropertyValueDefinition> GetValuesByPropertyName(string name);
    }
}