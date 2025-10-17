using backend.Models;
using backend.Repositories;
using backend.Services;
using backend.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Google.Apis.Auth;
using BCrypt.Net;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserRepository _userRepo;
        private readonly JwtService _jwtService;

        public AuthController(UserRepository userRepo, JwtService jwtService)
        {
            _userRepo = userRepo;
            _jwtService = jwtService;
        }

        // Email-based Registration
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var existing = await _userRepo.GetByEmailAsync(request.Email);
            if (existing != null)
                return BadRequest("Email already exists.");

            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = request.Role,
                ProfileImage = request.ProfileImageUrl,
                Bio = request.Bio,
                WalletBalance = 0,
                Points = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _userRepo.CreateAsync(user);
            return Ok("Registration successful!");
        }

        // Email-based Login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userRepo.GetByEmailAsync(request.Email);
            if (user == null || user.PasswordHash == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return Unauthorized("Invalid credentials.");

            var token = _jwtService.GenerateToken(user);
            return Ok(new { token, role = user.Role, name = user.Name });
        }

        // Google Login (OAuth callback simulation)
        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            try
            {
                // 1️⃣ Verify the Google ID Token
                var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken);

                // 2️⃣ Check if user already exists
                var existingUser = await _userRepo.GetByGoogleIdAsync(payload.Subject)
                                ?? await _userRepo.GetByEmailAsync(payload.Email);

                // 3️⃣ If not exists, register a new one
                if (existingUser == null)
                {
                    var newUser = new User
                    {
                        Name = payload.Name ?? payload.Email.Split('@')[0],
                        Email = payload.Email,
                        GoogleId = payload.Subject,
                        ProfileImage = payload.Picture,
                        Role = "Customer",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    await _userRepo.CreateAsync(newUser);
                    existingUser = newUser;
                }
                else if (existingUser.GoogleId == null)
                {
                    // Link Google ID to existing email account if not already linked
                    existingUser.GoogleId = payload.Subject;
                    await _userRepo.UpdateAsync(existingUser);
                }

                // 4️⃣ Generate JWT for your app
                var token = _jwtService.GenerateToken(existingUser);
                return Ok(new { token, role = existingUser.Role, name = existingUser.Name, email = existingUser.Email });
            }
            catch (InvalidJwtException)
            {
                return Unauthorized("Invalid Google token.");
            }
        }

        // Update Profile
        [HttpPut("update-profile/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile(string id, [FromBody] ProfileUpdateRequest update)
        {
            var user = await _userRepo.GetByIdAsync(id);
            if (user == null)
                return NotFound("User not found.");

            if (update.Name != null) user.Name = update.Name;
            if (update.Bio != null) user.Bio = update.Bio;
            if (update.ProfileImage != null) user.ProfileImage = update.ProfileImage;
            if (update.NotificationEnabled.HasValue) user.NotificationEnabled = update.NotificationEnabled.Value;
            if (update.PreferredCategories != null) user.PreferredCategories = update.PreferredCategories;

            await _userRepo.UpdateAsync(user);
            return Ok("Profile updated successfully!");
        }
    }
}
