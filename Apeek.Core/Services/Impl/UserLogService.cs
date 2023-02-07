using Amazon.IdentityManagement.Model;
using Apeek.Common.Logger;
using Apeek.NH.DataAccessLayer.DataAccess;
using Apeek.NH.Repository.Repositories;
using Momentarily.ViewModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.Core.Services.Impl
{
  public  class UserLogService:IUserLogService
    {
        protected readonly IRepositoryLogEntry _repLogEntry;
        public UserLogService(IRepositoryLogEntry repLogEntry)
        {
            _repLogEntry = repLogEntry;
        }

        public bool DeleteUserLogs(int userId)
        {
            var result= false;
            Uow.Wrap(u =>
            {
                var list = (from p in _repLogEntry.Table
                            where p.UserId == userId
                            select p).ToList();
                if (list != null && list.Count > 0)
                {
                    _repLogEntry.Delete(list);
                }   
                result = true;

            }, null, LogSource.Logger);
            return result;
        }

        public List<LogEntryViewModel> GetUserLogs(int userId)
        {
           List<LogEntryViewModel> logs = null;
            Uow.Wrap(u =>
            {
                logs = new List<LogEntryViewModel>();
                logs = (from p in _repLogEntry.Table
                        where p.UserId == userId
                        select new LogEntryViewModel
                        {
                            Id = p.Id,
                            Severity = p.Severity,
                            CreateDate = p.CreateDate,
                            ApplicationName = p.ApplicationName,
                            UserId = p.UserId,
                            SessionId = p.SessionId,
                            IpAddress = p.IpAddress,
                            SourceName = p.SourceName,
                            Message = p.Message,
                            AppVersion = p.AppVersion,
                        }).ToList();
            }, null, LogSource.Logger);
            return logs;
        }
    }
}
