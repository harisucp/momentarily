using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Apeek.Entities.Attributies
{
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class DictionaryPropertyAttribute : Attribute
    {
        public string PropertyName
        {
            get;
            private set;
        }
        public DictionaryPropertyAttribute(string propertyName)
        {
            this.PropertyName = propertyName;
        }
    }
    public class DictionaryPropertyIDValue : Attribute
    {
        public string PropertyName
        {
            get;
            private set;
        }
        public DictionaryPropertyIDValue(string propertyName)
        {
            PropertyName = propertyName;
        }
    }
}
