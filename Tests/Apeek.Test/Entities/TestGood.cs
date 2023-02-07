using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apeek.Entities.Attributies;
using Apeek.Entities.Entities;
using Apeek.Test.Common;
namespace Apeek.Test.Entities
{
    public class TestGood : Good
    {
        [DictionaryProperty(TestConstants.testGoodPropertyName)]
        public virtual string TestField { get; set; }
        [DictionaryPropertyIDValue(TestConstants.selectedGoodPropertyName)]
        public virtual int TestValueDefinitionField { get; set; }
    }
}
