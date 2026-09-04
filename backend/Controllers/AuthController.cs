using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using nba_mvc.Dtos.Auth;
using nba_mvc.Services.Auth;

namespace nba_mvc.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);
            if (result is null)
                return BadRequest(new { message = "Registration failed. Only 'Manager' can be requested via public registration; check your email/password." });

            return Ok(new { message = "Registration request submitted. An administrator must approve your account before you can log in." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            try
            {
                var result = await _authService.LoginAsync(dto);
                if (result is null) return Unauthorized(new { message = "Invalid email or password." });
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
        }

        [HttpGet("users")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<UserSummaryDto>>> GetUsers()
        {
            var users = await _authService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpPost("users/{id}/approve")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approve(string id)
        {
            var success = await _authService.ApproveManagerAsync(id);
            if (!success) return NotFound();
            return Ok(new { message = "User approved." });
        }

        [HttpPost("users/{id}/reject")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Reject(string id)
        {
            var success = await _authService.RejectManagerAsync(id);
            if (!success) return NotFound();
            return Ok(new { message = "User rejected." });
        }

        [HttpPost("create-admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateAdmin(RegisterDto dto)
        {
            var result = await _authService.CreateAdminAsync(dto);
            if (result is null) return BadRequest(new { message = "Failed to create admin. Check email/password." });
            return Ok(result);
        }

        [HttpDelete("users/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var (success, error) = await _authService.DeleteUserAsync(id);
            if (!success) return BadRequest(new { message = error });
            return NoContent();
        }
    }
}