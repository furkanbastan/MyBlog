using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MyBlog.Entity.DTOs.Users;
using MyBlog.Service.Services.Abstractions;
using MyBlog.Web.ResultMessages;
using NToastNotify;

namespace MyBlog.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class UserController : Controller
{
    private readonly IUserService _userService;
    private readonly IToastNotification _toast;
    private readonly IMapper _mapper;
    public UserController(IUserService userService, IToastNotification toast, IMapper mapper)
    {
        _userService = userService;
        _toast = toast;
        _mapper = mapper;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _userService.GetAllUsersWithRoleAsync();
        return View(result);
    }

    [HttpGet]
    public async Task<IActionResult> Add()
    {
        var roles = await _userService.GetAllRolesAsync();
        return View(new UserAddDto{Roles = roles});
    }

    [HttpPost]
    public async Task<IActionResult> Add(UserAddDto userAddDto)
    {
        var result = await _userService.CreateUserAsync(userAddDto, this.ModelState);

        if(result.Succeeded && ModelState.IsValid)
        {
            _toast.AddSuccessToastMessage(Messages.Success.Add(userAddDto.FirstName+"adlı kullanıcı"), new ToastrOptions{Title = "Başarılı"});
            return RedirectToAction("Index", "User", new {Area = "Admin"});
        }
        else
        {
            _toast.AddWarningToastMessage(Messages.Warning.Add(userAddDto.FirstName+"adlı kullanıcı"), new ToastrOptions{Title = "Hata..!!!"});
            return View(new UserAddDto(){Roles = await _userService.GetAllRolesAsync()});
        }
    }

    [HttpGet]
    public async Task<IActionResult> Update(Guid id)
    {
        var user = await _userService.GetAppUserByIdAsync(id);
        if (user != null)
        {
            var map = _mapper.Map<UserUpdateDto>(user);
            map.Roles = await _userService.GetAllRolesAsync();
            return View(map);
        }
        else
        {
            _toast.AddWarningToastMessage(Messages.Warning.Add("Kullanıcı Bulunamadı..!!"), new ToastrOptions{Title = "Hata..!!!"});
            return RedirectToAction("Index", "User", new {Area = "Admin"});
        }
    }

    [HttpPost]
    public async Task<IActionResult> Update(UserUpdateDto userUpdateDto)
    {
        var result = await _userService.UpdateUserAsync(userUpdateDto, this.ModelState);
        if(result != null && result.Succeeded && ModelState.IsValid)
        {
            _toast.AddSuccessToastMessage(Messages.Success.Update(userUpdateDto.Email), new ToastrOptions { Title = "İşlem Başarılı" });
            return RedirectToAction("Index", "User", new { Area = "Admin" });
        }
        else
        {
            _toast.AddWarningToastMessage(Messages.Warning.Add("Kullanıcı Bulunamadı..!!"), new ToastrOptions{Title = "Hata..!!!"});
            return View(new UserUpdateDto { Roles = userUpdateDto.Roles });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _userService.DeleteUser(id);
        if (result.identityResult.Succeeded)
        {
            _toast.AddSuccessToastMessage(Messages.Success.Delete(result.email!), new ToastrOptions { Title = "İşlem Başarılı" });
            return RedirectToAction("Index", "User", new { Area = "Admin" });
        }
        return NotFound();
    }
    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        var profile = await _userService.GetUserProfileAsync();

        return View(profile);
    }
    [HttpPost]
    public async Task<IActionResult> Profile(UserProfileDto userProfileDto)
    {

        if (ModelState.IsValid)
        {
            var result = await _userService.UserProfileUpdateAsync(userProfileDto);
            if (result)
            {
                _toast.AddSuccessToastMessage("Profil güncelleme işlemi tamamlandı", new ToastrOptions { Title = "İşlem Başarılı" });
                return RedirectToAction("Index", "Home", new { Area = "Admin" });
            }
            else
            {
                var profile = await _userService.GetUserProfileAsync();
                _toast.AddErrorToastMessage("Profil güncelleme işlemi tamamlanamadı", new ToastrOptions { Title = "İşlem Başarısız" });
                return View(profile);
            }
        }
        else
            return NotFound();
    }
}
