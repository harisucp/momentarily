using System;
using System.Security.Cryptography;
using System.Text;
namespace Apeek.Common.Encription
{
    public class Md5Hash
    {
        public static string GetMd5Hash(string input)
        {
            byte[] originalBytes;
            byte[] encodedBytes;
            MD5 md5 = new MD5CryptoServiceProvider();
            originalBytes = ASCIIEncoding.Default.GetBytes(input);
            encodedBytes = md5.ComputeHash(originalBytes);
            return BitConverter.ToString(encodedBytes).Replace("-", string.Empty);
        } 
    }
}