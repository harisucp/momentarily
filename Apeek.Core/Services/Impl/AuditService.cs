using System.Collections.Generic;
using Apeek.Common.Interfaces;
using Apeek.Entities.Entities;
using Apeek.NH.DataAccessLayer.DataAccess;
using Apeek.NH.Repository.Common;
namespace Apeek.Core.Services.Impl
{
    public interface IAuditService : IDependency
    {
        void AddAudit(int auditTable, int auditField, int primaryKeyId, string oldValue, string newValue, string affectedUsers);
        void SaveAudit(int userId);
    }
    public class AuditService : IAuditService
    {
        private readonly IRepositoryAudit<Audit> _repAudit;
        private List<Audit> _audit;
        public AuditService(IRepositoryAudit<Audit> repAudit)
        {
            _repAudit = repAudit;
            _audit = new List<Audit>();
        }
        public void AddAudit(int auditTable, int auditAction, int primaryKeyId, string oldValue, string newValue, string affectedUsers)
        {
            if (string.Compare(oldValue, newValue) == 0)
                return;
            _audit.Add(new Audit()
            {
                AuditTable = auditTable,
                AuditAction = auditAction,
                PrimaryKeyId = primaryKeyId,
                NewValue = newValue,
                OldValue = oldValue,
                AffectedUsers = affectedUsers
            });
        }
        public void SaveAudit(int userId)
        {
            SaveAudit(_audit, userId);
        }
        private void SaveAudit(List<Audit> audit, int userId)
        {
            //Task.Run(() =>
            //{
                Uow.Wrap(u =>
                {
                    audit.ForEach(a => SaveAudit(a, userId));
                });
            //});
        }
        private void SaveAudit(Audit audit, int userId)
        {
            _repAudit.SaveOrUpdateAudit(audit, userId);
        }
    }
}