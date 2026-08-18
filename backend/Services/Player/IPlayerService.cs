using nba_mvc.Dtos.Player;

namespace nba_mvc.Services.Player
{
    public interface IPlayerService
    {
        Task<PlayerDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<PlayerDto>> GetAllAsync();
        Task<IEnumerable<PlayerDto>> GetByTeamIdAsync(Guid teamId);
        Task<PlayerDto> CreateAsync(PlayerCreateDto dto);
        Task<bool> UpdateAsync(Guid id, PlayerUpdateDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}