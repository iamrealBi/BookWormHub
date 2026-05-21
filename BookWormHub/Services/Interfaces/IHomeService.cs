using BookWormHub.ViewModels;

namespace BookWormHub.Services.Interfaces;

public interface IHomeService
{
    Task<HomeIndexViewModel> GetDashboardAsync();
    Task<ProfileViewModel?> GetProfileAsync(string userId);
}
