namespace nba_mvc.Dtos.Team
{
    public class TeamDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string Site { get; set; }
        public string Sponsor { get; set; }
        public string News { get; set; }
        public string Ranking { get; set; }
        public string Contact { get; set; }
        public Models.Conference Conference { get; set; }
        public Models.Division Division { get; set; }
        public Guid ArenaId { get; set; }
        public string ArenaName { get; set; }
        public int PlayerCount { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}