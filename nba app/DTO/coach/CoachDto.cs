namespace nba_mvc.Dtos.Coach
{
    public class CoachDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public string History { get; set; }
        public Guid TeamId { get; set; }
        public string TeamName { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}