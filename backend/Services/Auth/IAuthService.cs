using nba_mvc.Dtos.Auth;

namespace nba_mvc.Services.Auth
{
    public interface IAuthService
    {
        Task<AuthResultDto?> RegisterAsync(RegisterDto dto);
        Task<AuthResultDto?> LoginAsync(LoginDto dto);
        Task<List<UserSummaryDto>> GetAllUsersAsync();
        Task<bool> ApproveManagerAsync(string userId);
        Task<bool> RejectManagerAsync(string userId);
        Task<AuthResultDto?> CreateAdminAsync(RegisterDto dto);
        Task<(bool success, string? error)> DeleteUserAsync(string userId);
    }
}