using MyBlog.Data.Contexts;
using MyBlog.Data.Repositories;

namespace MyBlog.Data.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly MyBlogDbContext dbContext;

    public UnitOfWork(MyBlogDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async ValueTask DisposeAsync()
    {
        await dbContext.DisposeAsync();
    }

    public int Save()
    {
        return dbContext.SaveChanges();
    }

    public async Task<int> SaveAsync()
    {
        return await dbContext.SaveChangesAsync();
    }

    IRepository<T> IUnitOfWork.GetRepository<T>()
    {
        return new Repository<T>(dbContext);
    }
}
