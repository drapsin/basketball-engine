using nba_mvc.Services.Game;

namespace nba_mvc.Services.Simulation
{
    public class GameSimulationBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IGameSimulationStateStore _stateStore;
        private readonly ILogger<GameSimulationBackgroundService> _logger;

        public GameSimulationBackgroundService(
            IServiceScopeFactory scopeFactory,
            IGameSimulationStateStore stateStore,
            ILogger<GameSimulationBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _stateStore = stateStore;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var activeGames = _stateStore.GetAllActive().ToList();

                foreach (var state in activeGames)
                {
                    try
                    {
                        using var scope = _scopeFactory.CreateScope();
                        var engine = scope.ServiceProvider.GetRequiredService<IGameSimulationEngine>();
                        var gameService = scope.ServiceProvider.GetRequiredService<IGameService>();

                        await engine.AdvanceAsync(state.GameId, state);

                        if (state.Quarter > 4)
                        {
                            await gameService.FinishGameAsync(state.GameId);
                            _stateStore.Stop(state.GameId);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Simulation tick failed for game {GameId}", state.GameId);
                    }
                }

                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    // Expected during shutdown
                }
            }
        }
    }
}