using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookWormHub.Services.Interfaces;
using BookWormHub.ViewModels;

namespace BookWormHub.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    // GET: /Admin/BannedWords
    public async Task<IActionResult> BannedWords(string? notify, string? error)
    {
        var model = await _adminService.GetBannedWordsAsync();
        model.StatusMessage = notify;
        model.ErrorMessage = error;
        return View(model);
    }

    // POST: /Admin/AddBannedWord
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AddBannedWord(string word)
    {
        var result = await _adminService.AddBannedWordAsync(word);

        return result.Success
            ? RedirectToAction(nameof(BannedWords), new { notify = result.Message })
            : RedirectToAction(nameof(BannedWords), new { error = result.Error });
    }

    // POST: /Admin/DeleteBannedWord/5
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteBannedWord(int id)
    {
        var result = await _adminService.DeleteBannedWordAsync(id);
        return RedirectToAction(nameof(BannedWords), new { notify = result.Message });
    }

    // GET: /Admin/Reviews
    public async Task<IActionResult> Reviews(string? notify)
    {
        var model = await _adminService.GetHiddenReviewsAsync();
        model.StatusMessage = notify;
        return View(model);
    }

    // POST: /Admin/ApproveReview/5
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ApproveReview(int id)
    {
        var result = await _adminService.ApproveReviewAsync(id);
        return RedirectToAction(nameof(Reviews), new { notify = result.Message });
    }

    // POST: /Admin/RejectReview/5
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> RejectReview(int id)
    {
        var result = await _adminService.RejectReviewAsync(id);
        return RedirectToAction(nameof(Reviews), new { notify = result.Message });
    }

    // POST: /Admin/RevokeBadge
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> RevokeBadge(string userId)
    {
        var result = await _adminService.RevokeBadgeAsync(userId);
        return RedirectToAction(nameof(Reviews), new { notify = result.Message });
    }
}
