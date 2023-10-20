using LoadBalancer.DAL.Entities;
using System.IdentityModel.Tokens.Jwt;

namespace LoadBalancer.BLL.Interfaces
{
    public interface ITokenService
    {
        public JwtSecurityToken GenerateJWTToken(User user);
        public JwtSecurityToken RefreshToken(string token);
    }
}
