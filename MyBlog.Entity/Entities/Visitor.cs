using MyBlog.Entity.Entities.Common;

namespace MyBlog.Entity.Entities;

public class Visitor : IEntityBase
{
    public int Id { get; set; }
    public string IpAddress { get; set; }
    public string UserAgent { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public ICollection<ArticleVisitor> ArticleVisitors { get; set; }
}
