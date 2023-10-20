using FluentValidation;
using FluentValidation.Results;
using LoadBalancer.BLL.Interfaces;
using LoadBalancer.DAL.DTOs.AuthDtos;
using LoadBalancer.DAL.Entities;
using LoadBalancer.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LoadBalancerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly IValidator<RegisterDto> _registerValidator;
        private readonly IValidator<LoginDto> _loginValidator;

        public AuthController(IUserRepository userRepository, IJwtService jwtService, IValidator<RegisterDto> registerValidator, IValidator<LoginDto> loginValidator)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _registerValidator = registerValidator;
            _loginValidator = loginValidator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto userDto)
        {
            ValidationResult validationResult = await _registerValidator.ValidateAsync(userDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            User user = new User
            {
                UserName = userDto.UserName,
                Email = userDto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(userDto.Password),
            };

            await _userRepository.Create(user);
            return Created("Success", user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto userDto)
        {
            ValidationResult validationResult = await _loginValidator.ValidateAsync(userDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            User user = await _userRepository.Get(email => email.Email == userDto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(userDto.Password, user.Password))
            {
                return BadRequest(new { message = "Not valid credentials." });
            }

            var jwt = _jwtService.Generate(user.Id);

            Response.Cookies.Append("jwt", jwt, new CookieOptions
            {
                HttpOnly = true
            });

            return Ok(user);
        }

        [HttpGet("getUser")]
        public async Task<IActionResult> User()
        {
            try
            {
                var jwt = Request.Cookies["jwt"];

                var token = _jwtService.Verify(jwt);

                int userId = int.Parse(token.Issuer);

                var user = await _userRepository.Get(u => u.Id == userId);
                return Ok(user);
            }
            catch (Exception)
            {
                return Unauthorized();
            }
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt");

            return Ok(new { message = "Success" });
        }
    }
}
