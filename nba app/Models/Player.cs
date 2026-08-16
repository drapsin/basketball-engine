using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace nba_mvc.Models
{
    public class Player : BaseId
    {
        [DisplayName("First name")]
        public string FirstName { get; set; }
        [DisplayName("Last name")]
        public string LastName { get; set; }

        [Range(0, 100, ErrorMessage = "The value must be positive")]
        public int Age { get; set; }
        public string Position { get; set; }
        [DisplayName("Team")]
        public Guid TeamId { get; set; }
        public Team? Team { get; set; }
        [Range(150, 230, ErrorMessage = "Please enter a valid height")]
        public int Height { get; set; }
        [Range(70, 200, ErrorMessage = "Please enter a valid weight")]
        public int Weight { get; set; }
        public string Agent { get; set; }
        public string Sponsor { get; set; }
        public string News { get; set; }
        public string? ImageUrl { get; set; }

    }
}