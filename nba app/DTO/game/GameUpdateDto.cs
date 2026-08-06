using System.ComponentModel.DataAnnotations;

namespace nba_mvc.Dtos.Game
{
    public class GameUpdateDto
    {
        [Required]
        public DateTime GameDate { get; set; }

        [Required]
        public string GameName { get; set; }

        [Required]
        public string GameTime { get; set; }

        public string GameResult { get; set; }
        public string Sponsor { get; set; }

        [Required]
        public Guid HomeTeamId { get; set; }

        [Required]
        public Guid AwayTeamId { get; set; }

        [Required]
        public Guid ArenaId { get; set; }

        public List<Guid> RefereeIds { get; set; } = new();
        public List<Guid> PlayerIds { get; set; } = new();

        public byte[] RowVersion { get; set; }
    }
}