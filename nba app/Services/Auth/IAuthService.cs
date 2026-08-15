using nba_mvc.Dtos.Auth;

namespace nba_mvc.Services.Auth
{
    public interface IAuthService
    {
        Task<AuthResultDto?> RegisterAsync(RegisterDto dto);
        Task<AuthResultDto?> LoginAsync(LoginDto dto);
    }
}