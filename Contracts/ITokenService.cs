using EMS.BACKEND.API.Models;

namespace EMS.BACKEND.API.Contracts
{
    public interface ITokenService
    {
        string CreateToken(ApplicationUser user, string role);
    }
}