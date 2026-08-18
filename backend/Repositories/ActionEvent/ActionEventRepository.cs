using Microsoft.EntityFrameworkCore;
using nba_mvc.Data;

namespace nba_mvc.Repositories.ActionEvent
{
    public class ActionEventRepository : IActionEventRepository
    {
        private readonly ApplicationDbContext _context;

        public ActionEventRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Models.ActionEvent?> GetByIdAsync(Guid id)
        {
            return await _context.ActionEvent
                .Include(ae => ae.Player)
                .Include(ae => ae.Team)
                .FirstOrDefaultAsync(ae => ae.Id == id);
        }

        public async Task<IEnumerable<Models.ActionEvent>> GetByGameIdAsync(Guid gameId)
        {
            return await _context.ActionEvent
                .Include(ae => ae.Player)
                .Include(ae => ae.Team)
                .Where(ae => ae.GameId == gameId)
                .OrderBy(ae => ae.Quarter)
                .ThenByDescending(ae => ae.GameTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Models.ActionEvent>> GetByGameAndPlayerIdAsync(Guid gameId, Guid playerId)
        {
            return await _context.ActionEvent
                .Where(ae => ae.GameId == gameId && ae.PlayerId == playerId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Models.ActionEvent>> GetByGameAndTeamIdAsync(Guid gameId, Guid teamId)
        {
            return await _context.ActionEvent
                .Where(ae => ae.GameId == gameId && ae.TeamId == teamId)
                .ToListAsync();
        }

        public async Task AddAsync(Models.ActionEvent actionEvent)
        {
            await _context.ActionEvent.AddAsync(actionEvent);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}