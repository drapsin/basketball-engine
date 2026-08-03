using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace nba_mvc.Models
{
    public class Game : BaseId
    {
        public DateTime GameDate { get; set; }
        public string GameName { get; set; }
        public string GameTime { get; set; }
        public string GameLocation { get; set; }
        public string GameResult { get; set; }
        public string Sponsor { get; set; }
        public ICollection<ActionEvent> ActionEvents { get; set; } = new List<ActionEvent>();
        public ICollection<Player> Players { get; set; }
    }
}