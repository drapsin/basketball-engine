using nba_mvc.Models;

namespace nba_mvc.Repositories.Game
{
    public interface IGameRepository
    {
        Task<Models.Game?> GetByIdAsync(Guid id);
        Task<Models.Game?> GetByIdWithDetailsAsync(Guid id);
        Task<IEnumerable<Models.Game>> GetAllAsync();
        Task<IEnumerable<Models.Game>> GetByTeamIdAsync(Guid teamId);
        Task AddAsync(Models.Game game);
        void Update(Models.Game game);
        void Delete(Models.Game game);
        Task<bool> SaveChangesAsync();
    }
}