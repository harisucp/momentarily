using System.Collections;
using System.Collections.Generic;
using NHibernate.Transform;
namespace Apeek.NH.Repository.Transformers
{
    public class TransformerValueList : IResultTransformer
    {
        List<string> _values = new List<string>();
        public IList TransformList(IList collection)
        {
            foreach (var item in collection)
            {
                var tuple = (object[])item;
                _values.Add(tuple[0] != null ? tuple[0].ToString() : null);
            }
            return _values;
        }
        public object TransformTuple(object[] tuple, string[] aliases)
        {
            return tuple;
        }
    }
}