namespace nba_mvc.Dtos.Stats
{
    public class PlayerCareerStatsDto
    {
        public Guid PlayerId { get; set; }
        public string PlayerName { get; set; }
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

        public int TotalTurnovers { get; set; }
        public double TurnoversPerGame { get; set; }

        public ShootingSplitDto FreeThrows { get; set; }
        public ShootingSplitDto TwoPointers { get; set; }
        public ShootingSplitDto ThreePointers { get; set; }
    }
}