using Microsoft.AspNetCore.Http;
using MyBlog.Entity.DTOs.Categories;
using MyBlog.Entity.Entities;

namespace MyBlog.Entity.DTOs.Articles;

public class ArticleUpdateDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public Guid CategoryId { get; set; }

    public Image Image { get; set; }
    public IFormFile Photo { get; set; }
    public IList<CategoryDto> Categories { get; set; }
}
