using nba_mvc.Models;

namespace nba_mvc.Repositories.Referee
{
    public interface IRefereeRepository
    {
        Task<Models.Referee?> GetByIdAsync(Guid id);
        Task<IEnumerable<Models.Referee>> GetAllAsync();
        Task AddAsync(Models.Referee referee);
        void Update(Models.Referee referee);
        void Delete(Models.Referee referee);
        Task<bool> SaveChangesAsync();
        Task<List<Models.Referee>> GetByIdsAsync(IEnumerable<Guid> ids);
    }
}