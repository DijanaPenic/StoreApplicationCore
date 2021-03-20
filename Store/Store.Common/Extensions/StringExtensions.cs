using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Store.Common.Extensions
{
    public static class StringExtensions
    {

        public static string GetCommaSeparated(this string[] input)
        {
            return string.Join(',', input);
        }

        public static string GetDigits(this string input)
        {
            return new string(input.Where(c => char.IsDigit(c)).ToArray());
        }

        public static string ToSnakeCase(this string input)
        {
            if (string.IsNullOrEmpty(input)) { return input; }

            Match startUnderscores = Regex.Match(input, @"^_+");

            return startUnderscores + Regex.Replace(input, @"([a-z0-9])([A-Z])", "$1_$2").ToLower();
        }

        public static string ToPascalCase(this string input)
        {
            input = input.ToLower().Replace("_", string.Empty);
            
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input);
        }

        public static string Base64ForUrlEncode(this string value)
        {
            byte[] encodedBytes = Encoding.UTF8.GetBytes(value);

            return Convert.ToBase64String(encodedBytes);
        }

        public static string Base64ForUrlDecode(this string value)
        {
            byte[] decodedBytes = Convert.FromBase64String(value);

            return Encoding.UTF8.GetString(decodedBytes);
        }

        public static string FirstCharToUpper(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            return value.First().ToString().ToUpper() + value[1..];
        }
    }
}