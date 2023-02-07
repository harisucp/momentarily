using System.Collections.Generic;
namespace Apeek.Common.Extensions
{
    public static class CommonExtensions
    {
        public static void AddItemIfNotEmpty(this Dictionary<string, string> dict, string key, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                dict.Add(key, value);
            }
        }
    }
}