using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyBlog.Entity.Entities.Identity;

namespace MyBlog.Data.Mappings;

public class AppUserTokenMap : IEntityTypeConfiguration<AppUserToken>
{
    public void Configure(EntityTypeBuilder<AppUserToken> builder)
    {
        builder.HasKey(t => new { t.UserId, t.LoginProvider, t.Name });

        // Limit the size of the composite key columns due to common DB restrictions
        builder.Property(t => t.LoginProvider);
        builder.Property(t => t.Name);

        // Maps to the AspNetUserTokens table
        builder.ToTable("AspNetUserTokens");
    }
}
