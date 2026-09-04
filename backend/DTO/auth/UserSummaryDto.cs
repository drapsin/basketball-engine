namespace nba_mvc.Dtos.Auth
{
    public class UserSummaryDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string ApprovalStatus { get; set; }
        public bool IsSeededAdmin { get; set; }
    }
}