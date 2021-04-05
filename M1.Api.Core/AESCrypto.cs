using System;
using System.Text;
using System.Security.Cryptography;
namespace M1.Api.Core
{
    public class AESCrypto: ICryptoOperation
    {
        private readonly byte[] _staticIV = new byte[16] { 209, 175, 245, 8, 120, 35, 49, 245, 24, 123, 230, 101, 156, 193, 14, 159 };
        private readonly byte[] _staticBytes = new byte[32] { 152, 183, 231, 172, 44, 91, 26, 22, 28, 108, 209, 81, 225, 165, 247, 95, 81, 154, 16, 91, 190, 54, 195, 132, 102, 61, 80, 135, 125, 5, 67, 103 };
        private readonly byte[] _staticSalt = new byte[32] { 139, 121, 72, 136, 197, 86, 57, 242, 154, 153, 211, 78, 63, 185, 229, 85, 35, 130, 96, 131, 47, 79, 18, 255, 155, 16, 215, 224, 243, 239, 93, 129 };
        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentNullException(nameof(plainText));

            byte[] dataBytes = Encoding.UTF8.GetBytes(plainText);
            
            using(Aes aes = Aes.Create())
            {
                byte[] keys = GetKeys();
                aes.Key = keys;
                aes.IV = _staticIV;
                using(ICryptoTransform cryptoTransform = aes.CreateEncryptor())
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
            using (Aes aes = Aes.Create())
            {
                byte[] keys = GetKeys();
                aes.Key = keys;
                aes.IV = _staticIV;
                using (ICryptoTransform cryptoTransform = aes.CreateDecryptor())
                {
                    byte[] result = cryptoTransform.TransformFinalBlock(dataBytes, 0, dataBytes.Length);
                    return Encoding.UTF8.GetString(result);
                }
            }
        }
        private byte[] GetKeys()
        {
            byte[] keys = new byte[32];
            using (Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(_staticBytes, _staticSalt, 5000))
            {
                keys = rfc2898DeriveBytes.GetBytes(32);
            }
            return keys;
        }
    }
}
