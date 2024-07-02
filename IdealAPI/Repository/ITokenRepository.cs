using Microsoft.AspNetCore.Identity;

namespace IdealAPI.Repository
{
    public interface ITokenRepository
    {
        string CreateJWTToken(IdentityUser user, string[] roles);
    }
}
