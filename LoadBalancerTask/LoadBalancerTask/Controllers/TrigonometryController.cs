using FluentValidation;
using FluentValidation.Results;
using LoadBalancer.BLL.Interfaces;
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
            /*string uniqueCookieValue = Guid.NewGuid().ToString();
            Console.WriteLine("uniqueCookieValue " + uniqueCookieValue);
            Response.Cookies.Append("reqq", uniqueCookieValue, new CookieOptions
            {
                Expires = DateTime.Now.AddHours(1),
                Path = "/",
                HttpOnly = true,
            });*/
            try
            {
                Console.WriteLine("uniqueCookieValue " + request.ConnectionId);
                int userId = await _trigonometryService.GetUserOrUserIdHereAsync(Request);
                var calculationResult = await _trigonometryService.CalculateTrigonometryAsync(request, userId);
                return Ok(calculationResult);
            }
            catch (OperationCanceledException)
            {
                return Conflict("Operation canceled");
            }
            catch(Exception ex)
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

            catch
            {
                return BadRequest();
            }
        }
    }
}
