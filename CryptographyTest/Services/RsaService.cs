using Microsoft.IdentityModel.Tokens;
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
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var rsaSettings = configuration.GetSection("RSASettings");

            LoadKeysFromConfiguration(rsaSettings);
        }

        private static void LoadKeysFromConfiguration(IConfigurationSection rsaSettings)
        {
            try
            {
                using (var rsa = new RSACryptoServiceProvider(2048))
                {
                    rsa.PersistKeyInCsp = false;

                    // Retrieve keys from the configuration
                    var publicKeyXml = rsaSettings.GetValue<string>("PublicKey");
                    var privateKeyXml = rsaSettings.GetValue<string>("PrivateKey");

                    // Check if keys are missing
                    if (string.IsNullOrEmpty(publicKeyXml) || string.IsNullOrEmpty(privateKeyXml))
                    {
                        throw new InvalidOperationException("RSA keys are not found in the configuration.");
                    }

                    // Load the public key
                    rsa.FromXmlString(publicKeyXml);
                    _publicKey = rsa.ExportParameters(false);

                    // Load the private key
                    rsa.FromXmlString(privateKeyXml);
                    _privateKey = rsa.ExportParameters(true);
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions as necessary (e.g., log the issue)
                throw new InvalidOperationException("Failed to load RSA keys from configuration.", ex);
            }
        }
        //private static bool LoadKeys()
        //{
        //    try
        //    {
        //        if (File.Exists(PublicKeyPath) && File.Exists(PrivateKeyPath))
        //        {
        //            using (var rsa = new RSACryptoServiceProvider(2048))
        //            {
        //                rsa.PersistKeyInCsp = false;
        //                rsa.FromXmlString(File.ReadAllText(PublicKeyPath));
        //                _publicKey = rsa.ExportParameters(false);

        //                rsa.FromXmlString(File.ReadAllText(PrivateKeyPath));
        //                _privateKey = rsa.ExportParameters(true);
        //            }
        //            return true;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        // Handle any exceptions (e.g., log the issue)
        //    }
        //    return false;
        //}

        // to generate new keys
        //public static void GenerateAndSaveKeys()
        //{
        //    using (var rsa = new RSACryptoServiceProvider(2048))
        //    {
        //        rsa.PersistKeyInCsp = false;

        //       // Export the keys
        //        _publicKey = rsa.ExportParameters(false);
        //        _privateKey = rsa.ExportParameters(true);

        //        // Save the keys in XML format
        //        string publicKeyXml = rsa.ToXmlString(false);
        //        string privateKeyXml = rsa.ToXmlString(true);

        //        // Save the keys to XML files
        //        System.IO.File.WriteAllText("publicKey.xml", publicKeyXml);
        //        System.IO.File.WriteAllText("privateKey.xml", privateKeyXml);
        //    }
        //}


        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentException("plainText cannot be null or empty");

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
                throw new ArgumentException("cipherText cannot be null or empty");

            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.PersistKeyInCsp = false;
                rsa.ImportParameters(_privateKey);
                var data = Convert.FromBase64String(cipherText);
                var decryptedData = rsa.Decrypt(data, true);
                return Encoding.UTF8.GetString(decryptedData);
            }
        }
        public static RsaSecurityKey GetSigningKey()
        {
            var rsa = new RSACryptoServiceProvider(2048);
            rsa.ImportParameters(_privateKey);
            return new RsaSecurityKey(rsa);
        }

        public static RsaSecurityKey GetValidationKey()
        {
            var rsa = new RSACryptoServiceProvider(2048);
            rsa.ImportParameters(_publicKey);
            return new RsaSecurityKey(rsa);
        }
    }
}