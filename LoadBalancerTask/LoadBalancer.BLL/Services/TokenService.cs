using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LoadBalancer.BLL.Interfaces;
using LoadBalancer.DAL.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Streetcode.BLL.Services
{
    public sealed class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _symmetricSecurityKey;
        private readonly SigningCredentials _signInCridentials;
        private readonly string _jwtIssuer;
        private readonly string _jwtAudience;
        private readonly string _jwtKey;

        public TokenService(IConfiguration configuration)
        {
            _jwtIssuer = configuration["Jwt:Issuer"];
            _jwtAudience = configuration["Jwt:Audience"];
            _jwtKey = configuration["Jwt:Key"];
            _symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
            _signInCridentials = new SigningCredentials(_symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
        }

        public JwtSecurityToken GenerateJWTToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
            };

            return new JwtSecurityToken(
                _jwtIssuer,
                _jwtAudience,
                claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: _signInCridentials);
        }

        public JwtSecurityToken RefreshToken(string token)
        {
            var principles = GetPrinciplesFromToken(token);
            return new JwtSecurityToken(
                _jwtIssuer,
                _jwtAudience,
                principles.Claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: _signInCridentials);
        }

        private ClaimsPrincipal GetPrinciplesFromToken(string token)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidAudience = _jwtAudience,
                ValidIssuer = _jwtIssuer,
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = securityKey,
                LifetimeValidator = (DateTime? notBefore, DateTime? expires, SecurityToken securityToken, TokenValidationParameters validationParameters) =>
                {
                    if (expires == null)
                    {
                        return false;
                    }

                    return (DateTime.Now.AddHours(1) - expires).Value.TotalSeconds > 0;
                },
                ValidateLifetime = true
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Error occurred");
            }

            return principal;
        }
    }
}
