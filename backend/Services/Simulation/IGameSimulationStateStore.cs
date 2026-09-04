namespace nba_mvc.Services.Simulation
{
    public interface IGameSimulationStateStore
    {
        bool TryStart(Guid gameId, bool isAutomatic);
        bool Pause(Guid gameId);
        bool Resume(Guid gameId);
        bool Stop(Guid gameId);
        GameSimulationState? Get(Guid gameId);
        IEnumerable<GameSimulationState> GetAllActive();
        IEnumerable<GameSimulationState> GetAllStates();
    }
}