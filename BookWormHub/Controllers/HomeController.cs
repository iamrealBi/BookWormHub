using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookWormHub.Models;
using BookWormHub.Services.Interfaces;
using System.Diagnostics;

namespace BookWormHub.Controllers;

public class HomeController : Controller
{
    private readonly IHomeService _homeService;

    public HomeController(IHomeService homeService)
    {
        _homeService = homeService;
    }

    public async Task<IActionResult> Index()
    {
        var model = await _homeService.GetDashboardAsync();
        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [Authorize]
    public async Task<IActionResult> Profile()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return NotFound();

        var model = await _homeService.GetProfileAsync(userId);
        if (model == null) return NotFound();

        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
