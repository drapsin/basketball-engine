namespace nba_mvc.Dtos.Arena
{
    public class ArenaDto
    {
        public Guid Id { get; set; }
        public string ArenaName { get; set; }
        public string ArenaLocation { get; set; }
        public int Capacity { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}