namespace nba_mvc.Dtos.Auth
{
    public class AuthResultDto
    {
        public string Token { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}