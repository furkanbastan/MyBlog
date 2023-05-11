using System.Security.Claims;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MyBlog.Data.UnitOfWork;
using MyBlog.Entity.DTOs.Articles;
using MyBlog.Entity.Entities;
using MyBlog.Entity.Enums;
using MyBlog.Service.Extensions;
using MyBlog.Service.Helpers.Images;
using MyBlog.Service.Services.Abstractions;

namespace MyBlog.Service.Services.Concrete;

public class ArticleService : IArticleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ClaimsPrincipal _user;
    private readonly IImageHelper _imageHelper;
    private readonly IValidator<Article> _validator;
    public ArticleService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor, IImageHelper imageHelper, IValidator<Article> validator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _user = httpContextAccessor.HttpContext!.User;
        _imageHelper = imageHelper;
        _validator = validator;
    }
    public async Task<ArticleListDto> GetAllByPagingAsync(Guid? categoryId, int currentPage = 1, int pageSize = 3, bool isAscending = false)
    {
        pageSize = pageSize > 20 ? 20 : pageSize;
        var articles = categoryId == null 
            ? await _unitOfWork.GetRepository<Article>().GetAllAsync(a => !a.IsDeleted, a => a.Category, i => i.Image, u => u.User)
            : await _unitOfWork.GetRepository<Article>().GetAllAsync(a => a.CategoryId == categoryId && !a.IsDeleted, a => a.Category, i => i.Image, u => u.User);
        
        var sortedArticles = isAscending
            ? articles.OrderBy(a => a.CreatedDate).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList()
            : articles.OrderByDescending(a => a.CreatedDate).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
        return new ArticleListDto
        {
            Articles = sortedArticles,
            CategoryId = categoryId == null ? null : categoryId.Value,
            CurrentPage = currentPage,
            PageSize = pageSize,
            TotalCount = articles.Count,
            IsAscending = isAscending
        };
    }
    public async Task CreateArticleAsync(ArticleAddDto articleAddDto, ModelStateDictionary modelState)
    {
        var userId = _user.GetLoggedInUserId();
        var userEmail = _user.GetLoggedInEmail();

        var map = _mapper.Map<Article>(articleAddDto);
        var result = await _validator.ValidateAsync(map);

        if (result.IsValid)
        {
            var imageUpload = await _imageHelper.Upload(articleAddDto.Title, articleAddDto.Photo, ImageType.Post);
            Image image = new()
            {
                FileName = imageUpload.FullName,
                FileType = articleAddDto.Photo.ContentType,
                CreatedBy = userEmail
            };

            await _unitOfWork.GetRepository<Image>().AddAsync(image);

            Article article = new()
            {
                Title = articleAddDto.Title,
                Content = articleAddDto.Content,
                CreatedBy = userEmail,
                CategoryId = articleAddDto.CategoryId,
                ImageId = image.Id,
                UserId = userId
            };

            await _unitOfWork.GetRepository<Article>().AddAsync(article);
            await _unitOfWork.SaveAsync();
        }
        else
        {
            result.AddToModelState(modelState);
        }
    }

    public async Task<List<ArticleDto>> GetAllArticlesWithCategoryNonDeletedAsync()
    {
        var article = await _unitOfWork.GetRepository<Article>().GetAllAsync(a => !a.IsDeleted, a => a.Category, a => a.Image);
        var map = _mapper.Map<List<ArticleDto>>(article);
        return map;
    }

    public async Task<ArticleDto> GetArticleWithCategoryNonDeletedAsync(Guid id)
    {
        var article = await _unitOfWork.GetRepository<Article>().GetAsync(a=> !a.IsDeleted && a.Id == id, a => a.Category, a => a.Image, a => a.User);
        var map = _mapper.Map<ArticleDto>(article);
        return map;
    }
    public async Task ArticleUpdateAsync(ArticleUpdateDto articleUpdateDto, ModelStateDictionary modelState)
    {
        var userEmail = _user.GetLoggedInEmail();
        var article = await _unitOfWork.GetRepository<Article>().GetAsync(a => !a.IsDeleted && a.Id == articleUpdateDto.Id, a => a.Category, a => a.Image);

        if(articleUpdateDto.Photo != null)
        {
            _imageHelper.Delete(articleUpdateDto.Image.FileName);
            var imageUpload = await _imageHelper.Upload(articleUpdateDto.Title, articleUpdateDto.Photo, ImageType.Post);

            Image image = new()
            {
                CreatedBy = userEmail,
                FileName = imageUpload.FullName,
                FileType = articleUpdateDto.Photo.ContentType
            };
            await _unitOfWork.GetRepository<Image>().AddAsync(image);

            article.ImageId = image.Id;
        }

        _mapper.Map(articleUpdateDto, article);
        var result = await _validator.ValidateAsync(article);

        if (result.IsValid)
        {
            article.ModifiedDate = DateTime.Now;
            article.ModifiedBy = userEmail;

            await _unitOfWork.GetRepository<Article>().UpdateAsync(article);
            await _unitOfWork.SaveAsync();
        }
        else
        {
            result.AddToModelState(modelState);
        }
        
    }
    public async Task<string> SafeDeleteArticleAsync(Guid id)
    {
        var userEmail = _user.GetLoggedInEmail();
        var article = await _unitOfWork.GetRepository<Article>().GetByGuidAsync(id);

        article.IsDeleted = true;
        article.DeletedBy = userEmail;
        article.DeletedDate = DateTime.Now;

        await _unitOfWork.GetRepository<Article>().UpdateAsync(article);
        await _unitOfWork.SaveAsync();
        return article.Title;
    }
    public async Task<List<ArticleDto>> GetAllArticlesWithCategoryDeletedAsync()
    {
        var articles = await _unitOfWork.GetRepository<Article>().GetAllAsync(a => a.IsDeleted, a => a.Category);
        var map = _mapper.Map<List<ArticleDto>>(articles);

        return map;
    }
    public async Task<string> UndoDeleteArticleAsync(Guid id)
    {
        var article = await _unitOfWork.GetRepository<Article>().GetByGuidAsync(id);

        article.IsDeleted = false;
        article.DeletedDate = null;
        article.DeletedBy = null;

        await _unitOfWork.GetRepository<Article>().UpdateAsync(article);
        await _unitOfWork.SaveAsync();

        return article.Title;
    }
    public async Task<ArticleListDto> SearchAsync(string keyword, int currentPage = 1, int pageSize = 3, bool isAscending = false)
    {
        pageSize = pageSize > 20 ? 20 : pageSize;
        var articles = await _unitOfWork.GetRepository<Article>().GetAllAsync(
            a => !a.IsDeleted && (a.Title.Contains(keyword) || a.Content.Contains(keyword) || a.Category.Name.Contains(keyword)),
            a => a.Category, i => i.Image, u => u.User);
            
        var sortedArticles = isAscending
            ? articles.OrderBy(a => a.CreatedDate).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList()
            : articles.OrderByDescending(a => a.CreatedDate).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
        return new ArticleListDto
        {
            Articles = sortedArticles,
            CurrentPage = currentPage,
            PageSize = pageSize,
            TotalCount = articles.Count,
            IsAscending = isAscending
        };
    }
}
