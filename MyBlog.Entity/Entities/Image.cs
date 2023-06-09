using MyBlog.Entity.Entities.Common;
using MyBlog.Entity.Entities.Identity;

namespace MyBlog.Entity.Entities;

public class Image : EntityBase
{
    public Image()
    {
        Articles = new HashSet<Article>();
        Users = new HashSet<AppUser>();
    }
    public string FileType { get; set; }
    public string FileName { get; set; }

    public ICollection<Article> Articles { get; set; }
    public ICollection<AppUser> Users { get; set; }
}
