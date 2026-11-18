using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Ferreteria.Extensions;
using Ferreteria.Helpers;
using Ferreteria.Models;
using Ferreteria.Models.Setting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Ferreteria.Services
{
    public class TokenService(IOptions<JwtSettings> jwtSettings) : ITokenService
    {
        private readonly JwtSettings _jwt = jwtSettings.Value;

        public string GenerateAccessToken(Usuario usuario)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new(ClaimTypes.Name, usuario.UserName),
                new(ClaimTypes.Email, usuario.Email),
                new(ClaimTypes.Role, usuario.Rol?.Name ?? "Usuario")
            };

            foreach (var permission in EffectivePermission.FromUser(usuario))
            {
                claims.Add(new Claim(
                    ClaimsPrincipalExtensions.PermissionClaimType,
                    permission));
            }

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwt.Secret));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            return Convert.ToBase64String(
                RandomNumberGenerator.GetBytes(64));
        }

        public string GeneratePasswordResetToken()
        {
            return Convert.ToBase64String(
                RandomNumberGenerator.GetBytes(64));
        }
    }
}
