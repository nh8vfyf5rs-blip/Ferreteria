using Ferreteria.Models;

namespace Ferreteria.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(Usuario usuario);

        string GenerateRefreshToken();

        string GeneratePasswordResetToken();
    }
}
