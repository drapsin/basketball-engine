using System.ComponentModel.DataAnnotations;

namespace nba_mvc.Dtos.ActionEvent
{
    public class ActionEventCreateDto
    {
        [Required]
        public Guid GameId { get; set; }

        [Required]
        public Guid PlayerId { get; set; }

        [Required]
        public Guid TeamId { get; set; }

        [Range(1, 10)]
        public int Quarter { get; set; }

        [Required]
        public TimeSpan GameTime { get; set; }

        [Required]
        public Models.EventType EventType { get; set; }
    }
}