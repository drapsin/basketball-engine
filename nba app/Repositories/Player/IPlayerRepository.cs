using nba_mvc.Models;

namespace nba_mvc.Repositories.Player
{
    public interface IPlayerRepository
    {
        Task<Models.Player?> GetByIdAsync(Guid id);
        Task<IEnumerable<Models.Player>> GetAllAsync();
        Task<IEnumerable<Models.Player>> GetByTeamIdAsync(Guid teamId);
        Task AddAsync(Models.Player player);
        void Update(Models.Player player);
        void Delete(Models.Player player);
        Task<bool> SaveChangesAsync();
    }
}