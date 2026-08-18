using System.ComponentModel.DataAnnotations;

namespace nba_mvc.Dtos.Auth
{
    public class RegisterDto
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, MinLength(6)]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; } // "Admin", "Manager", or "Viewer"
    }
}