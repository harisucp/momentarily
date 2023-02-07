using System;
using System.Linq;
using System.Text.RegularExpressions;
namespace Apeek.Entities.Validators
{
    public static class StringHelper
    {
        public static string ToUpperFirstChar(this string input)
        {
            if (String.IsNullOrEmpty(input))
                return null;
            return input.First().ToString().ToUpper() + input.Substring(1);
        }
        public static string MultipleSpacesToSingleSpace(string stringDirty)
        {
            Regex r = new Regex(@"\s+");
            stringDirty = r.Replace(stringDirty, @" ");
            return stringDirty.Trim();
        }
        public static string RemoveWhiteSpaces(string stringDirty)
        {
            Regex r = new Regex(@"\s+");
            stringDirty = r.Replace(stringDirty, @"");
            return stringDirty.Trim();
        }
        public static string GetByPattern(string stringDirty, string pattern, int matchIndex)
        {
            var regex = new Regex(pattern);
            MatchCollection matches = regex.Matches(stringDirty);
            if (matches.Count > matchIndex)
            {
                return matches[matchIndex].ToString().Trim();
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(stringDirty))
                    return stringDirty.Trim();
                return string.Empty;
            }
        }
        public static string RemoveSpecialCharacters(string stringDirty)
        {
            if (string.IsNullOrWhiteSpace(stringDirty))
                return null;
            Regex r = new Regex(@"[^a-zA-Z0-9\-\s]");
            stringDirty = r.Replace(stringDirty, @"");
            return MultipleSpacesToSingleSpace(stringDirty);
        }
        public static string ExstractDigitsForPhoneNumber(string stringDirty)
        {
            Regex r = new Regex(@"[^\+0-9*]");
            stringDirty = r.Replace(stringDirty, @"");
            return MultipleSpacesToSingleSpace(stringDirty);
        }
        public static string ExstractDigits(string stringDirty)
        {
            Regex r = new Regex(@"[^0-9*]");
            stringDirty = r.Replace(stringDirty, @"");
            return MultipleSpacesToSingleSpace(stringDirty);
        }
        public static string ExstractWords(string stringDirty)
        {
            Regex r = new Regex(@"[^а-яїіє\w]+");
            stringDirty = r.Replace(stringDirty, @" ");
            return MultipleSpacesToSingleSpace(stringDirty);
        }
        public static string RemoveSpecialCharactersExcept_Percent(string stringDirty)
        {
            Regex r = new Regex(@"[^a-zA-Z0-9\s()%+*]");
            stringDirty = r.Replace(stringDirty, @"");
            return MultipleSpacesToSingleSpace(stringDirty);
        }
        public static string ToUppercaseFirstChar(string value)
        {
            value = value.ToLowerInvariant();
            char[] array = value.ToCharArray();
            // Handle the first letter in the string.
            if (array.Length >= 1)
            {
                if (char.IsLower(array[0]))
                {
                    array[0] = char.ToUpper(array[0]);
                }
            }
            // Scan through the letters, checking for spaces.
            // ... Uppercase the lowercase letters following spaces.
            for (int i = 1; i < array.Length; i++)
            {
                if (array[i - 1] == ' ')
                {
                    if (char.IsLower(array[i]))
                    {
                        array[i] = char.ToUpper(array[i]);
                    }
                }
            }
            return new string(array);
        }
        public static string FormatPunctuation(string str)
        {
            string commaPattern = @"[\,](?!\s)";
            string dotPattern = @"[\.](?!\s)";
            string questPattern = @"[\?](?!\s)";
            string exclamationPattern = @"[\!](?!\s)";
            string doubleDotesPattern = @"[\:](?!\s)";
            string dotCommaPattern = @"[\;](?!\s)";
            str = Regex.Replace(str, commaPattern, @", ");
            str = Regex.Replace(str, dotPattern, @". ");
            str = Regex.Replace(str, questPattern, @"? ");
            str = Regex.Replace(str, exclamationPattern, @"! ");
            str = Regex.Replace(str, doubleDotesPattern, @": ");
            str = Regex.Replace(str, dotCommaPattern, @"; ");
            return str.Trim();
        }
    }
}