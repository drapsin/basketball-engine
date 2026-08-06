using System.ComponentModel.DataAnnotations;

namespace nba_mvc.Dtos.Referee
{
    public class RefereeCreateDto
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Range(0, 100, ErrorMessage = "The value must be positive")]
        public int Age { get; set; }

        [Required]
        public string Experience { get; set; }

        [Required]
        public string Licence { get; set; }

        public string? ImageUrl { get; set; }
    }
}