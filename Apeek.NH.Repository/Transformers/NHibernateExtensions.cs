using System.Collections.Generic;
using NHibernate;
namespace Apeek.NH.Repository.Transformers
{
    public static class NHibernateExtensions
    {
        public static IList<dynamic> DynamicList(this IQuery query)
        {
            return query.SetResultTransformer(new ExpandoObjectResultSetTransformer())
                .List<dynamic>();
        }
    }
}