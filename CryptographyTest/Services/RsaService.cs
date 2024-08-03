using System.Security.Cryptography;
using System.Text;

namespace CryptographyTest.Services
{
    public static class RsaService
    {
        private static RSAParameters _publicKey;
        private static RSAParameters _privateKey;

        static RsaService()
        {
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.PersistKeyInCsp = false; // Do not persist the keys in a container
                _publicKey = rsa.ExportParameters(false); // Export only the public key
                _privateKey = rsa.ExportParameters(true); // Export both public and private key
            }
        }

        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentException("plainText");

            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.PersistKeyInCsp = false;
                rsa.ImportParameters(_publicKey);
                var data = Encoding.UTF8.GetBytes(plainText);
                var encryptedData = rsa.Encrypt(data, false);
                return Convert.ToBase64String(encryptedData);
            }
        }

        public static string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                throw new ArgumentException("cipherText");

            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.PersistKeyInCsp = false;
                rsa.ImportParameters(_privateKey);
                var data = Convert.FromBase64String(cipherText);
                var decryptedData = rsa.Decrypt(data, false);
                return Encoding.UTF8.GetString(decryptedData);
            }
        }

        public static string GetPublicKey()
        {
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                return Convert.ToBase64String(rsa.ExportCspBlob(false));
            }
        }
    }
}