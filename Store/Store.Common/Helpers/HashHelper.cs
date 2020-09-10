using System;
using System.Security.Cryptography;

namespace Store.Common.Helpers
{
    public static class HashHelper
    {
        public static string GetSHA512Hash(string input)
        {
            HashAlgorithm hashAlgorithm = new SHA512CryptoServiceProvider();

            byte[] byteValue = System.Text.Encoding.UTF8.GetBytes(input);

            byte[] byteHash = hashAlgorithm.ComputeHash(byteValue);

            return Convert.ToBase64String(byteHash);
        }
    }
}