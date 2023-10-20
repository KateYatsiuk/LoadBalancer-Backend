using LoadBalancer.DAL.DTOs.AuthDtos;
using LoadBalancer.DAL.Entities;
using Microsoft.AspNetCore.Http;

namespace LoadBalancer.BLL.Interfaces
{
    public interface IUserService
    {
        Task<User> Register(RegisterDto userDto);
        Task<User> Login(LoginDto userDto);
        Task<User> GetUser();
        Task Logout(HttpContext httpContext);
    }
}
