using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public static class StringExtensions
    {
        public static string Md5(this string source)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.
            var md5Hasher = System.Security.Cryptography.MD5.Create();

            // Convert the input string to a byte array and compute the hash.
            var data = md5Hasher.ComputeHash(System.Text.Encoding.Default.GetBytes(source));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            var sBuilder = new System.Text.StringBuilder();
            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        public static int TryParseInt32(this string source)
        {
            var result = 0;
            if (int.TryParse(source, out result))
                return result;
            return 0;
        }
    }
}
