using Microsoft.AspNetCore.Identity;

namespace BookWormHub.Models
{
    public class ApplicationUser: IdentityUser
    {
        // F05: Critic

        public bool IsCritic { get; set; } = false;

        public DateTime? CrticSince { get; set; }

        // Relationship
        // 1 user - N reviews
        public List<Review> Reviews { get; set; } = new();
    }
}
