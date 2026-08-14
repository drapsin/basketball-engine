namespace nba_mvc.Dtos.Stats
{
    public class PlayerBoxScoreDto
    {
        public Guid PlayerId { get; set; }
        public string PlayerName { get; set; }
        public string Position { get; set; }
        public string MinutesPlayed { get; set; }

        public int Points { get; set; }
        public int OffensiveRebounds { get; set; }
        public int DefensiveRebounds { get; set; }
        public int Rebounds => OffensiveRebounds + DefensiveRebounds;
        public int Assists { get; set; }
        public int Steals { get; set; }
        public int Blocks { get; set; }
        public int Turnovers { get; set; }
        public int PersonalFouls { get; set; }

        public ShootingSplitDto FreeThrows { get; set; }
        public ShootingSplitDto TwoPointers { get; set; }
        public ShootingSplitDto ThreePointers { get; set; }
    }
}