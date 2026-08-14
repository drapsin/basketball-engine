using nba_mvc.Dtos.Stats;
using nba_mvc.Models;
using nba_mvc.Repositories.Game;
using nba_mvc.Repositories.Team;

namespace nba_mvc.Services.Stats
{
    public class StandingsService : IStandingsService
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IGameRepository _gameRepository;

        public StandingsService(ITeamRepository teamRepository, IGameRepository gameRepository)
        {
            _teamRepository = teamRepository;
            _gameRepository = gameRepository;
        }

        public async Task<List<TeamStandingDto>> GetStandingsAsync()
        {
            var teams = (await _teamRepository.GetAllAsync()).ToList();
            var allGames = (await _gameRepository.GetAllAsync())
                .Where(g => g.GameResult != null)
                .ToList();

            var standings = new List<TeamStandingDto>();

            foreach (var team in teams)
            {
                var teamGames = allGames
                    .Where(g => g.HomeTeamId == team.Id || g.AwayTeamId == team.Id)
                    .OrderBy(g => g.GameDate)
                    .ToList();

                var wins = 0;
                var losses = 0;
                var resultsChronological = new List<bool>(); 

                foreach (var game in teamGames)
                {
                    var scores = game.GameResult!.Split('-');
                    if (scores.Length != 2) continue;
                    if (!int.TryParse(scores[0], out var homeScore)) continue;
                    if (!int.TryParse(scores[1], out var awayScore)) continue;

                    var isHome = game.HomeTeamId == team.Id;
                    var teamScore = isHome ? homeScore : awayScore;
                    var opponentScore = isHome ? awayScore : homeScore;

                    var won = teamScore > opponentScore;
                    if (won) wins++; else losses++;
                    resultsChronological.Add(won);
                }

                standings.Add(new TeamStandingDto
                {
                    TeamId = team.Id,
                    TeamName = team.Name,
                    LogoUrl = team.ImageUrl,
                    Conference = team.Conference,
                    Division = team.Division,
                    Wins = wins,
                    Losses = losses,
                    Streak = CalculateStreak(resultsChronological),
                    IsProjected = false
                });
            }

            foreach (var conferenceGroup in standings.GroupBy(s => s.Conference))
            {
                var ranked = conferenceGroup.OrderByDescending(s => s.WinPercentage).ToList();
                for (int i = 0; i < ranked.Count; i++)
                    ranked[i].ConferenceRank = i + 1;
            }

            foreach (var divisionGroup in standings.GroupBy(s => s.Division))
            {
                var ranked = divisionGroup.OrderByDescending(s => s.WinPercentage).ToList();
                for (int i = 0; i < ranked.Count; i++)
                    ranked[i].DivisionRank = i + 1;
            }

            return standings.OrderBy(s => s.ConferenceRank).ToList();
        }

        private static string CalculateStreak(List<bool> resultsChronological)
        {
            if (resultsChronological.Count == 0) return "-";

            var last = resultsChronological[^1];
            var streakCount = 0;

            for (int i = resultsChronological.Count - 1; i >= 0; i--)
            {
                if (resultsChronological[i] == last) streakCount++;
                else break;
            }

            return (last ? "W" : "L") + streakCount;
        }
    }
}