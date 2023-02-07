using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using NHibernate.Transform;
namespace Apeek.NH.Repository.Transformers
{
    public class ExpandoObjectResultSetTransformer : IResultTransformer
    {
        public IList TransformList(IList collection)
        {
            return collection;
        }
        public object TransformTuple(object[] tuple, string[] aliases)
        {
            var expando = new ExpandoObject();
            var dictionary = (IDictionary<string, object>)expando;
            for (int i = 0; i < tuple.Length; i++)
            {
                string alias = aliases[i];
                if (alias != null)
                {
                    dictionary[alias] = tuple[i];
                }
            }
            return expando;
        }
    }
}
