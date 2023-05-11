using MyBlog.Data.Repositories;
using MyBlog.Entity.Entities.Common;

namespace MyBlog.Data.UnitOfWork;

public interface IUnitOfWork : IAsyncDisposable
{
    IRepository<T> GetRepository<T>() where T : class,IEntityBase, new();
    Task<int> SaveAsync();
    int Save();
}
