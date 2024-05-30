using System.Security.Cryptography;
using System.Text;

namespace CryptographyTest.Services
{

    public static class HashingService
    {
        private const string Salt = "12345678";  // Static salt for simplicity, consider a unique salt per user

        public static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Password cannot be null or empty.");
            }

            // Combine the password with the salt
            var saltedPassword = string.Concat(password, Salt);

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

        public static bool VerifyPassword(string enteredPassword, string storedHash)
        {
            // Hash the entered password
            var hashOfEnteredPassword = HashPassword(enteredPassword);
            // Check if the hashes match
            return hashOfEnteredPassword == storedHash;
        }
    }

}
