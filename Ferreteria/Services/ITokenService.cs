using Ferreteria.Models;
using Ferreteria.Models.Segurudad;

namespace Ferreteria.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(Usuario usuario);
        string GenerateRefreshToken();
        string GeneratePasswordResetToken();
    }
}
