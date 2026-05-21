using BookWormHub.Models;

namespace BookWormHub.ViewModels;

public class ProfileViewModel
{
    public ApplicationUser User { get; set; } = null!;
    public List<Review> Reviews { get; set; } = new();
}
