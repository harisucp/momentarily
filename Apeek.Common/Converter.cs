using System;
namespace Apeek.Common
{
    public class Converter
    {
        public static int? GetInt(string str)
        {
            int val;
            if (int.TryParse(str, out val))
            {
                return val;
            }
            return null;
        }
        public static bool? GetBool(string str)
        {
            bool val;
            if (bool.TryParse(str, out val))
            {
                return val;
            }
            return null;
        }
        public static int GetIntOrDefault(object obj)
        {
            int val;
            if (obj != null && int.TryParse(obj.ToString(), out val))
            {
                return val;
            }
            return 0;
        }
        public static string GetString(object obj)
        {
            if (obj != null)
            {
                return obj.ToString();
            }
            return null;
        }
        public static int? GetInt(object obj)
        {
            int val;
            if (obj != null && int.TryParse(obj.ToString(), out val))
            {
                return val;
            }
            return null;
        }
        public static double? GetDouble(object obj)
        {
            double val;
            if (obj != null && double.TryParse(obj.ToString(), out val))
            {
                return val;
            }
            return null;
        }
        public static DateTime? GetDateTime(object obj)
        {
            DateTime val;
            if (obj != null && DateTime.TryParse(obj.ToString(), out val))
            {
                return val;
            }
            return null;
        }
    }
}