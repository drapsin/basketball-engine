using nba_mvc.Models;

namespace nba_mvc.Repositories.ActionEvent
{
    public interface IActionEventRepository
    {
        Task<Models.ActionEvent?> GetByIdAsync(Guid id);
        Task<IEnumerable<Models.ActionEvent>> GetByGameIdAsync(Guid gameId);
        Task<IEnumerable<Models.ActionEvent>> GetByGameAndPlayerIdAsync(Guid gameId, Guid playerId);
        Task<IEnumerable<Models.ActionEvent>> GetByGameAndTeamIdAsync(Guid gameId, Guid teamId);
        Task AddAsync(Models.ActionEvent actionEvent);
        Task<bool> SaveChangesAsync();
    }
}