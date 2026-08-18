namespace nba_mvc.Services.Simulation
{
    public interface IGameSimulationEngine
    {
        Task AdvanceAsync(Guid gameId, GameSimulationState state);
    }
}