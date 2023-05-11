using Microsoft.AspNetCore.Identity;
using MyBlog.Data.Contexts;
using MyBlog.Entity.Entities.Identity;
using MyBlog.Service.Describers;
using MyBlog.Web.Filters;
using NToastNotify;

namespace MyBlog.Web;

public static class ServiceRegistration
{
    public static void AddWebLayerServices(this IServiceCollection services)
    {
        services.AddIdentity<AppUser,AppRole>(opt=>
        {
            opt.Password.RequireNonAlphanumeric = false;
            opt.Password.RequireLowercase = false;
            opt.Password.RequireUppercase = false;
        })
        .AddRoleManager<RoleManager<AppRole>>()
        .AddErrorDescriber<CustomIdentityErrorDescriber>()
        .AddEntityFrameworkStores<MyBlogDbContext>()
        .AddDefaultTokenProviders();
            
        services.ConfigureApplicationCookie(config =>
        {
            config.LoginPath = new PathString("/Admin/Auth/Login");
            config.LogoutPath = new PathString("/Admin/Auth/Logout");
            config.Cookie = new CookieBuilder
            {
                Name = "YoutubeBlog",
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                SecurePolicy = CookieSecurePolicy.SameAsRequest //Always 
            };
            config.SlidingExpiration = true;
            config.ExpireTimeSpan = TimeSpan.FromDays(7);
            config.AccessDeniedPath = new PathString("/Admin/Auth/AccessDenied");
        });

        services.AddControllersWithViews(opt => 
        {
            opt.Filters.Add<ArticleVisitorFilter>();
        })
        .AddNToastNotifyToastr(new NToastNotify.ToastrOptions()
        {
            PositionClass = ToastPositions.TopRight,
            TimeOut = 3000
        });
    }
}
