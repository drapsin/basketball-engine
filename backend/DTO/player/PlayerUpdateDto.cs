using System.ComponentModel.DataAnnotations;

namespace nba_mvc.Dtos.Player
{
    public class PlayerUpdateDto
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Range(0, 100, ErrorMessage = "The value must be positive")]
        public int Age { get; set; }

        [Required]
        public string Position { get; set; }

        [Required]
        public Guid TeamId { get; set; }

        [Range(150, 230, ErrorMessage = "Please enter a valid height")]
        public int Height { get; set; }

        [Range(70, 200, ErrorMessage = "Please enter a valid weight")]
        public int Weight { get; set; }

        public string Agent { get; set; }
        public string Sponsor { get; set; }
        public string News { get; set; }
        public string? ImageUrl { get; set; }

        public byte[]? RowVersion { get; set; }
    }
}