using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Data.Repositories
{
    public class EfRepository<T> : IRepository<T> where T : class
    {
        private readonly FumhContext _ctx;
        private readonly DbSet<T> _set;

        public EfRepository(FumhContext ctx)
        {
            _ctx = ctx;
            _set = ctx.Set<T>();
        }

        public IQueryable<T> Query() => _set.AsQueryable();

        public Task<T?> GetAsync(params object[] keys) =>
            _set.FindAsync(keys).AsTask();

        public Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate) =>
            _set.FirstOrDefaultAsync(predicate);

        public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate) =>
            _set.AnyAsync(predicate);

        public Task AddAsync(T entity) => _set.AddAsync(entity).AsTask();

        public Task AddRangeAsync(IEnumerable<T> entities) =>
            _set.AddRangeAsync(entities);

        public void Update(T entity) => _set.Update(entity);

        public void Remove(T entity) => _set.Remove(entity);

        public void RemoveRange(IEnumerable<T> entities) => _set.RemoveRange(entities);
    }
}
