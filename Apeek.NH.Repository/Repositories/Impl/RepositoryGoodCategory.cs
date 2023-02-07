using System.Collections.Generic;
using System.Linq;
using Apeek.Entities.Entities;
using Apeek.NH.Repository.Common;
namespace Apeek.NH.Repository.Repositories.Impl
{
    public class RepositoryGoodCategory : RepositoryAudit<GoodCategory>, IRepositoryGoodCategory
    {
        public int GetGoodCategoryId(int goodId)
        {
            var category = Table.FirstOrDefault(p => p.GoodId == goodId);
            if (category != null)
                return category.CategoryId;
            return 0;
        }
        public string GetGoodCategorylist(int goodId)
        {
            string concateCategoryId = string.Empty;
            var categoryids = Table.Where(p => p.GoodId == goodId).Select(x => x.CategoryId).ToList();
            if (categoryids != null)
            {
                foreach (var item in categoryids)
                {
                    concateCategoryId += item + ",";
                }
                concateCategoryId = concateCategoryId.TrimEnd(',');
                return concateCategoryId;
            }
            return string.Empty;

        }

    }
}
