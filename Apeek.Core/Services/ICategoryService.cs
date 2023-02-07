using System.Collections.Generic;
using Apeek.Common.Interfaces;
using Apeek.Entities.Entities;
namespace Apeek.Core.Services
{
    public interface ICategoryService : IDependency
    {
        List<Category> GetAllCategories();
        IList<KeyValuePair<int, string>> GetRootChilds();
    }
}