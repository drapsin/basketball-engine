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

        public async Task<(List<Models.Player> Items, int TotalCount)> GetPagedAsync(
            string? search,
            Guid? teamId,
            string? position,
            int page,
            int pageSize)
        {
            var query = _context.Player.Include(p => p.Team).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                query = query.Where(p =>
                    p.FirstName.ToLower().Contains(term) ||
                    p.LastName.ToLower().Contains(term));
            }

            if (teamId.HasValue)
            {
                query = query.Where(p => p.TeamId == teamId.Value);
            }

            if (!string.IsNullOrWhiteSpace(position))
            {
                query = query.Where(p => p.Position == position);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(p => p.LastName)
                .ThenBy(p => p.FirstName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
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