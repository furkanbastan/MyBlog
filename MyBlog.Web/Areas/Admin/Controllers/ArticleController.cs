using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBlog.Entity.Consts;
using MyBlog.Entity.DTOs.Articles;
using MyBlog.Service.Services.Abstractions;
using MyBlog.Web.ResultMessages;
using NToastNotify;

namespace MyBlog.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class ArticleController : Controller
{
    private readonly IArticleService _articleService;
    private readonly ICategoryService _categoryService;
    private readonly IToastNotification _toast;
    public ArticleController(IArticleService articleService, IMapper mapper, ICategoryService categoryService, IToastNotification toast)
    {
        _articleService = articleService;
        _categoryService = categoryService;
        _toast = toast;
    }

    [HttpGet]
    [Authorize(Roles = $"{RoleConsts.Superadmin}, {RoleConsts.Admin}, {RoleConsts.User}")]
    public async Task<IActionResult> Index()
    {
        var articles = await _articleService.GetAllArticlesWithCategoryNonDeletedAsync();
        return View(articles);
    }

    [HttpGet]
    [Authorize(Roles = $"{RoleConsts.Superadmin}, {RoleConsts.Admin}")]
    public async Task<IActionResult> DeletedArticle()
    {
        var articles = await _articleService.GetAllArticlesWithCategoryDeletedAsync();
        return View(articles);
    }

    [HttpGet]
    [Authorize(Roles = $"{RoleConsts.Superadmin}, {RoleConsts.Admin}")]
    public async Task<IActionResult> Add()
    {
        var categories = await  _categoryService.GetAllCategoriesNonDeleted();
        return View(new ArticleAddDto{Categories = categories});
    }

    [HttpPost]
    [Authorize(Roles = $"{RoleConsts.Superadmin}, {RoleConsts.Admin}")]
    public async Task<IActionResult> Add(ArticleAddDto articleAddDto)
    {
        await _articleService.CreateArticleAsync(articleAddDto, this.ModelState);

        if(ModelState.IsValid)
        {
            _toast.AddSuccessToastMessage(Messages.Success.Add(articleAddDto.Title), new ToastrOptions { Title = "İşlem Başarılı" });
            return RedirectToAction("Index", "Article", new { Area = "Admin" });
        }
        else
        {
            var categories = await _categoryService.GetAllCategoriesNonDeleted();
            return View(new ArticleAddDto { Categories = categories });
        }
    }

    [HttpGet]
    [Authorize(Roles = $"{RoleConsts.Superadmin}, {RoleConsts.Admin}")]
    public async Task<IActionResult> Update(Guid id)
    {
        var article = await _articleService.GetArticleWithCategoryNonDeletedAsync(id);
        var categories = await _categoryService.GetAllCategoriesNonDeleted();

        return View(new ArticleUpdateDto
        {
            Categories = categories,
            Id = id,
            Content = article.Content,
            Title = article.Title,
            Image = article.Image,
            CategoryId = article.Category.Id
        });
    }

    [HttpPost]
    [Authorize(Roles = $"{RoleConsts.Superadmin}, {RoleConsts.Admin}")]
    public async Task<IActionResult> Update(ArticleUpdateDto articleUpdateDto)
    {
        await _articleService.ArticleUpdateAsync(articleUpdateDto, this.ModelState);

        if (ModelState.IsValid)
        {
            _toast.AddSuccessToastMessage(Messages.Success.Update(articleUpdateDto.Title), new ToastrOptions { Title = "İşlem Başarılı" });
            return RedirectToAction("Index", "Article", new { Area = "Admin" });
        }
        else
        {
            _toast.AddSuccessToastMessage(Messages.Warning.Update(articleUpdateDto.Title), new ToastrOptions { Title = "İşlem Başarısız" });
            var categories = await _categoryService.GetAllCategoriesNonDeleted();
            return await Update(articleUpdateDto.Id);
        }
    }

    [Authorize(Roles = $"{RoleConsts.Superadmin}, {RoleConsts.Admin}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var title = await _articleService.SafeDeleteArticleAsync(id);
        _toast.AddSuccessToastMessage(Messages.Success.Delete(title), new ToastrOptions { Title = "İşlem Başarılı" });

        return RedirectToAction("Index", "Article", new { Area = "Admin" });
    }

    [Authorize(Roles = $"{RoleConsts.Superadmin}, {RoleConsts.Admin}")]
    public async Task<IActionResult> UndoDelete(Guid id)
    {
        var title = await _articleService.UndoDeleteArticleAsync(id);
        _toast.AddSuccessToastMessage(Messages.Success.UndoDelete(title), new ToastrOptions { Title = "İşlem Başarılı" });

        return RedirectToAction("Index", "Article", new { Area = "Admin" });
    }
}
