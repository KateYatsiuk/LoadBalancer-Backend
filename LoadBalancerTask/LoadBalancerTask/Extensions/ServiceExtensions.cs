using FluentValidation;
using LoadBalancer.BLL.Interfaces;
using LoadBalancer.BLL.Services;
using LoadBalancer.DAL.DTOs.AuthDtos;
using LoadBalancer.DAL.Entities;
using LoadBalancer.DAL.Repositories.Interfaces;
using LoadBalancer.DAL.Repositories;
using LoadBalancer.DAL.Validation;
using LoadBalancer.DAL.DTOs.CalculationDtos;

namespace LoadBalancerAPI.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddCustomServices(this IServiceCollection services)
        {
            services.AddScoped<IRepository<User>, Repository<User>>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITrigonometryRepository, TrigonometryRepository>();

            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IUserService, UserService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IValidator<RegisterDto>, RegisterDtoValidator>();
            services.AddTransient<IValidator<LoginDto>, LoginDtoValidator>();
            services.AddTransient<IValidator<TrigonometryRequestDto>, TrigonometryRequestDtoValidator>();

            services.AddScoped<ITrigonometryService, TrigonometryService>();

            return services;
        }
    }
}
