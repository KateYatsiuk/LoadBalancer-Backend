using LoadBalancer.DAL.DTOs;
using LoadBalancer.DAL.DTOs.CalculationDtos;
using LoadBalancer.DAL.Entities;
using Microsoft.AspNetCore.Http;

namespace LoadBalancer.BLL.Interfaces
{
    public interface ITrigonometryService
    {
        Task<TrigonometryCalculationResultDto> CalculateTrigonometryAsync(TrigonometryRequestDto request, int userId);
        void CancelTrigonometryCalculation();
        Task<IEnumerable<TrigonometryCalculation>> GetCalculationsByUserIdAsync(int userId);
        Task<int> GetUserOrUserIdHereAsync(HttpRequest request);
    }
}
