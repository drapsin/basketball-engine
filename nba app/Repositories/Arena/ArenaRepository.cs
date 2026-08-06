using Microsoft.EntityFrameworkCore;
using nba_mvc.Data;

namespace nba_mvc.Repositories.Arena
{
    public class ArenaRepository : IArenaRepository
    {
        private readonly ApplicationDbContext _context;

        public ArenaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Models.Arena?> GetByIdAsync(Guid id)
        {
            return await _context.Arena.FindAsync(id);
        }

        public async Task<IEnumerable<Models.Arena>> GetAllAsync()
        {
            return await _context.Arena.ToListAsync();
        }

        public async Task AddAsync(Models.Arena arena)
        {
            await _context.Arena.AddAsync(arena);
        }

        public void Update(Models.Arena arena)
        {
            _context.Arena.Update(arena);
        }

        public void Delete(Models.Arena arena)
        {
            _context.Arena.Remove(arena);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}