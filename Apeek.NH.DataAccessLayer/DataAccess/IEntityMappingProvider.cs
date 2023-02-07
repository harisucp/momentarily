using System;
using FluentNHibernate.Cfg;
namespace Apeek.Common.Interfaces
{
    public interface IEntityMappingProvider:IDependency
    {
        void GetMapping(MappingConfiguration mapConfig);
    }
}