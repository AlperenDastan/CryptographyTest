using System.Security.Cryptography;
using System.Text;

namespace CryptographyTest.Services
{
    public static class RsaService
    {
        private static readonly string PublicKeyPath = "publicKey.xml";
        private static readonly string PrivateKeyPath = "privateKey.xml";
        private static RSAParameters _publicKey;
        private static RSAParameters _privateKey;

        static RsaService()
        {
            if (!LoadKeys())
            {
                GenerateAndSaveKeys();
            }
        }

        private static bool LoadKeys()
        {
            try
            {
                if (File.Exists(PublicKeyPath) && File.Exists(PrivateKeyPath))
                {
                    using (var rsa = new RSACryptoServiceProvider(2048))
                    {
                        rsa.PersistKeyInCsp = false;
                        rsa.FromXmlString(File.ReadAllText(PublicKeyPath));
                        _publicKey = rsa.ExportParameters(false);

                        rsa.FromXmlString(File.ReadAllText(PrivateKeyPath));
                        _privateKey = rsa.ExportParameters(true);
                    }
                    return true;
                }
            }
            catch (Exception)
            {
                // Handle any exceptions (e.g., log the issue)
            }
            return false;
        }

        private static void GenerateAndSaveKeys()
        {
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.PersistKeyInCsp = false;
                _publicKey = rsa.ExportParameters(false);
                _privateKey = rsa.ExportParameters(true);

                File.WriteAllText(PublicKeyPath, rsa.ToXmlString(false)); // Save public key
                File.WriteAllText(PrivateKeyPath, rsa.ToXmlString(true));  // Save private key
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
                var encryptedData = rsa.Encrypt(data, true);
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
                var decryptedData = rsa.Decrypt(data, true);
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