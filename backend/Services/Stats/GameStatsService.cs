using nba_mvc.Dtos.Stats;
using nba_mvc.Models;
using nba_mvc.Repositories.ActionEvent;
using nba_mvc.Repositories.Game;

namespace nba_mvc.Services.Stats
{
    public class GameStatsService : IGameStatsService
    {
        private const int QuarterLengthSeconds = 720; // 12-minute quarters

        private readonly IGameRepository _gameRepository;
        private readonly IActionEventRepository _actionEventRepository;

        public GameStatsService(IGameRepository gameRepository, IActionEventRepository actionEventRepository)
        {
            _gameRepository = gameRepository;
            _actionEventRepository = actionEventRepository;
        }

        public async Task<List<PlayerBoxScoreDto>> GetBoxScoreAsync(Guid gameId)
        {
            var game = await _gameRepository.GetByIdWithDetailsAsync(gameId);
            if (game is null) return new List<PlayerBoxScoreDto>();

            var events = (await _actionEventRepository.GetByGameIdAsync(gameId)).ToList();

            var finalQuarter = events.Count == 0 ? 4 : events.Max(e => e.Quarter);

            var boxScores = new List<PlayerBoxScoreDto>();

            foreach (var player in game.Players)
            {
                var playerEvents = events.Where(e => e.PlayerId == player.Id).ToList();

                var dto = new PlayerBoxScoreDto
                {
                    PlayerId = player.Id,
                    PlayerName = $"{player.FirstName} {player.LastName}",
                    Position = player.Position,
                    MinutesPlayed = CalculateMinutesPlayed(playerEvents, finalQuarter),

                    Points = CalculatePoints(playerEvents),
                    OffensiveRebounds = playerEvents.Count(e => e.EventType == EventType.ReboundOff),
                    DefensiveRebounds = playerEvents.Count(e => e.EventType == EventType.ReboundDef),
                    Assists = playerEvents.Count(e => e.EventType == EventType.Assist),
                    Steals = playerEvents.Count(e => e.EventType == EventType.Steal),
                    Blocks = playerEvents.Count(e => e.EventType == EventType.Block),
                    Turnovers = playerEvents.Count(e => e.EventType == EventType.Turnover || e.EventType == EventType.OffensiveFoul),
                    PersonalFouls = playerEvents.Count(e => e.EventType == EventType.Foul
                        || e.EventType == EventType.OffensiveFoul
                        || e.EventType == EventType.TechnicalFoul
                        || e.EventType == EventType.FlagrantFoul),

                    FreeThrows = BuildShootingSplit(playerEvents, EventType.FreeThrowMade, EventType.FreeThrowMiss),
                    TwoPointers = BuildShootingSplit(playerEvents, EventType.TwoPointShot, EventType.TwoPointMiss),
                    ThreePointers = BuildShootingSplit(playerEvents, EventType.ThreePointShot, EventType.ThreePointMiss)
                };

                boxScores.Add(dto);
            }

            return boxScores;
        }

        private static int CalculatePoints(List<Models.ActionEvent> playerEvents)
        {
            return playerEvents.Sum(e => e.EventType switch
            {
                EventType.TwoPointShot => 2,
                EventType.ThreePointShot => 3,
                EventType.FreeThrowMade => 1,
                _ => 0
            });
        }

        private static ShootingSplitDto BuildShootingSplit(List<Models.ActionEvent> playerEvents, EventType madeType, EventType missType)
        {
            var made = playerEvents.Count(e => e.EventType == madeType);
            var missed = playerEvents.Count(e => e.EventType == missType);

            return new ShootingSplitDto
            {
                Made = made,
                Attempted = made + missed
            };
        }

        private static int ElapsedSeconds(Models.ActionEvent evt)
        {
            var elapsedInQuarter = QuarterLengthSeconds - (int)evt.GameTime.TotalSeconds;
            return (evt.Quarter - 1) * QuarterLengthSeconds + elapsedInQuarter;
        }

