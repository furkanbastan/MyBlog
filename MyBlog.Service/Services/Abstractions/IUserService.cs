using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MyBlog.Entity.DTOs.Users;
using MyBlog.Entity.Entities.Identity;

namespace MyBlog.Service.Services.Abstractions;

public interface IUserService
{
    public Task<AppUser?> GetAppUserByIdAsync(Guid id);
    public Task<IdentityResult> CreateUserAsync(UserAddDto userAddDto, ModelStateDictionary modelState);
    public Task<List<AppRole>> GetAllRolesAsync();
    public Task<(IdentityResult identityResult, string? email)> DeleteUser(Guid id);
    public Task<List<UserDto>> GetAllUsersWithRoleAsync();
    public Task<string> GetUserRoleAsync(AppUser user);
    public Task<IdentityResult?> UpdateUserAsync(UserUpdateDto userUpdateDto, ModelStateDictionary modelState);
    public Task<UserProfileDto> GetUserProfileAsync();
    public Task<bool> UserProfileUpdateAsync(UserProfileDto userProfileDto);
}
