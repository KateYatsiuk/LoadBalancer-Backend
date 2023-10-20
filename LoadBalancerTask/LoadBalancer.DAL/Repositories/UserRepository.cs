using LoadBalancer.DAL.Entities;
using LoadBalancer.DAL.Persistence;
using LoadBalancer.DAL.Repositories.Interfaces;

namespace LoadBalancer.DAL.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(BalancerDbContext context)
        : base(context)
        {
        }
    }
}
