using nba_mvc.Models;

namespace nba_mvc.Repositories.Arena
{
    public interface IArenaRepository
    {
        Task<Models.Arena?> GetByIdAsync(Guid id);
        Task<IEnumerable<Models.Arena>> GetAllAsync();
        Task AddAsync(Models.Arena arena);
        void Update(Models.Arena arena);
        void Delete(Models.Arena arena);
        Task<bool> SaveChangesAsync();
    }
}