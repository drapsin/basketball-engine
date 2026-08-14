using nba_mvc.Dtos.Stats;
using nba_mvc.Models;
using nba_mvc.Repositories.ActionEvent;
using nba_mvc.Repositories.Game;

namespace nba_mvc.Services.Stats
{
    public class GameStatsService : IGameStatsService
    {
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

            var boxScores = new List<PlayerBoxScoreDto>();

            foreach (var player in game.Players)
            {
                var playerEvents = events.Where(e => e.PlayerId == player.Id).ToList();

                var dto = new PlayerBoxScoreDto
                {
                    PlayerId = player.Id,
                    PlayerName = $"{player.FirstName} {player.LastName}",
                    Position = player.Position,
                    MinutesPlayed = CalculateMinutesPlayed(playerEvents),

                    Points = CalculatePoints(playerEvents),
                    OffensiveRebounds = playerEvents.Count(e => e.EventType == EventType.ReboundOff),
                    DefensiveRebounds = playerEvents.Count(e => e.EventType == EventType.ReboundDef),
                    Assists = playerEvents.Count(e => e.EventType == EventType.Assist),
                    Steals = playerEvents.Count(e => e.EventType == EventType.Steal),
                    Blocks = playerEvents.Count(e => e.EventType == EventType.Block),
                    Turnovers = playerEvents.Count(e => e.EventType == EventType.Turnover),
                    PersonalFouls = playerEvents.Count(e => e.EventType == EventType.Foul),

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

        private static string CalculateMinutesPlayed(List<Models.ActionEvent> playerEvents)
        {
            var subEvents = playerEvents
                .Where(e => e.EventType == EventType.SubstituteIn || e.EventType == EventType.SubstituteOut)
                .OrderBy(e => e.Quarter)
                .ThenByDescending(e => e.GameTime)
                .ToList();

            var totalSeconds = 0;
            DateTime? inTime = null;

            foreach (var evt in subEvents)
            {
                if (evt.EventType == EventType.SubstituteIn)
                {
                    inTime = DateTime.MinValue.Add(evt.GameTime);
                }
                else if (evt.EventType == EventType.SubstituteOut && inTime.HasValue)
                {
                    var outTime = DateTime.MinValue.Add(evt.GameTime);
                    var diff = (inTime.Value - outTime).TotalSeconds;
                    if (diff > 0) totalSeconds += (int)diff;
                    inTime = null;
                }
            }

            var minutes = totalSeconds / 60;
            var seconds = totalSeconds % 60;
            return $"{minutes}:{seconds:D2}";
        }
    }
}