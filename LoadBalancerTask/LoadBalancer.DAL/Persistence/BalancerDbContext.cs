using LoadBalancer.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace LoadBalancer.DAL.Persistence
{
    public class BalancerDbContext : DbContext
    {
        public BalancerDbContext(DbContextOptions<BalancerDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<TrigonometryCalculation> TrigonometryCalculations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity => 
            { 
                entity.HasIndex(e => e.Email).IsUnique();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
