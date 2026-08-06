using nba_mvc.Dtos.Arena;

namespace nba_mvc.Services.Arena
{
    public interface IArenaService
    {
        Task<ArenaDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<ArenaDto>> GetAllAsync();
        Task<ArenaDto> CreateAsync(ArenaCreateDto dto);
        Task<bool> UpdateAsync(Guid id, ArenaUpdateDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}