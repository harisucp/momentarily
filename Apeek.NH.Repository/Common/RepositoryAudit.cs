using System.Collections.Generic;
using Apeek.Entities.Interfaces;
namespace Apeek.NH.Repository.Common
{
    public interface IRepositoryAudit<T> : IRepository<T> where T : AuditEntity
    {
        IList<T> SaveOrUpdateAudit(IList<T> entities, int userId);
        T SaveOrUpdateAudit(T entity, int userId);
        IList<T> SaveAudit(IList<T> entities, int userId);
        T SaveAudit(T entity, int userId);

        IList<T> UpdateAudit(IList<T> entities, int userId);
        T UpdateAudit(T entity, int userId);
    }
    public class RepositoryAudit<T> : Repository<T>, IRepositoryAudit<T> where T : AuditEntity
    {
        public virtual IList<T> SaveOrUpdateAudit(IList<T> entities, int userId)
        {
            foreach (var entity in entities)
            {
                SaveOrUpdateAudit(entity, userId);
            }
            return entities;
        }
        public virtual T SaveOrUpdateAudit(T entity, int userId)
        {
            entity.UpdateAuditData(userId);
            Session.SaveOrUpdate(entity);
            return entity;
        }
        public virtual IList<T> SaveAudit(IList<T> entities, int userId)
        {
            foreach (var entity in entities)
            {
                SaveAudit(entity, userId);
            }
            return entities;
        }
        public virtual T SaveAudit(T entity, int userId)
        {
            entity.UpdateAuditData(userId);
            Session.Save(entity);
            return entity;
        }
        public virtual IList<T> UpdateAudit(IList<T> entities, int userId)
        {
            foreach (var entity in entities)
            {
                UpdateAudit(entity, userId);
            }
            return entities;
        }
        public virtual T UpdateAudit(T entity, int userId)
        {
            entity.UpdateAuditData(userId);
            Session.Update(entity);
            return entity;
        }
    }
}