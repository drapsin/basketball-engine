using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace nba_mvc.Models
{
    public class Coach : BaseId
    {
        [DisplayName("First name: ")]
        public string FirstName { get; set; }
        [DisplayName("Last name: ")]
        public string LastName { get; set; }

        [Range(0, 100, ErrorMessage = "The value must be positive")]
        public int Age { get; set; }
        public string History { get; set; }
        public Guid TeamId { get; set; }
        public Team? Team { get; set; }
        public string? ImageUrl { get; set; }

    }
}