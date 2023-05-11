using MyBlog.Entity.Entities.Common;

namespace MyBlog.Entity.Entities;

public class ArticleVisitor : IEntityBase
{
    public int VisitorId { get; set; }
    public Visitor Visitor { get; set; }
    public Guid ArticleId { get; set; }
    public Article Article { get; set; }
}
