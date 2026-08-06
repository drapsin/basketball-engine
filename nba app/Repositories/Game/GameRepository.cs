using Microsoft.EntityFrameworkCore;
using nba_mvc.Data;

namespace nba_mvc.Repositories.Game
{
    public class GameRepository : IGameRepository
    {
        private readonly ApplicationDbContext _context;

        public GameRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Models.Game?> GetByIdAsync(Guid id)
        {
            return await _context.Game
                .Include(g => g.HomeTeam)
                .Include(g => g.AwayTeam)
                .Include(g => g.Arena)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<Models.Game?> GetByIdWithDetailsAsync(Guid id)
        {
            return await _context.Game
                .Include(g => g.HomeTeam)
                .Include(g => g.AwayTeam)
                .Include(g => g.Arena)
                .Include(g => g.Referees)
                .Include(g => g.Players)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<IEnumerable<Models.Game>> GetAllAsync()
        {
            return await _context.Game
                .Include(g => g.HomeTeam)
                .Include(g => g.AwayTeam)
                .Include(g => g.Arena)
                .ToListAsync();
        }

        public async Task<IEnumerable<Models.Game>> GetByTeamIdAsync(Guid teamId)
        {
            return await _context.Game
                .Include(g => g.HomeTeam)
                .Include(g => g.AwayTeam)
                .Include(g => g.Arena)
                .Where(g => g.HomeTeamId == teamId || g.AwayTeamId == teamId)
                .ToListAsync();
        }

        public async Task AddAsync(Models.Game game)
        {
            await _context.Game.AddAsync(game);
        }

        public void Update(Models.Game game)
        {
            _context.Game.Update(game);
        }

        public void Delete(Models.Game game)
        {
            _context.Game.Remove(game);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}