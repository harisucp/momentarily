using System;using System.Collections.Generic;using System.Linq;using System.Text.RegularExpressions;using Apeek.Entities.Constants;namespace Apeek.Entities.Validators{    public class ValidatorPhoneNumber : ValidatorBase    {        public const int PhoneNumberDigitCount = 10;        public const int PhoneNumberDigitCount38 = 12;        public static List<string> PhoneCodes = new List<string>        {            "039",            "050",            "063",            "066",            "067",            "068",            "091",            "092",            "093",            "094",            "095",            "096",            "097",            "098",            "099",        };        public string PhoneNumber { get; set; }        public override bool IsValid(object value)        {            var inputPhoneNumber = value as string;            if (string.IsNullOrWhiteSpace(inputPhoneNumber))            {                return true;            }            //inputPhoneNumber = GetPhoneNumberByCountry(inputPhoneNumber, Countries.USA);            if (string.IsNullOrWhiteSpace(inputPhoneNumber))            {                ErrorMessage = ValidationErrors.FillInCorrectPhoneNumberUSA;                return false;            }            PhoneNumber = inputPhoneNumber;            return true;        }        public bool IsValidCode(string phoneNumber)        {            if (string.IsNullOrWhiteSpace(phoneNumber))            {                return false;            }            var code = phoneNumber.Substring(0, 3);            if (PhoneCodes.Contains(code))            {                return true;            }            return false;        }        public string GetPhoneNumber(string phoneNumber)        {            phoneNumber = StringHelper.ExstractDigits(phoneNumber);            var match = Regex.Match(phoneNumber, "^[0-9]{10}$");            if (match.Success)            {                return match.Value;            }            var match1 = Regex.Match(phoneNumber, "(?<=^38)[0-9]{10}$");            if (match1.Success)            {                return match1.Value;            }            return null;        }        public string GetPhoneNumberByCountry(string phoneNumber, Countries country)        {            var countryPattern = GetPatternForCountry(country);            string splitPhoneNo = Regex.Replace(phoneNumber, @"[^0-9a-zA-Z]+", "");
            //phoneNumber = StringHelper.ExstractDigits(phoneNumber);
            var match = Regex.Match(splitPhoneNo, countryPattern);            if (match.Success)            {                return match.Value;            }            return null;        }        private string GetPatternForCountry(Countries country)        {            switch (country)            {                case Countries.Australia:
                    return RegExpPatterns.CountriesPhoneNumbers.patternAustralia;                case Countries.USA:                    return RegExpPatterns.CountriesPhoneNumbers.patternUSA;                default:
                    return null;            }        }










        /// <summary>        /// Return an array of phone numbers        /// </summary>        /// <param name="phoneNumbers"></param>        /// <returns></returns>        public List<string> GetPhoneNumbers(string phoneNumbers)        {            List<string> results = new List<string>();            if (phoneNumbers.Contains(","))            {                var pn = phoneNumbers.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);                foreach (var p in pn)                {                    GetPhoneNumbersAlternate(p, results);                }            }            else            {
                //legacy code with no phone number delimiter
                var pn = StringHelper.ExstractDigits(phoneNumbers);
                //check if there are groups of 10-digits numbers
                if ((pn.Length % PhoneNumberDigitCount) == 0)                {                    int startIndex = 0;                    int count = pn.Length / PhoneNumberDigitCount;                    for (int i = 0; i < count; i++)                    {                        results.Add(pn.Substring(startIndex, PhoneNumberDigitCount));                        startIndex += PhoneNumberDigitCount;                    }                }                if (!results.Any())                    GetPhoneNumbersAlternate1(phoneNumbers, results);            }            var phoneNumsWithValidCodes = new List<string>();            foreach (var result in results)            {                if (IsValidCode(result))                    phoneNumsWithValidCodes.Add(result);            }            return phoneNumsWithValidCodes;        }










        /// <summary>        /// Return an array of phone numbers        /// </summary>        /// <param name="phoneNumbers"></param>        /// <returns></returns>        private void GetPhoneNumbersAlternate(string phoneNumbers, List<string> results)        {            var pn = StringHelper.ExstractDigits(phoneNumbers);            var match = Regex.Match(pn, "^[0-9]{10}$");            if (match.Success)            {                results.Add(match.Value);            }            else            {                GetPhoneNumbersAlternate1(phoneNumbers, results);            }        }










        /// <summary>        /// Return an array of phone numbers        /// </summary>        /// <param name="phoneNumbers"></param>        /// <returns></returns>        private void GetPhoneNumbersAlternate1(string phoneNumbers, List<string> results)        {            var pn = StringHelper.ExstractDigitsForPhoneNumber(phoneNumbers);            var matches = Regex.Matches(pn, @"(?<=\+38)[0-9]{10}");            if (matches.Count > 0)            {                foreach (Match match in matches)                {                    results.Add(match.Value);                }            }            else            {                GetPhoneNumbersAlternate2(phoneNumbers, results);            }        }










        /// <summary>        /// Return an array of phone numbers        /// </summary>        /// <param name="phoneNumbers"></param>        /// <returns></returns>        private void GetPhoneNumbersAlternate2(string phoneNumbers, List<string> results)        {            var pn = StringHelper.ExstractDigitsForPhoneNumber(phoneNumbers);            var match = Regex.Match(pn, @"(?<=^38)[0-9]{10}$");            if (match.Success)            {                results.Add(match.Value);            }            else            {                GetPhoneNumbersAlternate3(phoneNumbers, results);            }        }

        /// <summary>        /// Return an array of phone numbers        /// </summary>        /// <param name="phoneNumbers"></param>        /// <returns></returns>        private void GetPhoneNumbersAlternate3(string phoneNumbers, List<string> results)        {            var pn = StringHelper.ExstractDigitsForPhoneNumber(phoneNumbers);            var match = Regex.Match(pn, @"(?<=^8)[0-9]{10}$");            if (match.Success)            {                results.Add(match.Value);            }        }    }}