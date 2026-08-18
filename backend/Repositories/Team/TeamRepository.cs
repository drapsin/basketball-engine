using Microsoft.EntityFrameworkCore;
using nba_mvc.Data;
using nba_mvc.Models;

namespace nba_mvc.Repositories.Team
{
    public class TeamRepository : ITeamRepository
    {
        private readonly ApplicationDbContext _context;

        public TeamRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Models.Team?> GetByIdAsync(Guid id)
        {
            return await _context.Team
                .Include(t => t.Arena)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Models.Team?> GetByIdWithPlayersAsync(Guid id)
        {
            return await _context.Team
                .Include(t => t.Arena)
                .Include(t => t.Players)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<Models.Team>> GetAllAsync()
        {
            return await _context.Team
                .Include(t => t.Arena)
                .Include(t => t.Players)
                .ToListAsync();
        }

        public async Task<IEnumerable<Models.Team>> GetByConferenceAsync(Conference conference)
        {
            return await _context.Team
                .Include(t => t.Arena)
                .Where(t => t.Conference == conference)
                .ToListAsync();
        }

        public async Task AddAsync(Models.Team team)
        {
            await _context.Team.AddAsync(team);
        }

        public void Update(Models.Team team)
        {
            _context.Team.Update(team);
        }

        public void Delete(Models.Team team)
        {
            _context.Team.Remove(team);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}