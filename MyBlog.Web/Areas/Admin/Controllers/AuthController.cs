using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyBlog.Entity.DTOs.Users;
using MyBlog.Entity.Entities.Identity;

namespace MyBlog.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class AuthController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    public AuthController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }
    [HttpGet]
    public IActionResult Login()
    {
        if(User.Identity!.IsAuthenticated)
        {
            return RedirectToAction("Index", "Home", new{ Area = "Admin" });
        }
        return View();
    }
    
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Login(UserLoginDto userLoginDto)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(userLoginDto.Email);
            if(user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, userLoginDto.Password, userLoginDto.RememberMe, false);
                if(result.Succeeded)
                {
                    return RedirectToAction("Index", "Home", new{Area = "Admin"});
                }
                else
                {
                    ModelState.AddModelError("","E-Posta veya şifre hatalı...1");
                    return View();
                }
            }
            else
            {
                ModelState.AddModelError("","E-Posta veya şifre hatalı...2");
                return View();
            }
        }
        else
        {
            ModelState.AddModelError("","E-Posta veya şifre hatalı...3");
            return View();
        }
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home", new{ Area = "" });
    }

    [Authorize]
    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}
