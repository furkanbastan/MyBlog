using Microsoft.AspNetCore.Mvc.ModelBinding;
using MyBlog.Entity.DTOs.Categories;
using MyBlog.Entity.Entities;

namespace MyBlog.Service.Services.Abstractions;

public interface ICategoryService
{
    public Task<List<CategoryDto>> GetAllCategoriesNonDeleted();
    public Task<List<CategoryDto>> GetAllCategoriesNonDeletedTake24();
    public Task CreateCategoryAsync(CategoryAddDto categoryAddDto, ModelStateDictionary modelState);
    public Task<Category> GetCategoryByGuid(Guid id);
    public Task<string> UpdateCategoryAsync(CategoryUpdateDto categoryUpdateDto, ModelStateDictionary modelState);
    public Task<string> SafeDeleteCategoryAsync(Guid id);
    public Task<List<CategoryDto>> GetAllCategoriesDeleted();
    public Task<string> UndoDeleteCategoryAsync(Guid id);
}
