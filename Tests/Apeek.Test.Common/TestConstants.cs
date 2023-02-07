using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace Apeek.Test.Common
{
    public class TestConstants
    {
        public const string testFieldTypeNameSuffix = "_fieldTypeName_unitTest";
        public const string testFieldNameSuffix = "_fieldName_unitTest";
        public const string testGoodNameSuffix = "_goodName_unitTest";
        public const string testGoodPropertyName = "TestField" + testFieldNameSuffix;
        public const string testGoodPropertyTypeName = "TestPropertyType" + testFieldTypeNameSuffix;
        public const string selectedGoodPropertyName = "SelectedField" + testFieldNameSuffix;
        public const string selectedGoodPropertyTypeName = "selectedPropertyType" + testFieldTypeNameSuffix;
    }
}
