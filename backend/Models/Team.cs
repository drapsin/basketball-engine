using System.ComponentModel.DataAnnotations;
using System.Numerics;
namespace nba_mvc.Models
{
    public class Team : BaseId
    {
        public string Name { get; set; }
        public string City { get; set; }
        public string Site { get; set; }
        public string Sponsor { get; set; }
        public string News { get; set; }
        public string Ranking { get; set; }
        public string Contact { get; set; }
        public Conference Conference { get; set; }
        public Division Division { get; set; }
        public Guid ArenaId { get; set; }              // FK
        public Arena? Arena { get; set; }               // Navigation
        public ICollection<Player>? Players { get; set; }
        public string? ImageUrl { get; set; }
    }

    public enum Conference
    {
        Eastern,
        Western
    }
    public enum Division
    {
        Atlantic,
        Central,
        Southeast,
        Northwest,
        Pacific,
        Southwest
    }

}