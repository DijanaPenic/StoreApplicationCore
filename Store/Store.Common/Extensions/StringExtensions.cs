﻿using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Store.Common.Extensions
{
    public static class StringExtensions
    {
        public static string GetDigits(this string input)
        {
            return new string(input.Where(char.IsDigit).ToArray());
        }

        public static string ToSnakeCase(this string input)
        {
            if (string.IsNullOrEmpty(input)) { return input; }

            Match startUnderscores = Regex.Match(input, @"^_+");

            return startUnderscores + Regex.Replace(input, @"([a-z0-9])([A-Z])", "$1_$2").ToLower();
        }

        public static string ToPascalCase(this string input)
        {
            if (!input.Contains(" "))
            {
                input = Regex.Replace(input, "(?<=[a-z])(?=[A-Z])", " ");
            }

            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.ToLower()).Replace(" ", "").Replace("_", "");
        }

        public static string Base64Encode(this string value)
        {
            byte[] encodedBytes = Encoding.UTF8.GetBytes(value);

            return Convert.ToBase64String(encodedBytes);
        }

        public static string Base64Decode(this string value)
        {
            byte[] decodedBytes = Convert.FromBase64String(value);

            return Encoding.UTF8.GetString(decodedBytes);
        }
        
        public static bool IsBase64String(this string value)
        {
            Span<byte> buffer = new(new byte[value.Length]);
            
            return Convert.TryFromBase64String(value, buffer, out int _);
        }
    }
}