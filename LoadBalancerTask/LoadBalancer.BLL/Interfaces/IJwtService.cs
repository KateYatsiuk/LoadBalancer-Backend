using System.IdentityModel.Tokens.Jwt;

namespace LoadBalancer.BLL.Interfaces
{
    public interface IJwtService
    {
        public string Generate(int id);
        public JwtSecurityToken Verify(string jwt);
    }
}
