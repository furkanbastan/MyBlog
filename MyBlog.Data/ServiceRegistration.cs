using Microsoft.Extensions.DependencyInjection;
using MyBlog.Data.Contexts;
using MyBlog.Data.Repositories;
using MyBlog.Data.UnitOfWork;

namespace MyBlog.Data;

public static class ServiceRegistration
{
    public static void AddDataServices(this IServiceCollection services)
    {
        services.AddDbContext<MyBlogDbContext>();

        services.AddScoped(typeof(IRepository<>),typeof(Repository<>));

        services.AddScoped<IUnitOfWork,MyBlog.Data.UnitOfWork.UnitOfWork>();

        //services.AddIdentityCore<>
    }
}
