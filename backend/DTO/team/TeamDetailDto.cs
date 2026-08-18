using nba_mvc.Models;

namespace nba_mvc.Dtos.Team
{
    public class TeamDetailDto : TeamDto
    {
        public List<Player.PlayerDto> Players { get; set; } = new();
    }
}