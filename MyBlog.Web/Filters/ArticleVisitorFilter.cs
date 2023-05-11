using Microsoft.AspNetCore.Mvc.Filters;
using MyBlog.Data.UnitOfWork;
using MyBlog.Entity.Entities;

namespace MyBlog.Web.Filters;

public class ArticleVisitorFilter : IAsyncActionFilter
{
    private readonly IUnitOfWork _uof;

    public ArticleVisitorFilter(IUnitOfWork uof)
    {
        _uof = uof;
    }

    public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        string getIp = context.HttpContext.Connection.RemoteIpAddress!.MapToIPv4().ToString();
        string getUserAgent = context.HttpContext.Request.Headers["User-Agent"].ToString();
        
        var visitors = _uof.GetRepository<Visitor>().GetAllAsync().Result;

        if(visitors.Any(v => v.IpAddress == getIp))
            return next();
        else
        {
            Visitor visitor = new(){IpAddress = getIp, UserAgent = getUserAgent};
            _uof.GetRepository<Visitor>().AddAsync(visitor);
            _uof.Save();
        }

        return next();
    }
}
