using System.ComponentModel.DataAnnotations;

namespace nba_mvc.Dtos.Team
{
    public class TeamUpdateDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string City { get; set; }

        public string Site { get; set; }
        public string Sponsor { get; set; }
        public string News { get; set; }
        public string Ranking { get; set; }
        public string Contact { get; set; }

        [Required]
        public Models.Conference Conference { get; set; }

        [Required]
        public Models.Division Division { get; set; }

        [Required]
        public Guid ArenaId { get; set; }

        public string? ImageUrl { get; set; }

        public byte[] RowVersion { get; set; }
    }
}