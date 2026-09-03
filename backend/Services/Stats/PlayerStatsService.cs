using Microsoft.EntityFrameworkCore;
using nba_mvc.Data;
using nba_mvc.Dtos.Stats;
using nba_mvc.Models;

namespace nba_mvc.Services.Stats
{
    public class PlayerStatsService : IPlayerStatsService
    {
        private readonly ApplicationDbContext _context;

        public PlayerStatsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PlayerCareerStatsDto?> GetPlayerCareerStatsAsync(Guid playerId)
        {
            var player = await _context.Player
                .Include(p => p.Team)
                .FirstOrDefaultAsync(p => p.Id == playerId);

            if (player is null) return null;

            var events = await _context.ActionEvent
                .Where(e => e.PlayerId == playerId)
                .ToListAsync();

            return BuildCareerStats(player, events);
        }

        public async Task<TeamCareerStatsDto?> GetTeamCareerStatsAsync(Guid teamId)
        {
            var team = await _context.Team.FirstOrDefaultAsync(t => t.Id == teamId);
            if (team is null) return null;

            var players = await _context.Player
                .Where(p => p.TeamId == teamId)
                .Include(p => p.Team)
                .ToListAsync();

            var playerStats = new List<PlayerCareerStatsDto>();
            var allGameIds = new HashSet<Guid>();

            foreach (var player in players)
            {
                var events = await _context.ActionEvent
                    .Where(e => e.PlayerId == player.Id)
                    .ToListAsync();

                foreach (var gid in events.Select(e => e.GameId).Distinct())
                    allGameIds.Add(gid);

                playerStats.Add(BuildCareerStats(player, events));
            }

            var gamesPlayed = allGameIds.Count;

            var totalPoints = playerStats.Sum(p => p.TotalPoints);
            var totalRebounds = playerStats.Sum(p => p.TotalRebounds);
            var totalAssists = playerStats.Sum(p => p.TotalAssists);
            var totalSteals = playerStats.Sum(p => p.TotalSteals);
            var totalBlocks = playerStats.Sum(p => p.TotalBlocks);

            return new TeamCareerStatsDto
            {
                TeamId = team.Id,
                TeamName = team.Name,
                GamesPlayed = gamesPlayed,
                TotalPoints = totalPoints,
                PointsPerGame = gamesPlayed == 0 ? 0 : Math.Round((double)totalPoints / gamesPlayed, 1),
                TotalRebounds = totalRebounds,
                ReboundsPerGame = gamesPlayed == 0 ? 0 : Math.Round((double)totalRebounds / gamesPlayed, 1),
                TotalAssists = totalAssists,
                AssistsPerGame = gamesPlayed == 0 ? 0 : Math.Round((double)totalAssists / gamesPlayed, 1),
                TotalSteals = totalSteals,
                StealsPerGame = gamesPlayed == 0 ? 0 : Math.Round((double)totalSteals / gamesPlayed, 1),
                TotalBlocks = totalBlocks,
                BlocksPerGame = gamesPlayed == 0 ? 0 : Math.Round((double)totalBlocks / gamesPlayed, 1),
                Players = playerStats.OrderByDescending(p => p.TotalPoints).ToList()
            };
        }

        public async Task<List<LeagueLeaderDto>> GetLeagueLeadersAsync(string category, int limit)
        {
            var players = await _context.Player.Include(p => p.Team).ToListAsync();
            var allEvents = await _context.ActionEvent.ToListAsync();

            var leaders = new List<LeagueLeaderDto>();

            foreach (var player in players)
            {
                var events = allEvents.Where(e => e.PlayerId == player.Id).ToList();
                if (events.Count == 0) continue;

                var stats = BuildCareerStats(player, events);

                double value = category.ToLower() switch
                {
                    "points" => stats.PointsPerGame,
                    "rebounds" => stats.ReboundsPerGame,
                    "assists" => stats.AssistsPerGame,
                    "steals" => stats.StealsPerGame,
                    "blocks" => stats.BlocksPerGame,
                    _ => stats.PointsPerGame
                };

                leaders.Add(new LeagueLeaderDto
                {
                    PlayerId = player.Id,
                    PlayerName = $"{player.FirstName} {player.LastName}",
                    TeamName = player.Team?.Name ?? "",
                    GamesPlayed = stats.GamesPlayed,
                    Value = value
                });
            }

            return leaders
                .OrderByDescending(l => l.Value)
                .Take(limit)
                .ToList();
        }

        private static PlayerCareerStatsDto BuildCareerStats(Models.Player player, List<Models.ActionEvent> events)
        {
            var gamesPlayed = events.Select(e => e.GameId).Distinct().Count();

            var points = events.Sum(e => e.EventType switch
            {
                EventType.TwoPointShot => 2,
                EventType.ThreePointShot => 3,
                EventType.FreeThrowMade => 1,
                _ => 0
            });

            var rebounds = events.Count(e => e.EventType == EventType.ReboundOff || e.EventType == EventType.ReboundDef);
            var assists = events.Count(e => e.EventType == EventType.Assist);
            var steals = events.Count(e => e.EventType == EventType.Steal);
            var blocks = events.Count(e => e.EventType == EventType.Block);
            var turnovers = events.Count(e => e.EventType == EventType.Turnover || e.EventType == EventType.OffensiveFoul);

            ShootingSplitDto Split(EventType made, EventType missed)
            {
                var m = events.Count(e => e.EventType == made);
                var miss = events.Count(e => e.EventType == missed);
                return new ShootingSplitDto { Made = m, Attempted = m + miss };
            }

            double PerGame(int total) => gamesPlayed == 0 ? 0 : Math.Round((double)total / gamesPlayed, 1);

            return new PlayerCareerStatsDto
            {
                PlayerId = player.Id,
                PlayerName = $"{player.FirstName} {player.LastName}",
                TeamName = player.Team?.Name ?? "",
                GamesPlayed = gamesPlayed,
                TotalPoints = points,
                PointsPerGame = PerGame(points),
                TotalRebounds = rebounds,
                ReboundsPerGame = PerGame(rebounds),
                TotalAssists = assists,
                AssistsPerGame = PerGame(assists),
                TotalSteals = steals,
                StealsPerGame = PerGame(steals),
                TotalBlocks = blocks,
                BlocksPerGame = PerGame(blocks),
                TotalTurnovers = turnovers,
                TurnoversPerGame = PerGame(turnovers),
                FreeThrows = Split(EventType.FreeThrowMade, EventType.FreeThrowMiss),
                TwoPointers = Split(EventType.TwoPointShot, EventType.TwoPointMiss),
                ThreePointers = Split(EventType.ThreePointShot, EventType.ThreePointMiss)
            };
        }
    }
}