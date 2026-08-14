namespace nba_mvc.Dtos.Stats
{
    public class GameStateDto
    {
        public Guid GameId { get; set; }
        public int Quarter { get; set; }
        public string GameClock { get; set; }
        public int HomeScore { get; set; }
        public int AwayScore { get; set; }
        public PlayByPlayEntryDto? LastEvent { get; set; }
        public List<PlayerBoxScoreDto> HomeTeamStats { get; set; } = new();
        public List<PlayerBoxScoreDto> AwayTeamStats { get; set; } = new();
    }
}