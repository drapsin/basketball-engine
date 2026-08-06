using Microsoft.EntityFrameworkCore;
using nba_mvc.Data;

namespace nba_mvc.Repositories.Coach
{
    public class CoachRepository : ICoachRepository
    {
        private readonly ApplicationDbContext _context;

        public CoachRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Models.Coach?> GetByIdAsync(Guid id)
        {
            return await _context.Coach
                .Include(c => c.Team)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Models.Coach>> GetAllAsync()
        {
            return await _context.Coach
                .Include(c => c.Team)
                .ToListAsync();
        }

        public async Task<Models.Coach?> GetByTeamIdAsync(Guid teamId)
        {
            return await _context.Coach
                .Include(c => c.Team)
                .FirstOrDefaultAsync(c => c.TeamId == teamId);
        }

        public async Task AddAsync(Models.Coach coach)
        {
            await _context.Coach.AddAsync(coach);
        }

        public void Update(Models.Coach coach)
        {
            _context.Coach.Update(coach);
        }

        public void Delete(Models.Coach coach)
        {
            _context.Coach.Remove(coach);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}