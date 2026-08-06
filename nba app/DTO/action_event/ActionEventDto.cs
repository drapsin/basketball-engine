namespace nba_mvc.Dtos.ActionEvent
{
    public class ActionEventDto
    {
        public Guid Id { get; set; }
        public Guid GameId { get; set; }
        public Guid PlayerId { get; set; }
        public string PlayerName { get; set; }
        public Guid TeamId { get; set; }
        public string TeamName { get; set; }
        public int Quarter { get; set; }
        public TimeSpan GameTime { get; set; }
        public Models.EventType EventType { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}