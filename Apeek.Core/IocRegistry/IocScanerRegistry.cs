using System;
using System.Linq;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using Apeek.Common.Interfaces;
namespace Apeek.Core.IocRegistry
{
    public class IocScanerRegistry : Registry
    {
        public IocScanerRegistry(string pathToScan, string[] assembliesPrefix)
        {
            foreach (var name in assembliesPrefix)
            {
                Scan(scan =>
                {
                    scan.AssembliesFromPath(pathToScan, asmbl => asmbl.GetName().Name.ToLower().Contains(name.ToLower() + "."));
                    scan.Exclude(t => !HasDefaultConstructor(t));
                    scan.WithDefaultConventions();
                });
            }
        }
        private static bool HasDefaultConstructor(Type type)
        {
            if (type.GetInterfaces().ToList().Find(x => x.FullName == typeof(IDependency).FullName) != null)
            {
                return true;
            }
            return false;
        }
    }
}