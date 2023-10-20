using LoadBalancer.DAL.Entities;

namespace LoadBalancer.DAL.Repositories.Interfaces
{
    public interface ITrigonometryRepository
    {
        Task<IEnumerable<TrigonometryCalculation>> GetCalculationsByUserIdAsync(int userId);
        void AddResult(TrigonometryCalculation calculation);
    }
}
