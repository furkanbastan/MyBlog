using Microsoft.AspNetCore.Mvc.ModelBinding;
using MyBlog.Entity.DTOs.Articles;
using MyBlog.Entity.Entities;

namespace MyBlog.Service.Services.Abstractions;

public interface IArticleService
{
    public Task<ArticleListDto> GetAllByPagingAsync(Guid? categoryId, int currentPage = 1, int pageSize = 3, bool isAscending = false);
    public Task CreateArticleAsync(ArticleAddDto articleAddDto, ModelStateDictionary modelState);
    public Task<List<ArticleDto>> GetAllArticlesWithCategoryNonDeletedAsync();
    public Task<ArticleDto> GetArticleWithCategoryNonDeletedAsync(Guid id);
    public Task ArticleUpdateAsync(ArticleUpdateDto articleUpdateDto, ModelStateDictionary modelState);
    public Task<string> SafeDeleteArticleAsync(Guid id);
    public Task<List<ArticleDto>> GetAllArticlesWithCategoryDeletedAsync();
    public Task<string> UndoDeleteArticleAsync(Guid id);
    public Task<ArticleListDto> SearchAsync(string keyword, int currentPage = 1, int pageSize = 3, bool isAscending = false);
}
