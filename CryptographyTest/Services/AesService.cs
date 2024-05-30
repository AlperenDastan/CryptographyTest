using System.Security.Cryptography;
using System;
using System.IO;
using System.Text;
namespace CryptographyTest.Services
{

    public static class AesService
    {
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("12345678901234567890123456789012");  // AES-256 key
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("1234567890123456");  // IV for AES


        public static string Encrypt<T>(T plainText)
        {
            if (plainText == null) throw new ArgumentNullException(nameof(plainText));

            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        public static string Decrypt(string cipherText)
        {
            if (cipherText == null) throw new ArgumentNullException(nameof(cipherText));

            var cipherTextBytes = Convert.FromBase64String(cipherText);

            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (var msDecrypt = new MemoryStream(cipherTextBytes))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
    }


}
