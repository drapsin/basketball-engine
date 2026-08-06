using System.ComponentModel.DataAnnotations;

namespace nba_mvc.Dtos.Coach
{
    public class CoachCreateDto
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Range(0, 100, ErrorMessage = "The value must be positive")]
        public int Age { get; set; }

        public string History { get; set; }

        [Required]
        public Guid TeamId { get; set; }

        public string? ImageUrl { get; set; }
    }
}