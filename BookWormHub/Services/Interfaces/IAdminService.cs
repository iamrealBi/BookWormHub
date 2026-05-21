using BookWormHub.Models;
using BookWormHub.ViewModels;

namespace BookWormHub.Services.Interfaces;

public interface IAdminService
{
    Task<AdminBannedWordsViewModel> GetBannedWordsAsync();
    Task<ServiceResult> AddBannedWordAsync(string word);
    Task<ServiceResult> DeleteBannedWordAsync(int id);
    Task<AdminReviewsViewModel> GetHiddenReviewsAsync();
    Task<ServiceResult> ApproveReviewAsync(int id);
    Task<ServiceResult> RejectReviewAsync(int id);
    Task<ServiceResult> RevokeBadgeAsync(string userId);
}
