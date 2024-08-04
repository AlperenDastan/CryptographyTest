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
            // Key Pair Generation
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
                rsa.ImportParameters(_publicKey); // Use the public key for encryption
                var data = Encoding.UTF8.GetBytes(plainText);
                var encryptedData = rsa.Encrypt(data, false); // Encrypt the data
                return Convert.ToBase64String(encryptedData); // Return encrypted data as a base64 string
            }
        }

        public static string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                throw new ArgumentException("cipherText");

            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.PersistKeyInCsp = false;
                rsa.ImportParameters(_privateKey); // Use the private key for decryption
                var data = Convert.FromBase64String(cipherText);
                var decryptedData = rsa.Decrypt(data, false); // Decrypt the data
                return Encoding.UTF8.GetString(decryptedData); // Return decrypted data as a string
            }
        }

        public static string GetPublicKey()
        {
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                return Convert.ToBase64String(rsa.ExportCspBlob(false)); // Export the public key as a base64 string
            }
        }
    }
}
