using Apeek.Common.Interfaces;
using Apeek.Entities.Entities;
using Apeek.NH.Repository.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.NH.Repository.Repositories
{
   public interface IRepositoryChatTopics : IRepository<ChatTopics>, IDependency
    {
    }
}
