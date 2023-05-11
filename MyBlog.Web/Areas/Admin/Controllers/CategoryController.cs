using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBlog.Entity.DTOs.Categories;
using MyBlog.Service.Services.Abstractions;
using MyBlog.Web.ResultMessages;
using NToastNotify;

namespace MyBlog.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class CategoryController : Controller
{
    private readonly ICategoryService _categoryService;
    private readonly IMapper _mapper;
    private readonly IToastNotification _toast;
    public CategoryController(ICategoryService categoryService, IMapper mapper, IToastNotification toast)
    {
        _categoryService = categoryService;
        _mapper = mapper;
        _toast = toast;
    }

    public async Task<IActionResult> Index()
    {
        var categories = await _categoryService.GetAllCategoriesNonDeleted();
        return View(categories);
    }
    public async Task<IActionResult> DeletedCategory()
    {
        var categories = await _categoryService.GetAllCategoriesDeleted();
        return View(categories);
    }
    [HttpGet]
    public IActionResult Add()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Add(CategoryAddDto categoryAddDto)
    {
        await _categoryService.CreateCategoryAsync(categoryAddDto, this.ModelState);
        if(!ModelState.IsValid)
        {
            _toast.AddWarningToastMessage(Messages.Warning.Add(""), new ToastrOptions { Title = "!!!" });
        }
        return RedirectToAction("Index", "Category", new { Area = "Admin" });
    }
    [HttpPost]
    public async Task<IActionResult> AddWithAjax([FromBody] CategoryAddDto categoryAddDto)
    {
        await _categoryService.CreateCategoryAsync(categoryAddDto, this.ModelState);
        return View();
        /*
        var map = mapper.Map<Category>(categoryAddDto);
            var result = await validator.ValidateAsync(map);

            if (result.IsValid)
            {
                await categoryService.CreateCategoryAsync(categoryAddDto);
                toast.AddSuccessToastMessage(Messages.Category.Add(categoryAddDto.Name), new ToastrOptions { Title = "İşlem Başarılı" });

                return Json(Messages.Category.Add(categoryAddDto.Name));
            }
            else
            {
                toast.AddErrorToastMessage(result.Errors.First().ErrorMessage , new ToastrOptions { Title = "İşlem Başarısız" });
                return Json (result.Errors.First().ErrorMessage);
            }
        */
    }
    public async Task<IActionResult> Update(Guid id)
    {
        var category = await _categoryService.GetCategoryByGuid(id);
        var map = _mapper.Map<CategoryUpdateDto>(category);
        return View(map);
    }
    [HttpPost]
    public async Task<IActionResult> Update(CategoryUpdateDto categoryUpdateDto)
    {
        await _categoryService.UpdateCategoryAsync(categoryUpdateDto, this.ModelState);
        return RedirectToAction("Index", "Category", new { Area = "Admin" });
        /*
        var map = mapper.Map<Category>(categoryUpdateDto);
            var result = await validator.ValidateAsync(map);

            if (result.IsValid)
            {
                var name = await categoryService.UpdateCategoryAsync(categoryUpdateDto);
                toast.AddSuccessToastMessage(Messages.Category.Update(name), new ToastrOptions { Title = "İşlem Başarılı" }); 
                return RedirectToAction("Index", "Category", new { Area = "Admin" });
            }

            result.AddToModelState(this.ModelState);
            return View();
        */
    }
    public async Task<IActionResult> Delete(Guid id)
    {
        await _categoryService.SafeDeleteCategoryAsync(id);
        return RedirectToAction("Index", "Category", new { Area = "Admin" });
        /*
        var name = await categoryService.SafeDeleteCategoryAsync(categoryId);
            toast.AddSuccessToastMessage(Messages.Category.Delete(name), new ToastrOptions() { Title = "İşlem Başarılı" });

            return RedirectToAction("Index", "Category", new { Area = "Admin" });
        */
    }
    public async Task<IActionResult> UndoDelete(Guid id)
    {
        await _categoryService.SafeDeleteCategoryAsync(id);
        return RedirectToAction("Index", "Category", new { Area = "Admin" });
        /*
        var name = await categoryService.UndoDeleteCategoryAsync(categoryId);
            toast.AddSuccessToastMessage(Messages.Category.Delete(name), new ToastrOptions() { Title = "İşlem Başarılı" });

            return RedirectToAction("Index", "Category", new { Area = "Admin" });
        */
    }
}
