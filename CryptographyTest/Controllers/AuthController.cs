using CryptographyTest.Models;
using CryptographyTest.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CryptographyTest.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly UserManager<User> _userManager;

        public AuthController(AuthService authService, UserManager<User> userManager)
        {
            _authService = authService;
            _userManager = userManager;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var token = await _authService.GenerateJwtToken(user);
                // Populate other details as necessary
                return Ok(new AuthResponse { Token = token, Role = user.Role.ToString(), UserId = user.Id, Username = user.UserName });
            }

            return BadRequest("Invalid login attempt.");
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var user = new User { UserName = model.Username, Email = model.Email, BadgeNumber = model.BadgeNumber, Role = model.Role };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return Ok("User registered successfully.");
            }

            return BadRequest(result.Errors);
        }
    }

    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class RegisterModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string BadgeNumber { get; set; }
        public string Password { get; set; }
        public UserRole Role { get; set; }
    }
    public class AuthResponse
    {
        public string Token { get; set; }
        public string Role { get; set; } // Assuming role management is handled elsewhere
        public Guid UserId { get; set; } // Assuming user ID can be fetched or stored
        public string Username { get; set; }
    }
}

