using Apeek.Common.Interfaces;
using Momentarily.ViewModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.Core.Services
{
   public interface IUserLogService : IDependency
    {
        List<LogEntryViewModel> GetUserLogs(int userId);
        bool DeleteUserLogs(int userId);
    }
}
