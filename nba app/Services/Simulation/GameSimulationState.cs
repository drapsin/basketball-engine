namespace nba_mvc.Services.Simulation
{
    public class GameSimulationState
    {
        public Guid GameId { get; set; }
        public bool IsPaused { get; set; }
        public int Quarter { get; set; } = 1;
        public TimeSpan GameClock { get; set; } = TimeSpan.FromMinutes(12);
    }
}