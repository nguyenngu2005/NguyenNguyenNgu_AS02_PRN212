using System.Linq.Expressions;

namespace Data.Repositories
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> Query();                  
        Task<T?> GetAsync(params object[] keys);       
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        void Update(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
    }
}
