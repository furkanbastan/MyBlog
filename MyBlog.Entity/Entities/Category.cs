using MyBlog.Entity.Entities.Common;

namespace MyBlog.Entity.Entities;

public class Category : EntityBase
{
    public Category()
    {
        Articles = new HashSet<Article>();
    }
    public string Name { get; set; }
    public ICollection<Article> Articles { get; set; }
}
