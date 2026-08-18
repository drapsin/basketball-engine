using nba_mvc.Models;

namespace nba_mvc.Repositories.Coach
{
    public interface ICoachRepository
    {
        Task<Models.Coach?> GetByIdAsync(Guid id);
        Task<IEnumerable<Models.Coach>> GetAllAsync();
        Task<Models.Coach?> GetByTeamIdAsync(Guid teamId);
        Task AddAsync(Models.Coach coach);
        void Update(Models.Coach coach);
        void Delete(Models.Coach coach);
        Task<bool> SaveChangesAsync();
    }
}