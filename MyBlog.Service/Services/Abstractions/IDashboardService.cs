namespace MyBlog.Service.Services.Abstractions;

public interface IDashboardService
{
    public Task<List<int>> GetYearlyArticleCounts();
    public Task<int> GetTotalArticleCount();
    public Task<int> GetTotalCategoryCount();
}
