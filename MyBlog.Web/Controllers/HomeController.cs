using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyBlog.Data.UnitOfWork;
using MyBlog.Entity.Entities;
using MyBlog.Entity.Entities.Identity;
using MyBlog.Service.Services.Abstractions;
using MyBlog.Web.Models;
using NToastNotify;

namespace MyBlog.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IArticleService _articleService;
    private readonly UserManager<AppUser> _userManager;
    private readonly IToastNotification _toast;
    private readonly IUnitOfWork _unitOfWork;
    public HomeController(IToastNotification toast, UserManager<AppUser> userManager, IArticleService articleService, ILogger<HomeController> logger, IUnitOfWork unitOfWork)
    {
        _toast = toast;
        _userManager = userManager;
        _articleService = articleService;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var articles = await _articleService.GetAllArticlesWithCategoryNonDeletedAsync();
        return View(articles);
    }

    public IActionResult About()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    public async Task<IActionResult> Detail(Guid id)
    {
        string getIp = HttpContext.Connection.RemoteIpAddress!.MapToIPv4().ToString();

        var article = await _unitOfWork.GetRepository<Article>().GetByGuidAsync(id);
        var articleVisitors = await _unitOfWork.GetRepository<ArticleVisitor>().GetAllAsync(null, x => x.Visitor, x => x.Article);
        var visitor = await _unitOfWork.GetRepository<Visitor>().GetAsync(x => x.IpAddress == getIp);

        if(articleVisitors.Any(x => x.VisitorId == visitor.Id && x.ArticleId == article.Id))
            return View(await _articleService.GetArticleWithCategoryNonDeletedAsync(id));
        else
        {
            await _unitOfWork.GetRepository<ArticleVisitor>().AddAsync(new ArticleVisitor(){ArticleId = article.Id, VisitorId = visitor.Id});
            article.ViewCount++;
            await _unitOfWork.GetRepository<Article>().UpdateAsync(article);
            await _unitOfWork.SaveAsync();
        }
        return View(await _articleService.GetArticleWithCategoryNonDeletedAsync(id));
        
    }
}
