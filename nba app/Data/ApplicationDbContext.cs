using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using nba_mvc.Models;

namespace nba_mvc.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Team> Team { get; set; } = default!;
        public DbSet<Player> Player { get; set; } = default!;
        public DbSet<Arena> Arena { get; set; } = default!;
        public DbSet<Coach> Coach { get; set; } = default!;
        public DbSet<Referee> Referee { get; set; } = default!;
        public DbSet<Game> Game { get; set; } = default!;
        public DbSet<ActionEvent> ActionEvent { get; set; } = default!;
    }
}