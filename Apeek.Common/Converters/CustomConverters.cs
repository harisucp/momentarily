using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
namespace Apeek.Common.Converters
{
    public class CustomConverters
    {
        public static IList<T> StringToSeparatedList<T>(string str, string delimiter = ",")
        {
            var vals = new List<T>();
            if (string.IsNullOrWhiteSpace(str))
                return vals;
            foreach (var s in str.Split(new[] { delimiter }, StringSplitOptions.RemoveEmptyEntries))
            {
                vals.Add((T)Convert.ChangeType(s, typeof (T)));
            }
            return vals;
        }
        public static string ListToSeparatedString<T>(IList<T> list, string delimiter = ",") where T : struct
        {
            if (list == null || list.Count == 0)
                return null;
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < list.Count; i++)
            {
                builder.Append(list[i]);
                if (i < (list.Count - 1))
                {
                    builder.Append(delimiter);
                }
            }
            return builder.ToString();
        }
        public static string ListToSeparatedString(IList<string> list, string delimiter = ",")
        {
            if (list == null || list.Count == 0)
                return null;
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < list.Count; i++)
            {
                builder.Append(list[i]);
                if (i < (list.Count - 1))
                {
                    builder.Append(delimiter);
                }
            }
            return builder.ToString();
        }
        public static List<SelectListItem> ToSelectListItems(List<string> list)
        {
            var selectListItems = new List<SelectListItem>();
            list.ForEach(x=>selectListItems.Add(new SelectListItem(){ Text = x, Value = x}));
            return selectListItems;
        }
    }
}
