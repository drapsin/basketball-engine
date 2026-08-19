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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ActionEvent>()
                .HasOne(ae => ae.Game)
                .WithMany(g => g.ActionEvents)
                .HasForeignKey(ae => ae.GameId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ActionEvent>()
                .HasOne(ae => ae.Player)
                .WithMany()
                .HasForeignKey(ae => ae.PlayerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ActionEvent>()
                .HasOne(ae => ae.Team)
                .WithMany()
                .HasForeignKey(ae => ae.TeamId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Game>()
                .HasOne(g => g.HomeTeam)
                .WithMany()
                .HasForeignKey(g => g.HomeTeamId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Game>()
                .HasOne(g => g.AwayTeam)
                .WithMany()
                .HasForeignKey(g => g.AwayTeamId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Game>()
                .HasOne(g => g.Arena)
                .WithMany()
                .HasForeignKey(g => g.ArenaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Game>()
                .HasMany(g => g.Referees)
                .WithMany(r => r.Games);

            modelBuilder.Entity<Game>()
                .HasMany(g => g.Players)
                .WithMany();
        }
    }
}