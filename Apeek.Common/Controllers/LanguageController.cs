using System;
using System.Collections.Generic;
using Apeek.Common.HttpContextImpl;
namespace Apeek.Common.Controllers
{
    public class LanguageController
    {
        public static Language DefLanguage
        {
            get { return Language.en; }
        }
        public static Language GetLanguage(string lang)
        {
            Language result;
            if (!Enum.TryParse(lang, true, out result))
            {
                result = DefLanguage;
            }
            return result;
        }
        public static void UpdateCurLang()
        {
            CurLang = DefLanguage;
        }
        public static Language GetLanguage(int langid)
        {
            switch ((Language)langid)
            {
                case Language.uk:
                    return Language.uk;
                case Language.ru:
                    return Language.ru;
                case Language.en:
                    return Language.en;
                default:
                    return DefLanguage;
            }
        }
        public static void SetCurLang(int langid)
        {
            CurLang = GetLanguage(langid);
        }
        public static List<Language> GetAllLanguages()
        {
            return new List<Language> { Language.uk, Language.ru, Language.en };
        }
        public static int Count()
        {
            return 3;
        }
        public static Dictionary<Language, string> CreateLangFields()
        {
            var langFields = new Dictionary<Language, string>();
            foreach (var lang in GetAllLanguages())
            {
                langFields.Add(lang, "");
            }
            return langFields;
        }
        public static Language CurLang
        {
            get
            {
                if (HttpContextFactory.Current is NoHttpContext)
                {
                    return DefLanguage;
                }
                //request from mvc
                if (HttpContextFactory.Current.Session != null)
                    return (Language)HttpContextFactory.Current.Session[QS.lang];
                else //request from web api
                    return ContextService.FromQueryString.Lang;
            }
            private set { HttpContextFactory.Current.Session[QS.lang] = value; }
        }
        public static int CurLangId
        {
            get { return (int)CurLang; }
        }
    }
}