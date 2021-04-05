using System;
using System.Text;
using System.Security.Cryptography;

namespace M1.Api.Core
{
    public class TripleDESCrypto:ICryptoOperation
    {
        private readonly byte[] _staticIV = new byte[8] { 50, 185, 235, 15, 89, 137, 237, 221 };
        private readonly byte[] _staticBytes = new byte[32] { 3, 190, 200, 77, 251, 170, 10, 24, 192, 26, 21, 82, 188, 45, 48, 215, 249, 164, 21, 137, 3, 74, 247, 96, 22, 231, 92, 181, 131, 219, 44, 86 };
        private readonly byte[] _staticSalt = new byte[32] { 207, 189, 247, 233, 219, 180, 21, 206, 214, 212, 251, 132, 133, 83, 65, 132, 110, 106, 40, 154, 55, 222, 168, 69, 113, 92, 179, 247, 228, 105, 102, 136 };
        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentNullException(nameof(plainText));

            byte[] dataBytes = Encoding.UTF8.GetBytes(plainText);

            using (TripleDES des = TripleDES.Create())
            {
                byte[] keys = GetKeys();
                des.Key = keys;
                des.IV = _staticIV;
                using (ICryptoTransform cryptoTransform = des.CreateEncryptor())
                {
                    byte[] result = cryptoTransform.TransformFinalBlock(dataBytes, 0, dataBytes.Length);
                    return Convert.ToBase64String(result);
                }
            }
        }
        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                throw new ArgumentNullException(nameof(cipherText));
            byte[] dataBytes = Convert.FromBase64String(cipherText);
            using (TripleDES des = TripleDES.Create())
            {
                byte[] keys = GetKeys();
                des.Key = keys;
                des.IV = _staticIV;
                using (ICryptoTransform cryptoTransform = des.CreateDecryptor())
                {
                    byte[] result = cryptoTransform.TransformFinalBlock(dataBytes, 0, dataBytes.Length);
                    return Encoding.UTF8.GetString(result);
                }
            }
        }
        private byte[] GetKeys()
        {
            byte[] keys = new byte[24];
            using (Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(_staticBytes, _staticSalt, 5000))
            {
                keys = rfc2898DeriveBytes.GetBytes(24);
            }
            return keys;
        }
    }
}
