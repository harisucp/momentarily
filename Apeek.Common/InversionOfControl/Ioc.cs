using System;
using StructureMap;
namespace Apeek.Common
{
    public class Ioc
    {
        public static IContainer _container;
        public static object _createInstanceLocker = new object();
        public static IContainer Instance
        {
            get
            {
                if (_container == null)
                {
                    lock (_createInstanceLocker)
                    {
                        if (_container == null)
                            _container = new Container();
                    }
                }
                return _container;
            }
        }
        public static T Get<T>()
        {
            return Instance.GetInstance<T>();
        }
        public static object Get(Type type)
        {
            return Instance.GetInstance(type);
        }
        public static void Add(Action<ConfigurationExpression> exp)
        {
            Instance.Configure(exp);
        }
    }
}