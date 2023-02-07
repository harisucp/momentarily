using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apeek.Entities.Entities;
using Apeek.NH.Repository.Common;
namespace Apeek.NH.Repository.Repositories.Impl
{
    public class RepositoryGoodPropertyValueDefinition :RepositoryAudit<GoodPropertyValueDefinition>, IRepositoryGoodPropertyValueDefinition
    {
        public IList<GoodPropertyValueDefinition> GetValuesByPropertyName(string name)
        {
            return (from vd in Table
                join p in TableFor<GoodProperty>() on vd.GoodPropertyId equals p.Id
                where p.Name == name
                select vd).ToList();
        }
    }
}
