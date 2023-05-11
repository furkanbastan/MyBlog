using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MyBlog.Data;
using MyBlog.Service.FluentValidations;
using MyBlog.Service.Helpers.Images;
using MyBlog.Service.Services.Abstractions;
using MyBlog.Service.Services.Concrete;

namespace MyBlog.Service;

public static class ServiceRegistration
{
    public static void AddServiceLayerServices(this IServiceCollection services)
    {
        services.AddScoped<IArticleService, ArticleService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IImageHelper, ImageHelper>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IDashboardService, DashboardService>();

        services.AddDataServices();

        services.AddAutoMapper(typeof(AutoMapper.MappingProfile));

        services
            .AddFluentValidationAutoValidation(opt => 
            {
                opt.DisableDataAnnotationsValidation = true;
                //opt.ValidatorOptions.LanguageManager.Culture = new CultureInfo("tr");
            })
            .AddFluentValidationClientsideAdapters()
            .AddValidatorsFromAssemblyContaining<AppUserValidator>();
        
        services.AddSingleton<IHttpContextAccessor,HttpContextAccessor>();
    }
}
