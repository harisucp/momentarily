using Apeek.Common;
using Apeek.Common.Interfaces;
namespace Apeek.Core.Interfaces
{
    public interface IShapeFactory : ISingletonDependency
    {
        IShape<TModel> BuildShape<TModel>(string viewName, TModel models, string pageName = null);
    }
}