using Apeek.Common;
using Apeek.Common.Interfaces;
namespace Apeek.Core.Interfaces
{
    public interface ITranslateController : ISingletonDependency
    {
        string this[string key] { get; }
        string TranslateCase(string originalValue, Cases c);
        void Refresh();
    }
}