using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Common.Repositories;

public class SqlServerBaseRepository<TEntity> : IRepository<TEntity> where TEntity : class, new()
{
    private readonly ApplicationDbContext _applicationDbContext;

    public SqlServerBaseRepository(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public TEntity[] GetList(int? offset = null, int? limit = null, Expression<Func<TEntity, bool>>? predicate = null, Expression<Func<TEntity, object>>? orderBy = null,
        bool? descending = null)
    {
        var queryable = _applicationDbContext.Set<TEntity>().AsQueryable();

        if (predicate is not null)
        {
            queryable = queryable.Where(predicate);
        }

        if (orderBy is not null)
        {
            queryable = descending == true ? queryable.OrderByDescending(orderBy) : queryable.OrderBy(orderBy);
        }

        if (offset.HasValue)
        {
            queryable = queryable.Skip(offset.Value);
        }

        if (limit.HasValue)
        {
            queryable = queryable.Take(limit.Value);
        }

        return queryable.ToArray();
    }

    public TEntity? SingleOrDefault(Expression<Func<TEntity, bool>>? predicate = null)
    {
        var set = _applicationDbContext.Set<TEntity>();
        return predicate == null ? set.SingleOrDefault() : set.SingleOrDefault(predicate);
    }

    public async Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        var set = _applicationDbContext.Set<TEntity>();
        return predicate == null ? await set.SingleOrDefaultAsync(cancellationToken) : await set.SingleOrDefaultAsync(predicate, cancellationToken);
    }

    public int Count(Expression<Func<TEntity, bool>>? predicate = null)
    {
        var set = _applicationDbContext.Set<TEntity>();
        return predicate == null ? set.Count() : set.Count(predicate);
    }

    public TEntity Add(TEntity book)
    {
        var set = _applicationDbContext.Set<TEntity>();
        set.Add(book);
        _applicationDbContext.SaveChanges();
        return book;
    }

    public TEntity Update(TEntity book)
    {
        var set = _applicationDbContext.Set<TEntity>();
        set.Update(book);
        _applicationDbContext.SaveChanges();
        return book;
    }

    public bool Delete(TEntity book)
    {
        var set = _applicationDbContext.Set<TEntity>();
        set.Remove(book);
        return _applicationDbContext.SaveChanges() > 0;
    }
}