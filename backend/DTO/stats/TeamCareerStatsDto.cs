namespace nba_mvc.Dtos.Stats
{
    public class TeamCareerStatsDto
    {
        public Guid TeamId { get; set; }
        public string TeamName { get; set; }
        public int GamesPlayed { get; set; }

        public int TotalPoints { get; set; }
        public double PointsPerGame { get; set; }

        public int TotalRebounds { get; set; }
        public double ReboundsPerGame { get; set; }

        public int TotalAssists { get; set; }
        public double AssistsPerGame { get; set; }

        public int TotalSteals { get; set; }
        public double StealsPerGame { get; set; }

        public int TotalBlocks { get; set; }
        public double BlocksPerGame { get; set; }

        public List<PlayerCareerStatsDto> Players { get; set; } = new();
    }
}