using AutoMapper;
using MyBlog.Entity.DTOs.Articles;
using MyBlog.Entity.DTOs.Categories;
using MyBlog.Entity.DTOs.Users;
using MyBlog.Entity.Entities;
using MyBlog.Entity.Entities.Identity;

namespace MyBlog.Service.AutoMapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<AppUser, UserDto>().ReverseMap();
        CreateMap<AppUser, UserAddDto>().ReverseMap();
        CreateMap<AppUser, UserUpdateDto>().ReverseMap();
        CreateMap<AppUser, UserProfileDto>().ReverseMap();

        CreateMap<Category, CategoryDto>().ReverseMap();
        CreateMap<Category, CategoryAddDto>().ReverseMap();
        CreateMap<Category, CategoryUpdateDto>().ReverseMap();

        CreateMap<Article, ArticleListDto>().ReverseMap();
        CreateMap<Article, ArticleDto>().ReverseMap();
        CreateMap<Article, ArticleAddDto>().ReverseMap();
        CreateMap<Article, ArticleUpdateDto>().ReverseMap();
    }
}
