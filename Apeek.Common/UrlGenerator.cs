using System;
using System.Text.RegularExpressions;
using Apeek.Common.Interfaces;
using Apeek.Entities.Validators;
namespace Apeek.Common
{
    public interface IUrlGenerator : IDependency
    {
        string CreateUrl(string first, string second, Language lang, string delim=UrlGenerator.Delim);
        string GetUrl(int id, string url);
        int? GetId(string url);
    }
    public class UrlGenerator : IUrlGenerator
    {
        public const string Delim = "-";
        public string CreateUrl(string first, string second, Language lang, string delim = Delim)
        {
            var url = Format(first, second, delim);
            if (string.IsNullOrEmpty(url))
                return null;
            if (string.IsNullOrWhiteSpace(delim))
                delim = string.Empty;
            url = Transliter.Translit(url, lang);
            url = StringHelper.RemoveSpecialCharacters(url);
            url = url.Replace(" - ","-");
            url = Regex.Replace(url.Trim(), RegexPattern.GetEmptySpace, delim);
            return url;
        }
        private string Format(string first, string second, string delim)
        {
            if (string.IsNullOrWhiteSpace(second))
                return Format(first);
            if (string.IsNullOrWhiteSpace(first))
                return Format(second);
            if(string.IsNullOrWhiteSpace(delim))
                return string.Format("{0}{1}", first, second);
            return string.Format("{0}{1}{2}", first, delim, second);
        }
        private string Format(string first)
        {
            if (string.IsNullOrWhiteSpace(first))
                return null;
            return first;
        }
        public string GetUrl(int id, string url)
        {
            if (id == 0)
                return null;
            if(string.IsNullOrWhiteSpace(url))
                return string.Format("{0}", id);
            return string.Format("{0}{1}{2}", id, Delim, url);
        }
        public string GetUrl(int idFirst, int idSecond)
        {
            if (idFirst == 0 || idSecond == 0)
                return null;
            return string.Format("{0}{1}{2}", idFirst, Delim, idSecond);
        }
        public int? GetId(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return null;
            var regex = new Regex(RegexPattern.GetIdFromUrl);
            var match = regex.Match(url);
            if (match.Success)
                return Convert.ToInt32(match.Value);
            return null;
        }
    }
}
