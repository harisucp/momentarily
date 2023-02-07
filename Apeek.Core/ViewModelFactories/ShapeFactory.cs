using Apeek.Core.Interfaces;
using Apeek.Core.Services;
using Apeek.Common;
namespace Apeek.Core.ViewModelFactories
{
    public class ShapeFactory : IShapeFactory
    {
        private readonly ISeoEntryService _seoEntryService;
        public ShapeFactory(ISeoEntryService seoEntryService)
        {
            _seoEntryService = seoEntryService;
        }
        public IShape<TModel> BuildShape<TModel>(string viewName, TModel model, string pageName = null)
        {
            if (string.IsNullOrEmpty(pageName))
                pageName = PageName.Other.ToString();
            return new Shape<TModel>(viewName, model, _seoEntryService.GetSeoEntry(viewName), null, pageName);
        }
    }
}