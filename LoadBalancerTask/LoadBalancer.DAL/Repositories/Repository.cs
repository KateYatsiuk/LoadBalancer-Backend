using LoadBalancer.DAL.Persistence;
using LoadBalancer.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LoadBalancer.DAL.Repositories
{
    public class Repository<T> : IRepository<T>
        where T : class
    {
        private readonly BalancerDbContext _db;
        internal DbSet<T> dbSet;

        public Repository(BalancerDbContext db)
        {
            _db = db;
            dbSet = _db.Set<T>();
        }

        public async Task Create(T entity)
        {
            _db.Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task Delete(T entity)
        {
            _db.Remove(entity);
            await _db.SaveChangesAsync();
        }

        public async Task<T> Get(Expression<Func<T, bool>> filter)
        {
            IQueryable<T> query = dbSet;
            query = query.Where(filter);
            return await query.FirstOrDefaultAsync();
        }

        public IEnumerable<T> GetAll()
        {
            IQueryable<T> query = dbSet;
            return query.ToList();
        }

        public async Task Update(T obj)
        {
            dbSet.Update(obj);
            await _db.SaveChangesAsync();
        }
    }
}
