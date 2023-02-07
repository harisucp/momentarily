using System.Collections.Generic;
using System.Text.RegularExpressions;
namespace Apeek.Common.Extensions
{
    public class RegexHelper
    {
        public static List<string> SplitMatches(string value, string pattern)
        {
            var res1 = Regex.Matches(value, RegexPattern.GetWords);
            var list = new List<string>();
            foreach (Match match in res1)
            {
                list.Add(match.Value);
            }
            return list;
        }
    }
}