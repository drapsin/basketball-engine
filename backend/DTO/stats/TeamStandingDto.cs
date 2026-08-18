using nba_mvc.Models;

namespace nba_mvc.Dtos.Stats
{
    public class TeamStandingDto
    {
        public int ConferenceRank { get; set; }
        public int DivisionRank { get; set; }
        public Guid TeamId { get; set; }
        public string TeamName { get; set; }
        public string? LogoUrl { get; set; }
        public Conference Conference { get; set; }
        public Division Division { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public double WinPercentage => (Wins + Losses) == 0 ? 0 : Math.Round((double)Wins / (Wins + Losses), 3);
        public string Streak { get; set; } = "";
        public bool IsProjected { get; set; }
    }
}