using System.Security.Claims;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using MyBlog.Data.UnitOfWork;
using MyBlog.Entity.DTOs.Users;
using MyBlog.Entity.Entities;
using MyBlog.Entity.Entities.Identity;
using MyBlog.Entity.Enums;
using MyBlog.Service.Extensions;
using MyBlog.Service.Helpers.Images;
using MyBlog.Service.Services.Abstractions;

namespace MyBlog.Service.Services.Concrete;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IImageHelper _imageHelper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ClaimsPrincipal _user;
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly IValidator<AppUser> _validator;
    public UserService(IUnitOfWork unitOfWork, IMapper mapper, IImageHelper imageHelper, IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<AppRole> roleManager, IValidator<AppUser> validator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _imageHelper = imageHelper;
        _httpContextAccessor = httpContextAccessor;
        _user = httpContextAccessor.HttpContext!.User;
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _validator = validator;
    }
    public async Task<IdentityResult> CreateUserAsync(UserAddDto userAddDto, ModelStateDictionary modelState)
    {
        var map = _mapper.Map<AppUser>(userAddDto);
        map.UserName = userAddDto.Email;
        var validResult = await _validator.ValidateAsync(map);
        var identityResult = await _userManager.CreateAsync(map, string.IsNullOrEmpty(userAddDto.Password) ? "" : userAddDto.Password);

        if (modelState.IsValid && identityResult.Succeeded)
        {
            var findRole = await _roleManager.FindByIdAsync(userAddDto.RoleId.ToString());
            await _userManager.AddToRoleAsync(map, findRole!.ToString());
            return identityResult;
        }
        else
        {
            identityResult.AddToIdentityModelState(modelState);
            validResult.AddToModelState(modelState);
            return identityResult;
        }
    }

    public async Task<AppUser?> GetAppUserByIdAsync(Guid id)
        => await _userManager.FindByIdAsync(id.ToString());

    public async Task<List<AppRole>> GetAllRolesAsync()
    {
        return await _roleManager.Roles.ToListAsync();
    }
    public async Task<(IdentityResult identityResult, string? email)> DeleteUser(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        var result = await _userManager.DeleteAsync(user!);
        if (result.Succeeded)
            return (result, user!.Email);
        else
            return (result, null);
    }
    public async Task<List<UserDto>> GetAllUsersWithRoleAsync()
    {
        var users = await _userManager.Users.ToListAsync();
        var map = _mapper.Map<List<UserDto>>(users);

        foreach (var user in map)
        {
            var findUser = await _userManager.FindByIdAsync(user.Id.ToString());
            var role = string.Join("", await _userManager.GetRolesAsync(findUser!));

            user.Role = role;
        }

        return map;
    }
    public async Task<string> GetUserRoleAsync(AppUser user)
    {
        return string.Join("",await _userManager.GetRolesAsync(user));
    }
    public async Task<IdentityResult?> UpdateUserAsync(UserUpdateDto userUpdateDto, ModelStateDictionary modelState)
    {
        var user = await _userManager.FindByIdAsync(userUpdateDto.Id.ToString());

        if(user != null)
        {
            var userRole = string.Join("", await _userManager.GetRolesAsync(user));
            var map = _mapper.Map(userUpdateDto, user);
            var validResult = await _validator.ValidateAsync(map);

            if(validResult.IsValid)
            {
                map.UserName = userUpdateDto.Email;
                map.SecurityStamp = Guid.NewGuid().ToString();
                var identityResult = await _userManager.UpdateAsync(map);

                if (identityResult.Succeeded)
                {
                    await _userManager.RemoveFromRoleAsync(user, userRole);
                    var findRole = await _roleManager.FindByIdAsync(userUpdateDto.RoleId.ToString());
                    identityResult = await _userManager.AddToRoleAsync(user!, findRole!.ToString());
                    return identityResult;
                }
                else
                {
                    identityResult.AddToIdentityModelState(modelState);
                }
            }
            else
            {
                validResult.AddToModelState(modelState);
            }
        }
        return null;
    }
    public async Task<UserProfileDto> GetUserProfileAsync()
    {
        var userId = _user.GetLoggedInUserId();
        
        var getUserWithImage = await _unitOfWork.GetRepository<AppUser>().GetAsync(x => x.Id == userId, x => x.Image);
        var map = _mapper.Map<UserProfileDto>(getUserWithImage);
        map.Image.FileName = getUserWithImage.Image.FileName;

        return map;
    }
    private async Task<Guid> UploadImageForUser(UserProfileDto userProfileDto)
    {
        var userEmail = _user.GetLoggedInEmail();

        var imageUpload = await _imageHelper.Upload($"{userProfileDto.FirstName}{userProfileDto.LastName}", userProfileDto.Photo, ImageType.User);
        Image image = new(){FileName = imageUpload.FullName, FileType = userProfileDto.Photo.ContentType, CreatedBy = userEmail};
        await _unitOfWork.GetRepository<Image>().AddAsync(image);

        return image.Id;
    }

    public async Task<bool> UserProfileUpdateAsync(UserProfileDto userProfileDto)
    {
        var userId = _user.GetLoggedInUserId();
        var user = await _userManager.FindByIdAsync(userId.ToString());

        var isVerified = await _userManager.CheckPasswordAsync(user!, userProfileDto.CurrentPassword);
        if (isVerified && userProfileDto.NewPassword != null)
        {
            var result = await _userManager.ChangePasswordAsync(user!, userProfileDto.CurrentPassword, userProfileDto.NewPassword);
            if (result.Succeeded)
            {
                await _userManager.UpdateSecurityStampAsync(user!);
                await _signInManager.SignOutAsync();
                await _signInManager.PasswordSignInAsync(user!, userProfileDto.NewPassword, true, false);

                _mapper.Map(userProfileDto, user);

                if(userProfileDto.Photo != null)
                    user!.ImageId = await UploadImageForUser(userProfileDto);

                await _userManager.UpdateAsync(user!);
                await _unitOfWork.SaveAsync();

                return true;
            }
            else
                return false;
        }
        else if (isVerified)
        {
            await _userManager.UpdateSecurityStampAsync(user!);
            _mapper.Map(userProfileDto, user);

            if (userProfileDto.Photo != null)
                user!.ImageId = await UploadImageForUser(userProfileDto);

            await _userManager.UpdateAsync(user!);
            await _unitOfWork.SaveAsync();
            return true;
        }
        else
            return false;
    }
}
