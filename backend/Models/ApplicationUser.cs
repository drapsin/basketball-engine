using Microsoft.AspNetCore.Identity;

namespace nba_mvc.Models
{
    public enum ApprovalStatus
    {
        Pending,
        Approved,
        Rejected
    }

    public class ApplicationUser : IdentityUser
    {
        public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.Pending;
    }
}