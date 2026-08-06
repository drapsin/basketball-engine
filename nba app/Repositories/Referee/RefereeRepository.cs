using Microsoft.EntityFrameworkCore;
using nba_mvc.Data;

namespace nba_mvc.Repositories.Referee
{
    public class RefereeRepository : IRefereeRepository
    {
        private readonly ApplicationDbContext _context;

        public RefereeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Models.Referee?> GetByIdAsync(Guid id)
        {
            return await _context.Referee.FindAsync(id);
        }

        public async Task<IEnumerable<Models.Referee>> GetAllAsync()
        {
            return await _context.Referee.ToListAsync();
        }

        public async Task AddAsync(Models.Referee referee)
        {
            await _context.Referee.AddAsync(referee);
        }

        public void Update(Models.Referee referee)
        {
            _context.Referee.Update(referee);
        }

        public void Delete(Models.Referee referee)
        {
            _context.Referee.Remove(referee);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}