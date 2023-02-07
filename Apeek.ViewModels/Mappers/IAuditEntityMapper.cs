using Apeek.Common.Interfaces;
using Apeek.Entities.Interfaces;
namespace Apeek.ViewModels.Mappers
{
    public interface IAuditEntityMapper<Src, Dest> : IDependency
        where Src : IAuditEntity
        where Dest : IAuditEntity
    {
        Dest AuditMap(Src source, Dest dest);
        Src AuditMap(Dest source, Src dest);
    }
}