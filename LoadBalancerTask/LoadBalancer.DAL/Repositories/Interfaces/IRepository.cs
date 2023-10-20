using System.Linq.Expressions;

namespace LoadBalancer.DAL.Repositories.Interfaces
{
    public interface IRepository<T>
        where T : class
    {
        IEnumerable<T> GetAll();

        Task<T> Get(Expression<Func<T, bool>> filter);

        Task Create(T entity);

        Task Delete(T entity);

        Task Update(T obj);
    }
}
