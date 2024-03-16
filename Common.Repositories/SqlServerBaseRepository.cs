using System;
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

    public async Task<TEntity[]> GetListAsync(int? offset = null, int? limit = null, List<Expression<Func<TEntity, bool>>>? predicates = null, 
        Expression<Func<TEntity, object>>? orderBy = null, bool? descending = null, CancellationToken cancellationToken = default)
    {
        var queryable = _applicationDbContext.Set<TEntity>().AsQueryable();

        if (predicates is not null)
        {
            foreach (var expression in predicates)
            {
                queryable = queryable.Where(expression);
            }
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

    public async Task<int> CountAsync(List<Expression<Func<TEntity, bool>>>? predicates = null, CancellationToken cancellationToken = default)
    {
        var queryable = _applicationDbContext.Set<TEntity>().AsQueryable();

        if (predicates is not null)
        {
            foreach (var expression in predicates)
            {
                queryable = queryable.Where(expression);
            }
        }
        return  await queryable.CountAsync(cancellationToken);
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