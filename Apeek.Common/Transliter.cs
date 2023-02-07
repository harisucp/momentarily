using System.Collections.Generic;
using System.Text;
using Apeek.Common.Controllers;
namespace Apeek.Common
{
    public class Transliter
    {
        private static Dictionary<Language, Dictionary<string, string>> translitDict = new Dictionary<Language, Dictionary<string, string>>();
        static Transliter()
        {
            translitDict = GetWordTranslitDictionary();
        }
        public static string Translit(string str, Language lang)
        {
            StringBuilder newStr = new StringBuilder();
            foreach (char c in str)
            {
                string current = c.ToString().ToLower();
                if (translitDict[lang].ContainsKey(current))
                {
                    newStr.Append(translitDict[lang][current]);
                }
                else
                {
                    newStr.Append(current);
                }
            }
            return newStr.ToString();
        }
        public static Dictionary<Language, Dictionary<string, string>> GetWordTranslitDictionary()
        {
            foreach (var lang in LanguageController.GetAllLanguages())
            {
                translitDict.Add(lang, GetDict(lang));
            }
            return translitDict;
        }
        private static Dictionary<string, string> GetDict(Language lang)
        {
            switch (lang)
            {
                case Language.uk:
                    return GetUkrDict();
                case Language.ru:
                    return GetRusDict();
                default:
                    return GetUkrDict();
            }
        }
        private static Dictionary<string, string> GetRusDict()
        {
            var dict = new Dictionary<string, string>();
            dict.Add("'", "");
            dict.Add("&", "and");
            dict.Add("а", "a");
            dict.Add("б", "b");
            dict.Add("в", "v");
            dict.Add("г", "g");
            dict.Add("ґ", "g");
            dict.Add("д", "d");
            dict.Add("е", "e");
            dict.Add("ё", "yo");
            dict.Add("ж", "zh");
            dict.Add("з", "z");
            dict.Add("и", "i");
            dict.Add("й", "j");
            dict.Add("к", "k");
            dict.Add("л", "l");
            dict.Add("м", "m");
            dict.Add("н", "n");
            dict.Add("о", "o");
            dict.Add("п", "p");
            dict.Add("р", "r");
            dict.Add("с", "s");
            dict.Add("т", "t");
            dict.Add("у", "u");
            dict.Add("ф", "f");
            dict.Add("х", "h");
            dict.Add("ц", "c");
            dict.Add("ч", "ch");
            dict.Add("ш", "sh");
            dict.Add("щ", "shh");
            dict.Add("ъ", "#");
            dict.Add("ы", "y");
            dict.Add("ь", "");
            dict.Add("э", "je");
            dict.Add("ю", "yu");
            dict.Add("я", "ya");
            dict.Add("і", "i");
            dict.Add("ї", "ji");
            dict.Add("є", "ye");
            return dict;
        }
        private static Dictionary<string, string> GetUkrDict()
        {
            var dict = new Dictionary<string, string>();
            dict.Add("'", "");
            dict.Add("&", "and");
            dict.Add("а", "a");
            dict.Add("б", "b");
            dict.Add("в", "v");
            dict.Add("г", "g");
            dict.Add("ґ", "g");
            dict.Add("д", "d");
            dict.Add("е", "e");
            dict.Add("є", "ye");
            dict.Add("ж", "zh");
            dict.Add("з", "z");
            dict.Add("и", "y");
            dict.Add("і", "i");
            dict.Add("ї", "ji");
            dict.Add("й", "j");
            dict.Add("к", "k");
            dict.Add("л", "l");
            dict.Add("м", "m");
            dict.Add("н", "n");
            dict.Add("о", "o");
            dict.Add("п", "p");
            dict.Add("р", "r");
            dict.Add("с", "s");
            dict.Add("т", "t");
            dict.Add("у", "u");
            dict.Add("ф", "f");
            dict.Add("х", "h");
            dict.Add("ц", "c");
            dict.Add("ч", "ch");
            dict.Add("ш", "sh");
            dict.Add("щ", "shh");
            dict.Add("ь", "");
            dict.Add("ю", "yu");
            dict.Add("я", "ya");
            dict.Add("ъ", "#");
            dict.Add("ы", "y");
            dict.Add("э", "je");
            dict.Add("ё", "yo");
            return dict;
        }
    }
}
