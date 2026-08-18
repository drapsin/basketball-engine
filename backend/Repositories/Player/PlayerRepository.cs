using Microsoft.EntityFrameworkCore;
using nba_mvc.Data;

namespace nba_mvc.Repositories.Player
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly ApplicationDbContext _context;

        public PlayerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Models.Player?> GetByIdAsync(Guid id)
        {
            return await _context.Player
                .Include(p => p.Team)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Models.Player>> GetAllAsync()
        {
            return await _context.Player
                .Include(p => p.Team)
                .ToListAsync();
        }

        public async Task<IEnumerable<Models.Player>> GetByTeamIdAsync(Guid teamId)
        {
            return await _context.Player
                .Include(p => p.Team)
                .Where(p => p.TeamId == teamId)
                .ToListAsync();
        }

        public async Task AddAsync(Models.Player player)
        {
            await _context.Player.AddAsync(player);
        }

        public void Update(Models.Player player)
        {
            _context.Player.Update(player);
        }

        public void Delete(Models.Player player)
        {
            _context.Player.Remove(player);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<List<Models.Player>> GetByIdsAsync(IEnumerable<Guid> ids)
        {
            return await _context.Player.Where(p => ids.Contains(p.Id)).ToListAsync();
        }
    }
}