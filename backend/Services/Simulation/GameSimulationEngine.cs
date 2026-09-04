using Microsoft.AspNetCore.SignalR;
using nba_mvc.Hubs;
using nba_mvc.Models;
using nba_mvc.Repositories.ActionEvent;
using nba_mvc.Repositories.Game;
using nba_mvc.Services.Game;
using nba_mvc.Services.Stats;

namespace nba_mvc.Services.Simulation
{
    public class GameSimulationEngine : IGameSimulationEngine
    {
        private readonly IGameRepository _gameRepository;
        private readonly IActionEventRepository _actionEventRepository;
        private readonly IGameStatsService _gameStatsService;
        private readonly IGameService _gameService;
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
            IGameService gameService,
            IHubContext<GameHub> hubContext)
        {
            _gameRepository = gameRepository;
            _actionEventRepository = actionEventRepository;
            _gameStatsService = gameStatsService;
            _gameService = gameService;
            _hubContext = hubContext;
        }

        public async Task AdvanceAsync(Guid gameId, GameSimulationState state)
        {
            var elapsed = TimeSpan.FromSeconds(5);
            state.GameClock -= elapsed;

            if (state.GameClock <= TimeSpan.Zero)
            {
                state.Quarter++;
                state.GameClock = TimeSpan.FromMinutes(12);
            }

            if (state.IsAutomatic && state.Quarter <= 4)
            {
                var game = await _gameRepository.GetByIdWithDetailsAsync(gameId);
                if (game != null && game.Players.Count > 0)
                {
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

        // Runs a full 4-quarter game instantly, no delays, no SignalR broadcast
        // (nobody is "watching" — this is a bulk data-generation tool for Admin).
        public async Task<bool> SimulateInstantAsync(Guid gameId)
        {
            var game = await _gameRepository.GetByIdWithDetailsAsync(gameId);
            if (game is null || game.Players.Count == 0) return false;
            if (game.GameResult != null) return false;

            var quarter = 1;
            var clock = TimeSpan.FromMinutes(12);

            while (quarter <= 4)
            {
                clock -= TimeSpan.FromSeconds(5);

                if (clock <= TimeSpan.Zero)
                {
                    quarter++;
                    clock = TimeSpan.FromMinutes(12);
                    continue;
                }

                var randomPlayer = game.Players.ElementAt(_rnd.Next(game.Players.Count));
                var eventType = EventPool[_rnd.Next(EventPool.Length)];

                var actionEvent = new Models.ActionEvent
                {
                    GameId = gameId,
                    PlayerId = randomPlayer.Id,
                    TeamId = randomPlayer.TeamId,
                    Quarter = quarter,
                    GameTime = clock,
                    EventType = eventType
                };

                await _actionEventRepository.AddAsync(actionEvent);
            }

            await _actionEventRepository.SaveChangesAsync();
            await _gameService.FinishGameAsync(gameId);

            return true;
        }
    }
}