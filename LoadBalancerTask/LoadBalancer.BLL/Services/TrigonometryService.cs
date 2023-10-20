using LoadBalancer.BLL.Interfaces;
using LoadBalancer.BLL.SignalR;
using LoadBalancer.DAL.DTOs;
using LoadBalancer.DAL.DTOs.CalculationDtos;
using LoadBalancer.DAL.Entities;
using LoadBalancer.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;

namespace LoadBalancer.BLL.Services
{
    public class TrigonometryService : ITrigonometryService
    {
        private readonly ITrigonometryRepository _trigonometryRepository;
        private readonly IJwtService _jwtService;
        private readonly IUserRepository _userRepository;
        private static CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private CancellationToken _cancellationToken = _cancellationTokenSource.Token;
        private readonly IHubContext<CalculationProgressHub> _hubContext;
        private int totalSum = 0;
        private bool bothToCalculate = false;

        public TrigonometryService(ITrigonometryRepository trigonometryRepository, IUserRepository userRepository, IJwtService jwtService, IHubContext<CalculationProgressHub> hubContext)
        {
            _trigonometryRepository = trigonometryRepository;
            _userRepository = userRepository;
            _jwtService = jwtService;
            _hubContext = hubContext;
        }

    public async Task<TrigonometryCalculationResultDto> CalculateTrigonometryAsync(TrigonometryRequestDto request, int userId)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;

            if (!request.XForSin.HasValue && !request.XForCos.HasValue)
            {
                throw new Exception("Not valid data");
            }

            if (request.XForSin.HasValue && request.XForCos.HasValue)
            {
                totalSum = request.N * 2;
                bothToCalculate = true;
            }
            else
            {
                totalSum = request.N;
            }

            double sinResult = -1;
            double cosResult = -1;

            if (request.XForSin.HasValue)
            {
                sinResult = await CalculateSin(request.XForSin.Value, totalSum, request.N, request.ConnectionId);
            }

            if (request.XForCos.HasValue)
            {
                cosResult = await CalculateCos(request.XForCos.Value, totalSum, request.N, request.ConnectionId);
            }

            var calculationResult = new TrigonometryCalculationResultDto
            {
                SinResult = sinResult,
                CosResult = cosResult
            };

            var calculation = new TrigonometryCalculation
            {
                Accuracy = request.N,
                UserId = userId,
                XForSin = request.XForSin ?? null,
                XForCos = request.XForCos ?? null,
                SinResult = (sinResult != -1) ? sinResult : null,
                CosResult = (cosResult != -1) ? cosResult : null
            };
            _trigonometryRepository.AddResult(calculation);

            return calculationResult;
        }

        public void CancelTrigonometryCalculation()
        {
            _cancellationTokenSource?.Cancel();
        }

        public async Task<IEnumerable<TrigonometryCalculation>> GetCalculationsByUserIdAsync(int userId)
        {
            return await _trigonometryRepository.GetCalculationsByUserIdAsync(userId);
        }

        public async Task<int> GetUserOrUserIdHereAsync(HttpRequest request)
        {
            try
            {
                var jwt = request.Cookies["jwt"];

                var token = _jwtService.Verify(jwt);

                int userId = int.Parse(token.Issuer);

                var user = await _userRepository.Get(u => u.Id == userId);
                return user.Id;
            }
            catch (Exception)
            {
                throw new Exception("Unauthorized");
            }
        }

        private async Task<double> CalculateSin(double x, int n, int requestN, string connectionId)
        {
            double result = 0.0;
            
            for (int i = 0; i < requestN; ++i)
            {
                _cancellationToken.ThrowIfCancellationRequested();
                double term = Math.Pow(-1, i) * Math.Pow(x, 2 * i + 1) / Factorial(2 * i + 1);
                result += term;
                double progress = (i / (double)n) * 100;
                int roundedProgress = (int)Math.Round(progress);
                await UpdateProgress(roundedProgress, connectionId);
            }

            return result;
        }

        private async Task<double> CalculateCos(double x, int n, int requestN, string connectionId)
        {
            double result = 0.0;
            int p = bothToCalculate ? totalSum / 2 : -1;
            for (int i = 0; i < requestN; ++i)
            {
                _cancellationToken.ThrowIfCancellationRequested();
                double term = Math.Pow(-1, i) * Math.Pow(x, 2 * i) / Factorial(2 * i);
                result += term;
                double progress = requestN;
                if (p != -1)
                {
                    ++p;
                    progress = (p / (double)n) * 100;
                }
                else
                {
                    progress = (i / (double)n) * 100;
                }
                int roundedProgress = (int)Math.Round(progress);
                await UpdateProgress(roundedProgress, connectionId);
            }

            return result;
        }

        private static double Factorial(int n)
        {
            if (n == 0)
                return 1.0;

            double result = 1.0;
            for (int i = 1; i <= n; ++i)
            {
                result *= i;
            }

            return result;
        }

        private async Task UpdateProgress(int progress, string connectionId)
        {
            // var userId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            // _hubContext.Clients.All.SendAsync("UpdateProgress", progress);
            await _hubContext.Clients.Client(connectionId).SendAsync("UpdateProgress", progress);

            // await _hubContext.Clients.Group(connectionId).SendAsync("UpdateProgress", progress);

        }
    }
}
