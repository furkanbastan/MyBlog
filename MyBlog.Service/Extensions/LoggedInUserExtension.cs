using System.Security.Claims;

namespace MyBlog.Service.Extensions;

public static class LoggedInUserExtension
{
    public static Guid GetLoggedInUserId(this ClaimsPrincipal principal) //get user id in logged => bu isim olabilir
    {
        return Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.NewGuid().ToString());
    }
    public static string GetLoggedInEmail(this ClaimsPrincipal principal)
    {
        return principal.FindFirstValue(ClaimTypes.Email) ?? "";
    }
}
