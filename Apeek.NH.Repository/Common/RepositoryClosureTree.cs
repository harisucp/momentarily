using System.Collections.Generic;
using System.Linq;
using Apeek.Entities.Interfaces;
namespace Apeek.NH.Repository.Common
{
    public interface IRepositoryClosureTree<ET> : IRepositoryAudit<ET> where ET : AuditEntity, IEntityTree
    {
        ET UpdateAudit(ET entity, int userId, bool needMoveSubTree);
    }
    //how to work with closure tables
    //http://technobytz.com/closure_table_store_hierarchical_data.html
    public abstract class RepositoryClosureTree<ET> : RepositoryAudit<ET>, IRepositoryClosureTree<ET> where ET : AuditEntity, IEntityTree
    {
        public override ET SaveAudit(ET entityTree, int userId)
        {
            base.SaveAudit(entityTree, userId);
            CreateEntityTree(entityTree);
            return entityTree;
        }
        public ET UpdateAudit(ET entity, int userId, bool needMoveSubTree)
        {
            base.UpdateAudit(entity, userId);
            if (needMoveSubTree)
            {
                var parentId = (entity.ParentId.HasValue && entity.ParentId.Value > 0) ? entity.ParentId.Value : GetRoot().Id;
                MoveSubTree(entity.Id, parentId);
            }
            return entity;
        }
        public void CreateEntityTree(ET entityTree)
        {
            //creating default record with 0 depth
            SaveTreeForEntity(entityTree);
            var parentId = (entityTree.ParentId.HasValue && entityTree.ParentId.Value > 0) ? entityTree.ParentId.Value : GetRoot().Id;
            //creating tree
            SaveTree(entityTree, parentId);
        }
        private void SaveTreeForEntity(ET entityTree)
        {
            var sql = string.Format(@"INSERT INTO {0}(parent_id, child_id, depth) values (:pId, :cId, :depth)", EntityTreeTableName);
            Session.CreateSQLQuery(sql)
                .SetInt32("pId", entityTree.Id)
                .SetInt32("cId", entityTree.Id)
                .SetInt32("depth", 0)
                .ExecuteUpdate();
        }
        public ET GetRoot()
        {
            var entityTree = Table.FirstOrDefault(x => x.IsRoot);
            if (entityTree == null)
            {
                entityTree = CreateRoot();
                base.Save(entityTree);
                SaveTreeForEntity(entityTree);
            }
            return entityTree;
        }
        public void SaveTree(ET entityTree, int parentId)
        {
            var sql = string.Format(@"INSERT INTO {0}(parent_id, child_id, depth)
                        SELECT p.parent_id, c.child_id, p.depth+c.depth+1
                        FROM {0} p, {0} c
                        WHERE p.child_id = :parentId AND c.parent_id = :newEntityId", EntityTreeTableName);
            Session.CreateSQLQuery(sql)
                .SetInt32("parentId", parentId)
                .SetInt32("newEntityId", entityTree.Id)
                .ExecuteUpdate();
        }
        public void RemoveEntityTree(int entityId)
        {
            var sql = string.Format(@"Delete stToDelete From {0} as stToDelete
                        join {0} as st on st.child_id = stToDelete.child_id
                        Where st.parent_id=:entityId", EntityTreeTableName);
            Session.CreateSQLQuery(sql)
                .SetInt32("entityId", entityId)
                .ExecuteUpdate();
        }
        public IList<int> GetChildIds(int entityId)
        {
            var sql = string.Format(@"Select child_id from {0} where parent_id=:entityId", EntityTreeTableName);
            return Session.CreateSQLQuery(sql)
                .SetInt32("entityId", entityId)
                .List<int>();
        }
        public void MoveSubTree(int entityId, int parentId)
        {
            var sql = string.Format(@"Delete a FROM {0} AS a
                                    JOIN {0} AS d ON a.child_id = d.child_id
                                    LEFT JOIN {0} AS x
                                    ON x.parent_id = d.parent_id AND x.child_id = a.parent_id
                                    WHERE d.parent_id = :entityId AND x.parent_id IS NULL;
                                    INSERT INTO {0} (parent_id, child_id, depth)
                                    SELECT supertree.parent_id, subtree.child_id, supertree.depth+subtree.depth+1
                                    FROM {0} AS supertree JOIN {0} AS subtree
                                    WHERE subtree.parent_id = :entityId
                                    AND supertree.child_id = :parentId;", EntityTreeTableName);
            Session.CreateSQLQuery(sql)
                .SetInt32("entityId", entityId)
                .SetInt32("parentId", parentId)
                .ExecuteUpdate();
        }
        public abstract void RemoveEntity(int serviceId);
        protected abstract ET CreateRoot();
        protected abstract string EntityTableName { get; }
        protected abstract string EntityTreeTableName { get; }
    }
}