using System.Collections.Generic;
using System.Linq;
using Apeek.Entities.Entities;
using Apeek.NH.DataAccessLayer.DataAccess;
using Apeek.NH.Repository.Common;
namespace Apeek.NH.Repository.Repositories.Impl
{
    public class RepositoryCategory : RepositoryClosureTree<Category>, IRepositoryCategory
    {
        public override void RemoveEntity(int categoryId)
        {
            var idsToDelete = GetChildIds(categoryId);
            RemoveEntityTree(categoryId);
            var sql = @"Delete from c_category where id in (:idsToDelete);";
            Session.CreateSQLQuery(sql)
                .SetParameterList("idsToDelete", idsToDelete)
                .ExecuteUpdate();
        }
        protected override Category CreateRoot()
        {
            return new Category()
            {
                IsRoot = true,
            };
        }
        protected override string EntityTableName
        {
            get { return Category._TableName; }
        }
        protected override string EntityTreeTableName
        {
            get { return "c_category_tree"; }
        }
        public IList<Category> GetCategories()
        {
            var root = GetRoot();
            return GetCategories(root.Id, 0);
        }
        private IList<Category> GetCategories(int rootId, int maxDepth)
        {
            var query = string.Format(@"
                SELECT * FROM c_category AS c
                LEFT JOIN c_category_tree AS ct ON ct.child_id = c.id
                WHERE depth <= {0} AND ct.parent_id = {1}", maxDepth, rootId);
            return Session.CreateSQLQuery(query)
                    .AddEntity(typeof(Category))
                    .List<Category>();
        }
        public Category GetCategory(int categoryId)
        {
            return Table.FirstOrDefault(p => p.Id == categoryId);
        }
    }
}