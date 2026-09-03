using Microsoft.AspNetCore.SignalR;
using nba_mvc.Hubs;
using nba_mvc.Models;
using nba_mvc.Repositories.ActionEvent;
using nba_mvc.Repositories.Game;
using nba_mvc.Services.Stats;

namespace nba_mvc.Services.Simulation
{
    public class GameSimulationEngine : IGameSimulationEngine
    {
        private readonly IGameRepository _gameRepository;
        private readonly IActionEventRepository _actionEventRepository;
        private readonly IGameStatsService _gameStatsService;
        private readonly IHubContext<GameHub> _hubContext;
        private readonly Random _rnd = new();

        private static readonly EventType[] EventPool =
        {
            EventType.TwoPointShot, EventType.TwoPointShot, EventType.TwoPointMiss,
            EventType.ThreePointShot, EventType.ThreePointMiss, EventType.ThreePointMiss,
            EventType.Assist,
            EventType.ReboundOff, EventType.ReboundDef, EventType.ReboundDef,
            EventType.Steal, EventType.Block, EventType.Turnover,
            EventType.Foul, EventType.OffensiveFoul,
            EventType.FreeThrowMade, EventType.FreeThrowMiss,
            EventType.Timeout
        };

        public GameSimulationEngine(
            IGameRepository gameRepository,
            IActionEventRepository actionEventRepository,
            IGameStatsService gameStatsService,
            IHubContext<GameHub> hubContext)
        {
            _gameRepository = gameRepository;
            _actionEventRepository = actionEventRepository;
            _gameStatsService = gameStatsService;
            _hubContext = hubContext;
        }

        public async Task AdvanceAsync(Guid gameId, GameSimulationState state)
        {
            var game = await _gameRepository.GetByIdWithDetailsAsync(gameId);
            if (game is null || game.Players.Count == 0) return;

            var elapsed = TimeSpan.FromSeconds(5);
            state.GameClock -= elapsed;

            if (state.GameClock <= TimeSpan.Zero)
            {
                state.Quarter++;
                state.GameClock = TimeSpan.FromMinutes(12);
            }

            var updatedState = await _gameStatsService.GetGameStateAsync(gameId);
            if (updatedState != null)
            {
                updatedState.Quarter = state.Quarter;
                updatedState.GameClock = state.GameClock.ToString(@"mm\:ss");

                await _hubContext.Clients.Group(gameId.ToString())
                    .SendAsync("ReceiveGameUpdate", updatedState);
            }
        }
    }
}