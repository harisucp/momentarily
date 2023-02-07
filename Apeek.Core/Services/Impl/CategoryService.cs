using System;
using System.Collections.Generic;
using System.Linq;
using Apeek.Common;
using Apeek.Common.Logger;
using Apeek.Entities.Entities;
using Apeek.NH.DataAccessLayer.DataAccess;
using Apeek.NH.Repository.Repositories;
namespace Apeek.Core.Services.Impl
{
    public class CategoryService : ICategoryService
    {
        private readonly IRepositoryCategory _repCategory;
        public CategoryService(IRepositoryCategory repCategory)
        {
            _repCategory = repCategory;
        }
        public List<Category> GetAllCategories()
        {
            var categories = new List<Category>();
            Uow.Wrap(u =>
            {
                categories = _repCategory.Table.ToList();
            });
            return categories;
        }
        public IList<KeyValuePair<int, string>> GetRootChilds()
        {
            try
            {
                IList<KeyValuePair<int, string>> rootChilds = null;
                Uow.Wrap(u =>
                {
                    var categories = _repCategory.GetCategories();
                    rootChilds = categories.Select(item => new KeyValuePair<int, string>(item.Id, item.Name)).OrderBy(i=>i.Value).ToList();
                });
                return rootChilds;
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.CategoryService, string.Format("CategoryService.GetRootChilds() error. Ex: {0}.", ex));
            }
            return new List<KeyValuePair<int, string>>();
        }
    }
}