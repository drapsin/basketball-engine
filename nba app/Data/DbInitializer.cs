using Microsoft.AspNetCore.Identity;
using nba_mvc.Models;

namespace nba_mvc.Data
{
    public static class DbInitializer
    {
        public static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = { "Admin", "Manager", "Viewer" };
            foreach (var role in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        public static void SeedData(ApplicationDbContext context)
        {
            var positions = new[] { "PG", "SG", "SF", "PF", "C" };
            var rnd = new Random();

            if (!context.Arena.Any())
            {
                var arenas = new List<Arena>();
                for (int i = 1; i <= 5; i++)
                {
                    arenas.Add(new Arena
                    {
                        Id = Guid.NewGuid(),
                        ArenaName = $"Arena {i}",
                        ArenaLocation = $"City {i}",
                        Capacity = 10000 + i * 1000
                    });
                }
                context.Arena.AddRange(arenas);
                context.SaveChanges();
            }

            if (!context.Team.Any())
            {
                var arenaIds = context.Arena.Select(a => a.Id).ToList();
                var conferences = Enum.GetValues<Conference>();
                var divisions = Enum.GetValues<Division>();
                var teams = new List<Team>();

                for (int i = 1; i <= 10; i++)
                {
                    teams.Add(new Team
                    {
                        Id = Guid.NewGuid(),
                        Name = $"Team {i}",
                        City = $"City {i}",
                        Site = $"https://www.team{i}.com",
                        Sponsor = $"Sponsor {i}",
                        News = $"News for Team {i}",
                        Ranking = i.ToString(),
                        Contact = $"contact@team{i}.com",
                        Conference = conferences[i % conferences.Length],
                        Division = divisions[i % divisions.Length],
                        ArenaId = arenaIds[i % arenaIds.Count],
                        ImageUrl = null
                    });
                }
                context.Team.AddRange(teams);
                context.SaveChanges();
            }

            if (!context.Player.Any())
            {
                var teamIds = context.Team.Select(t => t.Id).ToList();
                var players = new List<Player>();

                for (int i = 1; i <= 100; i++)
                {
                    players.Add(new Player
                    {
                        Id = Guid.NewGuid(),
                        FirstName = $"Player{i}",
                        LastName = $"Last{i}",
                        Age = rnd.Next(19, 35),
                        Position = positions[rnd.Next(positions.Length)],
                        TeamId = teamIds[i % teamIds.Count],
                        Height = rnd.Next(170, 220),
                        Weight = rnd.Next(70, 130),
                        Agent = $"Agent {i}",
                        Sponsor = $"Sponsor {i}",
                        News = $"Player {i} signs deal.",
                        ImageUrl = null
                    });
                }
                context.Player.AddRange(players);
                context.SaveChanges();
            }

            if (!context.Coach.Any())
            {
                var teamIds = context.Team.Select(t => t.Id).ToList();
                var coaches = new List<Coach>();

                for (int i = 1; i <= teamIds.Count; i++)
                {
                    coaches.Add(new Coach
                    {
                        Id = Guid.NewGuid(),
                        FirstName = $"Coach{i}",
                        LastName = $"Last{i}",
                        Age = rnd.Next(35, 65),
                        History = $"Coach {i} has led multiple teams over the years.",
                        TeamId = teamIds[i - 1],
                        ImageUrl = null
                    });
                }
                context.Coach.AddRange(coaches);
                context.SaveChanges();
            }

            if (!context.Referee.Any())
            {
                var referees = new List<Referee>();
                for (int i = 1; i <= 10; i++)
                {
                    referees.Add(new Referee
                    {
                        Id = Guid.NewGuid(),
                        FirstName = $"Referee{i}",
                        LastName = $"Last{i}",
                        Age = rnd.Next(30, 60),
                        Experience = $"{rnd.Next(1, 25)} years",
                        Licence = $"LIC-{1000 + i}",
                        ImageUrl = null
                    });
                }
                context.Referee.AddRange(referees);
                context.SaveChanges();
            }
        }
    }
}