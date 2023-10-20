using LoadBalancer.DAL.Entities;
using LoadBalancer.DAL.Persistence;
using LoadBalancer.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LoadBalancer.DAL.Repositories
{
    public class TrigonometryRepository : ITrigonometryRepository
    {
        private readonly BalancerDbContext _db;
        internal DbSet<TrigonometryCalculation> dbSet;

        public TrigonometryRepository(BalancerDbContext db)
        {
            _db = db;
            dbSet = _db.Set<TrigonometryCalculation>();
        }

        public async Task<IEnumerable<TrigonometryCalculation>> GetCalculationsByUserIdAsync(int userId)
        {
            var calculations = await dbSet
                .Where(c => c.UserId == userId)
                .ToListAsync();

            return calculations;
        }

        public void AddResult(TrigonometryCalculation calculation)
        {
            dbSet.Add(calculation);
            _db.SaveChanges();
        }
    }
}