        private static string CalculateMinutesPlayed(List<Models.ActionEvent> playerEvents, int finalQuarter)
        {
            var subEvents = playerEvents
                .Where(e => e.EventType == EventType.SubstituteIn || e.EventType == EventType.SubstituteOut)
                .OrderBy(ElapsedSeconds)
                .ToList();

            var totalSeconds = 0;
            int? inElapsed = null;

            foreach (var evt in subEvents)
            {
                var elapsed = ElapsedSeconds(evt);

                if (evt.EventType == EventType.SubstituteIn)
                {
                    inElapsed = elapsed;
                }
                else if (evt.EventType == EventType.SubstituteOut && inElapsed.HasValue)
                {
                    var diff = elapsed - inElapsed.Value;
                    if (diff > 0) totalSeconds += diff;
                    inElapsed = null;
                }
            }

            // Player was subbed in but never subbed back out (e.g. finished the game on court) —
            // count them through to the end of the last quarter actually played, instead of dropping the stint.
            if (inElapsed.HasValue)
            {
                var gameEndElapsed = finalQuarter * QuarterLengthSeconds;
                var diff = gameEndElapsed - inElapsed.Value;
                if (diff > 0) totalSeconds += diff;
            }

            var minutes = totalSeconds / 60;
            var seconds = totalSeconds % 60;
            return $"{minutes}:{seconds:D2}";
        }

        public async Task<List<PlayByPlayEntryDto>> GetPlayByPlayAsync(Guid gameId)
        {
            var game = await _gameRepository.GetByIdAsync(gameId);
            if (game is null) return new List<PlayByPlayEntryDto>();

            var events = await _actionEventRepository.GetByGameIdAsync(gameId);

            var runningHomeScore = 0;
            var runningAwayScore = 0;
            var entries = new List<PlayByPlayEntryDto>();

            foreach (var evt in events)
            {
                var points = evt.EventType switch
                {
                    EventType.TwoPointShot => 2,
                    EventType.ThreePointShot => 3,
                    EventType.FreeThrowMade => 1,
                    _ => 0
                };

                if (points > 0)
                {
                    if (evt.TeamId == game.HomeTeamId)
                        runningHomeScore += points;
                    else if (evt.TeamId == game.AwayTeamId)
                        runningAwayScore += points;
                }

                entries.Add(new PlayByPlayEntryDto
                {
                    Quarter = evt.Quarter,
                    GameTime = evt.GameTime,
                    PlayerName = $"{evt.Player.FirstName} {evt.Player.LastName}",
                    TeamName = evt.Team.Name,
                    EventType = evt.EventType,
                    RunningHomeScore = runningHomeScore,
                    RunningAwayScore = runningAwayScore
                });
            }

            return entries;
        }

        public async Task<GameStateDto?> GetGameStateAsync(Guid gameId)
        {
            var game = await _gameRepository.GetByIdWithDetailsAsync(gameId);
            if (game is null) return null;

            var boxScore = await GetBoxScoreAsync(gameId);
            var playByPlay = await GetPlayByPlayAsync(gameId);
            var lastEvent = playByPlay.LastOrDefault();

            var homeTeamPlayerIds = game.Players
                .Where(p => p.TeamId == game.HomeTeamId)
                .Select(p => p.Id)
                .ToHashSet();

            return new GameStateDto
            {
                GameId = game.Id,
                Quarter = lastEvent?.Quarter ?? 1,
                GameClock = lastEvent?.GameTime.ToString(@"mm\:ss") ?? "12:00",
                HomeScore = lastEvent?.RunningHomeScore ?? 0,
                AwayScore = lastEvent?.RunningAwayScore ?? 0,
                LastEvent = lastEvent,
                HomeTeamStats = boxScore.Where(b => homeTeamPlayerIds.Contains(b.PlayerId)).ToList(),
                AwayTeamStats = boxScore.Where(b => !homeTeamPlayerIds.Contains(b.PlayerId)).ToList()
            };
        }
    }
}