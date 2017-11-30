using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Models
{
    public class HashEncryptor
    {
        public static string EncryptWithMD5(string value)
        {
            return Encrypt(MD5.Create(), Encoding.ASCII, value);
        }
        public static string Encrypt(HashAlgorithm algorithm, Encoding encoding, string valueToEncrypt)
        {
            var inputBytes = encoding.GetBytes(valueToEncrypt);

            var hash = algorithm.ComputeHash(inputBytes);

            var sb = new StringBuilder();

            foreach (var t in hash)
            {
                sb.Append(t.ToString("X2"));
            }

            return sb.ToString();
        }
        public static string ComputeHash(string stringToEncrypt)
        {
            return EncryptWithMD5(stringToEncrypt);
        }
    }
}
