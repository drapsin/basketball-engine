using nba_mvc.Models;
using nba_mvc.Repositories.ActionEvent;
using nba_mvc.Repositories.Game;

namespace nba_mvc.Services.Simulation
{
    public class GameSimulationEngine : IGameSimulationEngine
    {
        private readonly IGameRepository _gameRepository;
        private readonly IActionEventRepository _actionEventRepository;
        private readonly Random _rnd = new();

        private static readonly EventType[] EventPool =
        {
            EventType.TwoPointShot, EventType.TwoPointShot, EventType.TwoPointMiss,
            EventType.ThreePointShot, EventType.ThreePointMiss, EventType.ThreePointMiss,
            EventType.Assist,
            EventType.ReboundOff, EventType.ReboundDef, EventType.ReboundDef,
            EventType.Steal, EventType.Block, EventType.Turnover,
            EventType.Foul, EventType.FreeThrowMade, EventType.FreeThrowMiss
        };

        public GameSimulationEngine(IGameRepository gameRepository, IActionEventRepository actionEventRepository)
        {
            _gameRepository = gameRepository;
            _actionEventRepository = actionEventRepository;
        }

        public async Task AdvanceAsync(Guid gameId, GameSimulationState state)
        {
            var game = await _gameRepository.GetByIdWithDetailsAsync(gameId);
            if (game is null || game.Players.Count == 0) return;

            var elapsed = TimeSpan.FromSeconds(_rnd.Next(5, 21));
            state.GameClock -= elapsed;

            if (state.GameClock <= TimeSpan.Zero)
            {
                state.Quarter++;
                state.GameClock = TimeSpan.FromMinutes(12);

                if (state.Quarter > 4)
                {
                    await FinishSimulatedGameAsync(gameId);
                    return;
                }
            }

            var randomPlayer = game.Players.ElementAt(_rnd.Next(game.Players.Count));
            var eventType = EventPool[_rnd.Next(EventPool.Length)];

            var actionEvent = new Models.ActionEvent
            {
                GameId = gameId,
                PlayerId = randomPlayer.Id,
                TeamId = randomPlayer.TeamId,
                Quarter = state.Quarter,
                GameTime = state.GameClock,
                EventType = eventType
            };

            await _actionEventRepository.AddAsync(actionEvent);
            await _actionEventRepository.SaveChangesAsync();
        }

        private async Task FinishSimulatedGameAsync(Guid gameId)
        {
            var game = await _gameRepository.GetByIdAsync(gameId);
            if (game is null) return;
        }
    }
}