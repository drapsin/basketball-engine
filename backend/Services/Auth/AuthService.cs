using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using nba_mvc.Dtos.Auth;
using nba_mvc.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace nba_mvc.Services.Auth
{
    public class AuthService : IAuthService
    {
        private const string SeededAdminEmail = "admin@nba.com";

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;

        public AuthService(UserManager<ApplicationUser> userManager, IConfiguration config)
        {
            _userManager = userManager;
            _config = config;
        }

        public async Task<AuthResultDto?> RegisterAsync(RegisterDto dto)
        {
            // Public registration can only ever request the Manager role, it goes to Pending
            // approval, not immediate access. Admin accounts can only be created by an existing Admin.
            if (dto.Role != "Manager") return null;

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                ApprovalStatus = ApprovalStatus.Pending
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded) return null;

            await _userManager.AddToRoleAsync(user, "Manager");

            // No token means the account exists but cannot log in until an Admin approves it.
            return new AuthResultDto
            {
                Token = "",
                Email = user.Email!,
                Role = "PendingApproval",
                ExpiresAt = DateTime.UtcNow
            };
        }

        public async Task<AuthResultDto?> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user is null) return null;

            var validPassword = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!validPassword) return null;

            if (user.ApprovalStatus == ApprovalStatus.Pending)
                throw new InvalidOperationException("Your account is awaiting admin approval.");

            if (user.ApprovalStatus == ApprovalStatus.Rejected)
                throw new InvalidOperationException("Your registration request was rejected.");

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();
            if (role is null) return null;

            return GenerateToken(user, role);
        }

        public async Task<List<UserSummaryDto>> GetAllUsersAsync()
        {
            var users = _userManager.Users.ToList();
            var result = new List<UserSummaryDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                result.Add(new UserSummaryDto
                {
                    Id = user.Id,
                    Email = user.Email!,
                    Role = roles.FirstOrDefault() ?? "",
                    ApprovalStatus = user.ApprovalStatus.ToString(),
                    IsSeededAdmin = user.Email == SeededAdminEmail
                });
            }

            return result;
        }

        public async Task<bool> ApproveManagerAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return false;

            user.ApprovalStatus = ApprovalStatus.Approved;
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> RejectManagerAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return false;

            user.ApprovalStatus = ApprovalStatus.Rejected;
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<AuthResultDto?> CreateAdminAsync(RegisterDto dto)
        {
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                ApprovalStatus = ApprovalStatus.Approved
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded) return null;

            await _userManager.AddToRoleAsync(user, "Admin");

            return GenerateToken(user, "Admin");
        }

        public async Task<(bool success, string? error)> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return (false, "User not found.");

            if (user.Email == SeededAdminEmail)
                return (false, "The default administrator account cannot be deleted.");

            var result = await _userManager.DeleteAsync(user);
            return (result.Succeeded, result.Succeeded ? null : "Failed to delete user.");
        }

        private AuthResultDto GenerateToken(ApplicationUser user, string role)
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