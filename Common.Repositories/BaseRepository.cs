using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Common.Repositories
{
    public class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : class, new()
    {
        private readonly List<TEntity> _data = [];

        public TEntity[] GetList(int? offset = null, int? limit = null, Expression<Func<TEntity, bool>>? predicate = null,
            Expression<Func<TEntity, object>>? orderBy = null,
            bool? descending = null)
        {
            IEnumerable<TEntity> result = _data;

            if (predicate != null)
            {
                result = result.Where(predicate.Compile());
            }

            if (orderBy != null)
            {
                result = descending.GetValueOrDefault()
                    ? result.OrderByDescending(orderBy.Compile())
                    : result.OrderBy(orderBy.Compile());
            }

            result = result.Skip(offset.GetValueOrDefault());


            if (limit.HasValue)
            {
                result = result.Take(limit.Value);
            }

            return result.ToArray();
        }

        public TEntity? SingleOrDefault(Expression<Func<TEntity, bool>>? predicate = null)
        {
            if (predicate is null)
            {
                return _data.SingleOrDefault();
            }
            return _data.SingleOrDefault(predicate.Compile());
        }

        public int Count(Expression<Func<TEntity, bool>>? predicate = null)
        {
            IEnumerable<TEntity> entities = _data;
            if (predicate == null)
            {
                return entities.Count();
            }

            return entities.Where(predicate.Compile()).Count();
        }

        public TEntity Add(TEntity book)
        {
            _data.Add(book);
            return book;
        }

        public TEntity Update(TEntity book)
        {
            Delete(book);
            _data.Add(book);
            return book;
        }

        public bool Delete(TEntity book)
        {
            return _data.Remove(book);
        }
    }
}
