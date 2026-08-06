using nba_mvc.Models;

namespace nba_mvc.Repositories.Team
{
    public interface ITeamRepository
    {
        Task<Models.Team?> GetByIdAsync(Guid id);
        Task<Models.Team?> GetByIdWithPlayersAsync(Guid id);
        Task<IEnumerable<Models.Team>> GetAllAsync();
        Task<IEnumerable<Models.Team>> GetByConferenceAsync(Conference conference);
        Task AddAsync(Models.Team team);
        void Update(Models.Team team);
        void Delete(Models.Team team);
        Task<bool> SaveChangesAsync();
    }
}