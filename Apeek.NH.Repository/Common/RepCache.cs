using System;
using System.Collections.Generic;
using System.Linq;
using Apeek.Common.Interfaces;
using Apeek.Common.Validation;
namespace Apeek.NH.Repository.Common
{
    public class RepCache<T> : Repository<T>, IDependency where T : class, new()
    {
        static Dictionary<Type, List<T>> _dictionary = new Dictionary<Type, List<T>>();
        public static bool Loaded { get; set; }
        private static object _lockObject = new object();
        public static void Clear()
        {
            _dictionary.Clear();
            Loaded = false;
        }
        public List<T> List()
        {
            var list = TryGetFromCache();
            if (list == null)
            {
                lock (_lockObject)
                {
                    if (list == null)
                    {
                        list = Table.ToList();
                        AddToCache(list);
                        Loaded = true;
                    }
                }
            }
            return list;
        }
        public List<T> List(Func<List<T>> funcThatReturnsListOfObjToCache)
        {
            Argument.ThrowIfNull(funcThatReturnsListOfObjToCache, "funcThatReturnsListOfObjToCache");
            var list = TryGetFromCache();
            if (list == null)
            {
                list = funcThatReturnsListOfObjToCache();
                AddToCache(list);
                Loaded = true;
            }
            return list;
        }
        private List<T> TryGetFromCache()
        {
            if (_dictionary.ContainsKey(typeof (T)))
                return _dictionary[typeof (T)];
            return null;
        }
        private void AddToCache(List<T> list)
        {
            if (!_dictionary.ContainsKey(typeof (T)))
            {
                _dictionary.Add(typeof (T), list);
            }
        }
    }
}
