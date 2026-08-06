namespace nba_mvc.Dtos.Game
{
    public class GameDetailDto : GameDto
    {
        public List<Referee.RefereeDto> Referees { get; set; } = new();
        public List<Player.PlayerDto> Players { get; set; } = new();
    }
}