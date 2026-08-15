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
            if (result is null) return BadRequest(new { message = "Registration failed. Check email/password/role." });
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            if (result is null) return Unauthorized(new { message = "Invalid email or password." });
            return Ok(result);
        }
    }
}