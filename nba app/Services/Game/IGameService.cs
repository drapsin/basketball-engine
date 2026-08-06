using nba_mvc.Dtos.Game;

namespace nba_mvc.Services.Game
{
    public interface IGameService
    {
        Task<GameDto?> GetByIdAsync(Guid id);
        Task<GameDetailDto?> GetDetailByIdAsync(Guid id);
        Task<IEnumerable<GameDto>> GetAllAsync();
        Task<IEnumerable<GameDto>> GetByTeamIdAsync(Guid teamId);
        Task<GameDetailDto?> CreateAsync(GameCreateDto dto);
        Task<bool> UpdateAsync(Guid id, GameUpdateDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}