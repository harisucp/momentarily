using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
namespace Apeek.Entities.Validators
{
    public abstract class ValidatorBase : ValidationAttribute
    {
        public static bool Identity(int value)
        {
            return value > 0;
        }
        public static bool Identity(IEnumerable<int> values)
        {
            var enumerable = values as int[] ?? values.ToArray();
            return enumerable.Any() && enumerable.All(x => x > 0);
        }
        public static bool String(string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }
        public static bool DateTime(DateTime? value)
        {
            return (value.HasValue) && (value.Value > System.DateTime.MinValue);
        }
    }
}