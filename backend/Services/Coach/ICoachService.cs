using nba_mvc.Dtos.Coach;

namespace nba_mvc.Services.Coach
{
    public interface ICoachService
    {
        Task<CoachDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<CoachDto>> GetAllAsync();
        Task<CoachDto?> GetByTeamIdAsync(Guid teamId);
        Task<CoachDto> CreateAsync(CoachCreateDto dto);
        Task<bool> UpdateAsync(Guid id, CoachUpdateDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}