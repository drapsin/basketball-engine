using System.Collections.Concurrent;

namespace nba_mvc.Services.Simulation
{
    public class GameSimulationStateStore : IGameSimulationStateStore
    {
        private readonly ConcurrentDictionary<Guid, GameSimulationState> _activeGames = new();

        public bool TryStart(Guid gameId)
        {
            return _activeGames.TryAdd(gameId, new GameSimulationState { GameId = gameId });
        }

        public bool Pause(Guid gameId)
        {
            if (_activeGames.TryGetValue(gameId, out var state))
            {
                state.IsPaused = true;
                return true;
            }
            return false;
        }

        public bool Resume(Guid gameId)
        {
            if (_activeGames.TryGetValue(gameId, out var state))
            {
                state.IsPaused = false;
                return true;
            }
            return false;
        }

        public bool Stop(Guid gameId)
        {
            return _activeGames.TryRemove(gameId, out _);
        }

        public GameSimulationState? Get(Guid gameId)
        {
            return _activeGames.TryGetValue(gameId, out var state) ? state : null;
        }

        public IEnumerable<GameSimulationState> GetAllActive()
        {
            return _activeGames.Values.Where(s => !s.IsPaused);
        }
    }
}