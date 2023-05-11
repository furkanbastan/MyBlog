using System.Reflection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using MyBlog.Data.Mappings;
using MyBlog.Entity.Entities;
using MyBlog.Entity.Entities.Identity;

namespace MyBlog.Data.Contexts;

public class MyBlogDbContext : IdentityDbContext<AppUser, AppRole, Guid, AppUserClaim, AppUserRole, AppUserLogin, AppRoleClaim, AppUserToken>
{
    public MyBlogDbContext(DbContextOptions<MyBlogDbContext> options) : base(options)
    {
        
    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source = C:\\Users\\bbast\\OneDrive\\Masaüstü\\MyBlog\\MyBlog.Data\\DB\\MyBlog.db;");
        base.OnConfiguring(optionsBuilder);
    }
    public DbSet<Article> Articles { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Image> Images { get; set; }
    public DbSet<Visitor> Visitors { get; set; }
    public DbSet<ArticleVisitor> ArticleVisitors { get; set; }
}

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<MyBlogDbContext>
{
    public MyBlogDbContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<MyBlogDbContext> dbContextOptionsBuilder = new();
        dbContextOptionsBuilder.UseSqlite("Data Source = C:\\Users\\bbast\\OneDrive\\Masaüstü\\MyBlog\\MyBlog.Data\\DB\\MyBlog.db;");
        return new(dbContextOptionsBuilder.Options);
    }
}
