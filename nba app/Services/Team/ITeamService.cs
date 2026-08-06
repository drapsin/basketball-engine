using nba_mvc.Dtos.Team;
using nba_mvc.Models;

namespace nba_mvc.Services.Team
{
    public interface ITeamService
    {
        Task<TeamDto?> GetByIdAsync(Guid id);
        Task<TeamDetailDto?> GetDetailByIdAsync(Guid id);
        Task<IEnumerable<TeamDto>> GetAllAsync();
        Task<IEnumerable<TeamDto>> GetByConferenceAsync(Conference conference);
        Task<TeamDto> CreateAsync(TeamCreateDto dto);
        Task<bool> UpdateAsync(Guid id, TeamUpdateDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}