using LoadBalancer.BLL.Interfaces;
using LoadBalancer.DAL.DTOs.AuthDtos;
using LoadBalancer.DAL.Entities;
using LoadBalancer.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;

namespace LoadBalancer.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(
            IUserRepository userRepository,
            IJwtService jwtService,
            IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<User> Register(RegisterDto userDto)
        {
            User user = new User
            {
                UserName = userDto.UserName,
                Email = userDto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(userDto.Password),
            };

            await _userRepository.Create(user);
            return user;
        }

        public async Task<User> Login(LoginDto userDto)
        {
            var user = await _userRepository.Get(email => email.Email == userDto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(userDto.Password, user.Password))
            {
                throw new Exception("Not valid credentials");
            }

            var jwt = _jwtService.Generate(user.Id);

            _httpContextAccessor.HttpContext.Response.Cookies.Append("jwt", jwt, new CookieOptions
            {
                HttpOnly = true
            });

            return user;
        }

        public async Task<User> GetUser()
        {
            try
            {
                var jwt = _httpContextAccessor.HttpContext.Request.Cookies["jwt"];

                var token = _jwtService.Verify(jwt);

                int userId = int.Parse(token.Issuer);

                var user = await _userRepository.Get(u => u.Id == userId);

                return user;
            }
            catch (Exception)
            {
                throw new Exception("Error occurred");
            }
        }
        public async Task Logout(HttpContext httpContext)
        {
            var jwt = httpContext.Request.Cookies["jwt"];
            if (jwt != null)
            {
                httpContext.Response.Cookies.Delete("jwt");
            }
        }
    }
}
/*using LoadBalancer.BLL.Interfaces;
using LoadBalancer.DAL.DTOs;
using LoadBalancer.DAL.Entities;
using LoadBalancer.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;

namespace LoadBalancer.BLL.Services
{
    public class LoginResultDTO
    {
        public LoginDto User { get; set; }
        public string Token { get; set; }
        public DateTime ExpireAt { get; set; }
    }
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
*//*        private readonly IJwtService _jwtService;
*//*        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITokenService _tokenService;


        public UserService(
            IUserRepository userRepository,
            ITokenService tokenService,
            IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<User> Register(RegisterDto userDto)
        {
            User user = new User
            {
                UserName = userDto.UserName,
                Email = userDto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(userDto.Password),
            };

            await _userRepository.Create(user);
            return user;
        }

        public async Task<User> Login(LoginDto userDto)
        {
            var user = await _userRepository.Get(email => email.Email == userDto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(userDto.Password, user.Password))
            {
                throw new Exception("Not valid credentials");
            }

            var token = _tokenService.GenerateJWTToken(user);
            var stringToken = new JwtSecurityTokenHandler().WriteToken(token);

            return user;
        }

        public async Task<User> GetUser()
        {
            try
            {
                var jwt = _httpContextAccessor.HttpContext.Request.Cookies["jwt"];

                var token = _jwtService.Verify(jwt);

                int userId = int.Parse(token.Issuer);

                var user = await _userRepository.Get(u => u.Id == userId);

                return user;
            }
            catch (Exception)
            {
                throw new Exception("Error occurred");
            }
        }
        public async Task Logout(HttpContext httpContext)
        {
            var jwt = httpContext.Request.Cookies["jwt"];
            if (jwt != null)
            {
                httpContext.Response.Cookies.Delete("jwt");
            }
        }
    }
}*/