using System;
using System.Collections.Generic;
using System.Linq;
using Apeek.Common.Extensions;
using Apeek.Common.Interfaces;
using Apeek.Entities.Interfaces;
namespace Apeek.NH.DataAccessLayer
{
    public class EntityMappingProvider : IEntityMappingProvider
    {
        public void GetMapping(FluentNHibernate.Cfg.MappingConfiguration mapConfig)
        {
            var interfaceType = typeof(IEntity);
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
              .SelectMany(x => x.GetLoadableTypes())
              .Where(x => interfaceType.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
              .Select(x =>x.Assembly).Distinct();
            foreach (var a in assemblies)
            {
                mapConfig.FluentMappings.AddFromAssembly(a);
            }
        }
    }
}