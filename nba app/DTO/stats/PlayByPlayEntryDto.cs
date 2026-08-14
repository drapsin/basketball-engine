using nba_mvc.Models;

namespace nba_mvc.Dtos.Stats
{
    public class PlayByPlayEntryDto
    {
        public int Quarter { get; set; }
        public TimeSpan GameTime { get; set; }
        public string PlayerName { get; set; }
        public string TeamName { get; set; }
        public EventType EventType { get; set; }
        public int RunningHomeScore { get; set; }
        public int RunningAwayScore { get; set; }
    }
}