namespace BookWormHub.Services.Interfaces;

public interface IBadgeService
{
    Task CheckAndAwardBadge(string userId);
}
