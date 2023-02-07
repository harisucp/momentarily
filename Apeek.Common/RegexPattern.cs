namespace Apeek.Common
{
    public class RegexPattern
    {
        public const string GetIdFromUrl = "^[0-9]+((?=-)|($))";
        public const string GetEmptySpace = @"\s+";
        public const string GetWords = @"[а-яА-Я\w]+";
    }
}