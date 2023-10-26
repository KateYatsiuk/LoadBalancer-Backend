using FluentValidation;
using FluentValidation.Results;
using LoadBalancer.BLL.Interfaces;
using LoadBalancer.BLL.Services;
using LoadBalancer.DAL.DTOs.CalculationDtos;
using Microsoft.AspNetCore.Mvc;

namespace LoadBalancer.LoadBalancerAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class TrigonometryController : ControllerBase
    {
        private readonly ITrigonometryService _trigonometryService;
        private readonly IValidator<TrigonometryRequestDto> _requestValidator;

        public TrigonometryController(ITrigonometryService trigonometryService, IValidator<TrigonometryRequestDto> requestValidator)
        {
            _trigonometryService = trigonometryService;
            _requestValidator = requestValidator;
        }

        [HttpPost("calculate")]
        public async Task<IActionResult> CalculateTrigonometry([FromBody] TrigonometryRequestDto request)
        {
            ValidationResult validationResult = await _requestValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            try
            {
                int userId = await _trigonometryService.GetUserOrUserIdHereAsync(Request);
                var calculationResult = await _trigonometryService.CalculateTrigonometryAsync(request, userId);
                return Ok(calculationResult);
            }
            catch (OperationCanceledException)
            {
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Unauthorized");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("cancel")]
        public IActionResult CancelTrigonometry()
        {
            _trigonometryService.CancelTrigonometryCalculation();
            Console.WriteLine("Executing cancellation...");
            return Ok("Task canceled");
        }


        [HttpGet("history")]
        public async Task<IActionResult> GetCalculationsByUserId()
        {
            try
            {
                int userId = await _trigonometryService.GetUserOrUserIdHereAsync(Request);
                var calculations = await _trigonometryService.GetCalculationsByUserIdAsync(userId);
                return Ok(calculations);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Unauthorized");
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet("polling")]
        public async Task<IActionResult> GetProgress([FromQuery] string datetime)
        {
            try
            {
                int userId = await _trigonometryService.GetUserOrUserIdHereAsync(Request);
                int progress = TrigonometryService.GetProgressFromRedis(userId, datetime);
                Console.WriteLine("currentProgress " + progress);
                return Ok(new { progress });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Unauthorized");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}
