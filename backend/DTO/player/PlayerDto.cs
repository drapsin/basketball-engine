namespace nba_mvc.Dtos.Player
{
    public class PlayerDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public string Position { get; set; }
        public Guid TeamId { get; set; }
        public string TeamName { get; set; }
        public int Height { get; set; }
        public int Weight { get; set; }
        public string Agent { get; set; }
        public string Sponsor { get; set; }
        public string News { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}