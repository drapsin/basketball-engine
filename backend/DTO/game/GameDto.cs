namespace nba_mvc.Dtos.Game
{
    public class GameDto
    {
        public Guid Id { get; set; }
        public DateTime GameDate { get; set; }
        public string GameName { get; set; }
        public string GameTime { get; set; }
        public string? GameResult { get; set; }
        public string Sponsor { get; set; }

        public Guid HomeTeamId { get; set; }
        public string HomeTeamName { get; set; }

        public Guid AwayTeamId { get; set; }
        public string AwayTeamName { get; set; }

        public Guid ArenaId { get; set; }
        public string ArenaName { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}