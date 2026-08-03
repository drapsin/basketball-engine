using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace nba_mvc.Models
{
    public class Arena : BaseId
    {
        [DisplayName("Arena name:")]
        public string ArenaName { get; set; }
        [DisplayName("Arena location:")]
        public string ArenaLocation { get; set; }
        public int Capacity { get; set; }
    }
}