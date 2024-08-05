using System.Security.Cryptography;
using System.Text;

namespace CryptographyTest.Services
{
    public static class HashingService
    {
        private const int SaltSize = 16; // 128-bit salt

        public static string GenerateSalt()
        {
            // Generate a cryptographically strong random salt
            using (var rng = new RNGCryptoServiceProvider())
            {
                var saltBytes = new byte[SaltSize];
                rng.GetBytes(saltBytes);
                return Convert.ToBase64String(saltBytes);
            }
        }

        public static string HashPassword(string password, string salt)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Password cannot be null or empty.");
            }

            // Combine the password with the salt
            var saltedPassword = string.Concat(password, salt);

            using (var sha256 = SHA256.Create())
            {
                // Convert the salted password to a byte array
                var saltedPasswordAsBytes = Encoding.UTF8.GetBytes(saltedPassword);
                // Compute the hash
                var hashBytes = sha256.ComputeHash(saltedPasswordAsBytes);
                // Convert back to a base64 string (to store in DB, for example)
                return Convert.ToBase64String(hashBytes);
            }
        }

        public static bool VerifyPassword(string enteredPassword, string storedHash, string storedSalt)
        {
            // Hash the entered password with the stored salt
            var hashOfEnteredPassword = HashPassword(enteredPassword, storedSalt);
            // Check if the hashes match
            return hashOfEnteredPassword == storedHash;
        }
    }
}
