using System.Collections.Generic;
using Apeek.Common.Interfaces;
using Apeek.Entities.Entities;
using Apeek.NH.Repository.Common;
namespace Apeek.NH.Repository.Repositories
{
    public interface IRepositoryCategory : IRepositoryClosureTree<Category>, IDependency
    {
        IList<Category> GetCategories();
        Category GetCategory(int categoryId);
    }
}