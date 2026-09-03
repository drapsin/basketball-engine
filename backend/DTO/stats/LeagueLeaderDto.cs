namespace nba_mvc.Dtos.Stats
{
    public class LeagueLeaderDto
    {
        public Guid PlayerId { get; set; }
        public string PlayerName { get; set; }
        public string TeamName { get; set; }
        public int GamesPlayed { get; set; }
        public double Value { get; set; }
    }
}