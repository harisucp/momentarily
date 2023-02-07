using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Apeek.Common;
using Apeek.Common.Logger;
using Momentarily.EventManager.Tasks;
namespace Momentarily.EventManager
{
    public class TaskLoader
    {
        public List<Type> ScanForTasks(string pathToScan)
        {
            var baseType = typeof(EventManagerTask);
            if (string.IsNullOrEmpty(pathToScan))   
            {
                pathToScan = Directory.GetParent(baseType.Module.FullyQualifiedName).FullName; // warkaround for Unit Test
            }
            var files = Directory.GetFiles(pathToScan, "*.*", SearchOption.AllDirectories)
                .Where(x =>
                {
                    var fileName = Path.GetFileName(x);
                    return fileName != null && (fileName.ToLower().StartsWith("momentarily.") && (x.ToLower().EndsWith(".dll") || x.ToLower().EndsWith(".exe")));
                }).ToArray();
            var tasks = new List<Type>();
            foreach (var file in files)
            {
                try
                {
                    var assembly = Assembly.LoadFile(file);
                    tasks.AddRange(assembly.GetTypes().Where(type => baseType.IsAssignableFrom(type) && type.IsClass && string.Compare(baseType.Name, type.Name, StringComparison.OrdinalIgnoreCase) != 0));
                }
                catch (Exception ex)
                {
                    Ioc.Get<IDbLogger>().LogMessage(LogSource.EventManager, string.Format("Exception while loading tasks: {0}", ex));
                }
            }
            return tasks;
        }
    }
}
