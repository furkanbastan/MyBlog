using MyBlog.Entity.Entities.Common;
using MyBlog.Entity.Entities.Identity;

namespace MyBlog.Entity.Entities;

public class Article : EntityBase
{
    public string Title { get; set; }
    public string Content { get; set; }
    public int ViewCount { get; set; } = 0;

    public Guid? ImageId { get; set; }
    public Image Image { get; set; }

    public Guid CategoryId { get; set; }
    public Category Category { get; set; }

    public Guid UserId { get; set; }
    public AppUser User { get; set; }

    public ICollection<ArticleVisitor> ArticleVisitors { get; set; }
}
