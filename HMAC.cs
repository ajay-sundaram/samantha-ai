using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace samantha_ai
{
    public static class HMAC
    {
        public static string CalculateHash(string key, string dataToSign)
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            Byte[] secretBytes = encoding.GetBytes(key);

            HMACSHA1 hmac = new HMACSHA1(secretBytes);
            Byte[] dataBytes = encoding.GetBytes(dataToSign);

            Byte[] calcHash = hmac.ComputeHash(dataBytes);
          
            return ByteToString(calcHash).ToLower();
        }

        private static string ByteToString(byte[] buff)
        {
            string sbinary = "";

            for (int i = 0; i < buff.Length; i++)
            {
                sbinary += buff[i].ToString("X2"); // hex format
            }
            return (sbinary);
        }
    }
}
