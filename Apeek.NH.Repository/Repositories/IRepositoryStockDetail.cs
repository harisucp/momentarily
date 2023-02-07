using Apeek.NH.Repository.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apeek.Entities.Entities;
using Apeek.Common.Interfaces;

namespace Apeek.NH.Repository.Repositories
{
   public interface IRepositoryStockDetail : IRepository<StockDetail>, IDependency
    {
    }
}
