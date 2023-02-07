using Apeek.Common;
namespace Apeek.ViewModels.Mappers.Imp
{
    public class EntityMapper<T> where T:class
    {
        public static T Mapper()
        {
            return Ioc.Get<T>();
        }
    }
}