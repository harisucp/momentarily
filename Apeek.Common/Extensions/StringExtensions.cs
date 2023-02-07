using System.Collections.Generic;
using System.Text;
using Apeek.Common.Converters;
namespace Apeek.Common.Extensions
{
    public static class StringExtensions
    {
        public static string ReplaceWithPlaceholders(this string str, string citywhere, string city, string page, string service)
        {
            return str.Replace(Placeholders.citywhere, citywhere)
                .Replace(Placeholders.city, city)
                .Replace(Placeholders.service, service)
                .Replace(Placeholders.page, page);
        }
        public static string RemoveBr(this string str, int lenght)
        {
            if (str.Length > lenght)
                str = str.Substring(0, lenght);
            return str.Replace(Constants.Html.Br, string.Empty);
        }
        public static string RnToBr(this string str)
        {
            return str.Replace(Constants.Html.N, Constants.Html.Br);
        }
        public static string BrToRn(this string str)
        {
            return str.Replace(Constants.Html.Br, Constants.Html.N);
        }
    }
    public static class ListExtensions
    {
        public static string ToSeparatedString<T>(this IList<T> list, string delimiter = ",") where T : struct
        {
            return CustomConverters.ListToSeparatedString(list, delimiter);
        }
    }
}