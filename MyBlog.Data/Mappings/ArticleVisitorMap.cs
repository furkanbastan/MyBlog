using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyBlog.Entity.Entities;

namespace MyBlog.Data.Mappings;

public class ArticleVisitorMap : IEntityTypeConfiguration<ArticleVisitor>
{
    public void Configure(EntityTypeBuilder<ArticleVisitor> builder)
    {
        builder.HasKey(av => new {av.ArticleId, av.VisitorId});
    }
}
