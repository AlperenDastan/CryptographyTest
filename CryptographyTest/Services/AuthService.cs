using CryptographyTest.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace CryptographyTest.Services
{
    public class AuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RSA _privateKey;

        public AuthService(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _privateKey = new RSACryptoServiceProvider(2048);
        }

        public async Task<string> GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName), // Using Identity's UserName property
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // Identity's Id
                new Claim(ClaimTypes.Role, user.Role.ToString()) // Assuming you have Role set up
            };

            var signingKey = new RsaSecurityKey(_privateKey);
            var creds = new SigningCredentials(signingKey, SecurityAlgorithms.RsaSha256);

            var token = new JwtSecurityToken(
                issuer: "CryptographyTest",
                audience: "CryptographyTestAPI",
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<SignInResult> SignInAsync(string userName, string password, bool rememberMe = false)
        {
            var result = await _signInManager.PasswordSignInAsync(userName, password, rememberMe, lockoutOnFailure: false);
            return result;
        }

        public async Task<IdentityResult> RegisterAsync(User user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            return result;
        }

        public async Task<User> FindByNameAsync(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }
    }
}
