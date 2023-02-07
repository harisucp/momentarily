using System;
using System.Collections.Generic;
using System.Linq;
using Apeek.Entities.Entities;
using Apeek.NH.Repository.Common;
namespace Apeek.NH.Repository.Repositories.Impl
{
    public class RepositoryGoodPropertyValue : RepositoryAudit<GoodPropertyValue>, IRepositoryGoodPropertyValue
    {
        public Dictionary<string, GoodPropertyValue> GetGoodProperties(int goodId)
        {
            var properties = (from pv in Table
                join p in TableFor<GoodProperty>() on pv.GoodPropertyId equals p.Id
                where pv.GoodId == goodId
                select new
                {
                    Key = p.Name,
                    Value = pv
                }).ToDictionary(p => p.Key, p => p.Value);
            return properties;
        }
        public IQueryable<GoodPropertyValue> GetGoodPropertiesValue(int goodId)
        {
            return Table.Where(p => p.GoodId == goodId);
        }
    }
}