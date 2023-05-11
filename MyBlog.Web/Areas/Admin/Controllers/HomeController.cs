using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBlog.Service.Services.Abstractions;
using Newtonsoft.Json;

namespace MyBlog.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class HomeController : Controller
{
    private readonly IArticleService _articleService;
    private readonly IDashboardService _dashboardService;
    public HomeController(IArticleService articleService, IDashboardService dashboardService)
    {
        _articleService = articleService;
        _dashboardService = dashboardService;
    }
    public async Task<IActionResult> Index()
    {
        var articles = await _articleService.GetAllArticlesWithCategoryNonDeletedAsync();
        return View(articles);
    }
    [HttpGet]
    public async Task<IActionResult> YearlyArticleCounts()
    {
        var count = await _dashboardService.GetYearlyArticleCounts();
        return Json(JsonConvert.SerializeObject(count));
    }
    [HttpGet]
    public async Task<IActionResult> TotalArticleCount()
    {
        var count = await _dashboardService.GetTotalArticleCount();
        return Json(count);
    }
    [HttpGet]
    public async Task<IActionResult> TotalCategoryCount()
    {
        var count = await _dashboardService.GetTotalCategoryCount();
        return Json(count);
    }
}
