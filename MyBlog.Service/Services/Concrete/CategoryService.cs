using System.Security.Claims;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MyBlog.Data.UnitOfWork;
using MyBlog.Entity.DTOs.Categories;
using MyBlog.Entity.Entities;
using MyBlog.Service.Extensions;
using MyBlog.Service.Services.Abstractions;

namespace MyBlog.Service.Services.Concrete;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ClaimsPrincipal _user;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IValidator<Category> _validator;
    public CategoryService(IUnitOfWork unitOfWork, IMapper mapper, IValidator<Category> validator, IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _user = httpContextAccessor.HttpContext!.User;
        _validator = validator;
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task<List<CategoryDto>> GetAllCategoriesNonDeleted()
    {
        var categories = await _unitOfWork.GetRepository<Category>().GetAllAsync(c => !c.IsDeleted);
        var map = _mapper.Map<List<CategoryDto>>(categories);
        return map;
    }
    public async Task<List<CategoryDto>> GetAllCategoriesNonDeletedTake24()
    {
        var categories = await _unitOfWork.GetRepository<Category>().GetAllAsync(c => !c.IsDeleted);
        var map = _mapper.Map<List<CategoryDto>>(categories);
        return map.Take(24).ToList();
    }
    public async Task CreateCategoryAsync(CategoryAddDto categoryAddDto, ModelStateDictionary modelState)
    {
        var userEmail = _user.GetLoggedInEmail();
        var map = _mapper.Map<Category>(categoryAddDto);
        var result = await _validator.ValidateAsync(map);

        if (result.IsValid)
        {
            map.CreatedBy = userEmail;
            await _unitOfWork.GetRepository<Category>().AddAsync(map);
            await _unitOfWork.SaveAsync();
        }
        else
        {
            result.AddToModelState(modelState);
        }
    }
    public async Task<Category> GetCategoryByGuid(Guid id)
    {
        var category = await _unitOfWork.GetRepository<Category>().GetByGuidAsync(id);
        return category;
    }
    public async Task<string> UpdateCategoryAsync(CategoryUpdateDto categoryUpdateDto, ModelStateDictionary modelState)
    {
        var userEmail = _user.GetLoggedInEmail();
        var category = await _unitOfWork.GetRepository<Category>().GetByGuidAsync(categoryUpdateDto.Id);
        _mapper.Map(categoryUpdateDto,category);
        var result = await _validator.ValidateAsync(category);

        if(result.IsValid)
        {
            category.ModifiedBy = userEmail;
            category.ModifiedDate = DateTime.Now;
            await _unitOfWork.GetRepository<Category>().UpdateAsync(category);
            await _unitOfWork.SaveAsync();
            return category.Name;
        }
        else
        {
            result.AddToModelState(modelState);
            return category.Name;
        }
    }
    public async Task<string> SafeDeleteCategoryAsync(Guid id)
    {
        var userEmail = _user.GetLoggedInEmail();
        var category = await _unitOfWork.GetRepository<Category>().GetAsync(c => c.Id == id);

        category.IsDeleted = true;
        category.DeletedBy = userEmail;
        category.DeletedDate = DateTime.Now;

        await _unitOfWork.GetRepository<Category>().UpdateAsync(category);
        await _unitOfWork.SaveAsync();

        return category.Name;
    }
    public async Task<List<CategoryDto>> GetAllCategoriesDeleted()
    {
        var categories = await _unitOfWork.GetRepository<Category>().GetAllAsync(c => c.IsDeleted);
        var map = _mapper.Map<List<CategoryDto>>(categories);

        return map;
    }
    public async Task<string> UndoDeleteCategoryAsync(Guid id)
    {
        var category = await _unitOfWork.GetRepository<Category>().GetByGuidAsync(id);

        category.DeletedBy = "";
        category.DeletedDate = null;
        category.IsDeleted = false;

        await _unitOfWork.GetRepository<Category>().UpdateAsync(category);
        await _unitOfWork.SaveAsync();

        return category.Name;
    }
}
