using System.Linq.Expressions;

namespace Common.Repositories
{
    public interface IRepository<TEntity> where TEntity : class, new()
    {
        Task<TEntity[]> GetListAsync(
            int? offset = null,
            int? limit = null,
            List<Expression<Func<TEntity, bool>>>? expressions = null,
            Expression<Func<TEntity, object>>? orderBy = null,
            bool? descending = null,
            CancellationToken cancellationToken = default);
        Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>>? predicate, CancellationToken cancellationToken = default);
        Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>>? predicate, CancellationToken cancellationToken = default);
        Task<int> CountAsync(List<Expression<Func<TEntity, bool>>>? expressions = null, CancellationToken cancellationToken = default);
        Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
    }
}
