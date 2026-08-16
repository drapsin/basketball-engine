using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using nba_mvc.Dtos.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace nba_mvc.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _config;

        public AuthService(UserManager<IdentityUser> userManager, IConfiguration config)
        {
            _userManager = userManager;
            _config = config;
        }

        public async Task<AuthResultDto?> RegisterAsync(RegisterDto dto)
        {
            var validRoles = new[] { "Admin", "Manager"};
            if (!validRoles.Contains(dto.Role)) return null;

            var user = new IdentityUser { UserName = dto.Email, Email = dto.Email };
            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded) return null;

            await _userManager.AddToRoleAsync(user, dto.Role);

            return GenerateToken(user, dto.Role);
        }

        public async Task<AuthResultDto?> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user is null) return null;

            var validPassword = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!validPassword) return null;

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "Viewer";

            return GenerateToken(user, role);
        }

        private AuthResultDto GenerateToken(IdentityUser user, string role)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Email, user.Email!),
                new(ClaimTypes.Role, role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiry = DateTime.UtcNow.AddHours(double.Parse(_config["Jwt:ExpiryInHours"]!));

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: expiry,
                signingCredentials: creds
            );

            return new AuthResultDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Email = user.Email!,
                Role = role,
                ExpiresAt = expiry
            };
        }
    }
}