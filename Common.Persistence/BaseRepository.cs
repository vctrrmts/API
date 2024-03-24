using System.Linq.Expressions;
using Common.Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Common.Persistence;

public class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : class, new()
{
    private readonly ApplicationDbContext _applicationDbContext;

    public BaseRepository(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public async Task<TEntity[]> GetListAsync(int? offset = null, int? limit = null, Expression<Func<TEntity, bool>>? predicate = null,
        Expression<Func<TEntity, object>>? orderBy = null, bool? descending = null, CancellationToken cancellationToken = default)
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

        return await queryable.ToArrayAsync(cancellationToken);
    }

    public async Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        var set = _applicationDbContext.Set<TEntity>();
        return predicate == null ? await set.SingleOrDefaultAsync(cancellationToken) : await set.SingleOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>>? predicate, CancellationToken cancellationToken = default)
    {
        var set = _applicationDbContext.Set<TEntity>();
        return predicate == null ? await set.SingleAsync(cancellationToken) : await set.SingleAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        var queryable = _applicationDbContext.Set<TEntity>().AsQueryable();

        if (predicate is not null)
        {
            queryable = queryable.Where(predicate);
        }

        return await queryable.CountAsync(cancellationToken);
    }

    public async Task<TEntity> AddAsync(TEntity book, CancellationToken cancellationToken = default)
    {
        var set = _applicationDbContext.Set<TEntity>();
        await set.AddAsync(book, cancellationToken);
        await _applicationDbContext.SaveChangesAsync(cancellationToken);
        return book;
    }

    public async Task<TEntity> UpdateAsync(TEntity book, CancellationToken cancellationToken = default)
    {
        var set = _applicationDbContext.Set<TEntity>();
        set.Update(book);
        await _applicationDbContext.SaveChangesAsync(cancellationToken);
        return book;
    }

    public async Task<bool> DeleteAsync(TEntity book, CancellationToken cancellationToken = default)
    {
        var set = _applicationDbContext.Set<TEntity>();
        set.Remove(book);
        return await _applicationDbContext.SaveChangesAsync(cancellationToken) > 0;
    }
}