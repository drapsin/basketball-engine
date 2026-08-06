using System.ComponentModel.DataAnnotations;

namespace nba_mvc.Dtos.Arena
{
    public class ArenaCreateDto
    {
        [Required]
        public string ArenaName { get; set; }

        [Required]
        public string ArenaLocation { get; set; }

        [Range(1, 100000, ErrorMessage = "Please enter a valid capacity")]
        public int Capacity { get; set; }
    }
}