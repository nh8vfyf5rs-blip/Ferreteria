using System.Net.Mail;
using System.Security.Cryptography;
using Ferreteria.DTOS;
using Ferreteria.Models.Segurudad;
using Ferreteria.Repositorios;
using Microsoft.Extensions.Configuration;

namespace Ferreteria.Services
{
    public class AuthService(
        IUsuarioRepository userRepo,
        IRolRepository roleRepo,
        ITokenService tokenService,
        IEmailService emailService,
        IConfiguration config) : IAuthService
    {
        private string BaseUrl => (config["App:BaseUrl"] ?? "https://localhost:7255").TrimEnd('/');

        public async Task<bool> ChangePasswordAsync(
            string userName, ChangePasswordDto dto)
        {
            var user = await userRepo.GetUsuarioByUserName(userName);

            if (user == null ||
                !userRepo.ValidatePassword(user, dto.CurrentPassword))
                return false;

            user.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(
                dto.NewPassword);

            await userRepo.SaveAsync();

            return true;
        }

        public async Task<bool> ConfirmEmailAsync(string email, string token)
        {
            var user = await userRepo.GetUsuarioByEmail(email);

            if (user == null)
                return false;

            var decodedToken = Uri.UnescapeDataString(token.Trim());

            if (!string.Equals(
                user.EmailConfirmToken,
                decodedToken,
                StringComparison.Ordinal))
                return false;

            user.EmailConfirmed = true;
            user.EmailConfirmToken = null;

            await userRepo.SaveAsync();

            return true;
        }

        public async Task<(bool Success, LoginResponseDto? result, string? ErrorMessage)>
            LoginAsync(LoginRequestDto dto)
        {
            var user = await userRepo.GetUsuarioByEmail(dto.Email);

            if (user == null ||
                !userRepo.ValidatePassword(user, dto.Password))
                return (false, null, "Credenciales inválidas.");

            if (!user.EmailConfirmed)
                return (false, null, "Debe confirmar su correo electrónico.");

            var accessToken = tokenService.GenerateAccessToken(user);
            var refreshToken = tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await userRepo.SaveAsync();

            var result = new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                RefreshTokenExpiresAt = user.RefreshTokenExpiryTime
            };

            return (true, result, null);
        }

        public async Task<LoginResponseDto?> RefreshAccessTokenAsync(
            string refreshToken)
        {
            var user = await userRepo.GetUsuarioByRefreshTokenAsync(
                refreshToken);

            if (user == null ||
                !user.RefreshTokenExpiryTime.HasValue ||
                user.RefreshTokenExpiryTime.Value <= DateTime.UtcNow)
                return null;

            var newAccessToken = tokenService.GenerateAccessToken(user);
            var newRefreshToken = tokenService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await userRepo.SaveAsync();

            return new LoginResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                RefreshTokenExpiresAt = user.RefreshTokenExpiryTime
            };
        }

        public async Task<(bool Success, string? ErrorMessage)> RegisterAsync(
            RegisterRequestDto dto)
        {
            var existingUser = await userRepo.GetUsuarioByEmail(dto.Email);

            if (existingUser != null)
                return (false, "El correo ya está en uso. Intente con otro correo.");

            var roles = await roleRepo.GetAllAsync();

            var usuarioRole = roles.FirstOrDefault(
                r => r.Name == "Usuario");

            if (usuarioRole == null)
                return (false, "El rol Usuario no está configurado.");

            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            var rawToken = Convert.ToBase64String(tokenBytes);
            var encodedToken = Uri.EscapeDataString(rawToken);

            var user = new Usuario
            {
                UserName = dto.Username,
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.EnhancedHashPassword(
                    dto.Password),
                RolId = usuarioRole.Id,
                EmailConfirmed = false,
                EmailConfirmToken = rawToken
            };

            await userRepo.CreateAsync(user);

            string confirmationLink =
                $"{BaseUrl}/Auth/ConfirmEmail?email={Uri.EscapeDataString(user.Email)}&token={encodedToken}";

            string htmlBody =
                "<!DOCTYPE html><html><body>" +
                "<p>Hola,</p>" +
                "<p>Haz clic en el siguiente botón para confirmar tu cuenta:</p>" +
                "<p><a href=\"" + confirmationLink +
                "\" style='background:#007BBFF;color:white;padding:10px 20px;border-radius:5px;text-decoration:none;'>" +
                "Confirmar Cuenta</a></p>" +
                "</body></html>";

            try
            {
                emailService.SendPasswordResetEmail(user.Email, htmlBody);
            }
            catch (SmtpException)
            {
                userRepo.Delete(user);
                await userRepo.SaveAsync();

                return (false,
                    "No se pudo enviar el correo de confirmación. Verifique la configuración SMTP.");
            }

            return (true, null);
        }

        public async Task<bool> ResendConfirmationEmailAsync(string email)
        {
            var user = await userRepo.GetUsuarioByEmail(email);

            if (user == null || user.EmailConfirmed)
                return false;

            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            var rawToken = Convert.ToBase64String(tokenBytes);
            var encodedToken = Uri.EscapeDataString(rawToken);

            user.EmailConfirmToken = rawToken;

            await userRepo.SaveAsync();

            string confirmationLink =
                $"{BaseUrl}/Auth/ConfirmEmail?email={Uri.EscapeDataString(user.Email)}&token={encodedToken}";

            string htmlBody =
                "<!DOCTYPE html><html><body>" +
                "<p>Hola,</p>" +
                "<p>Haz clic en el siguiente botón para confirmar tu cuenta:</p>" +
                "<p><a href=\"" + confirmationLink +
                "\" style='background:#007BBFF;color:white;padding:10px 20px;border-radius:5px;text-decoration:none;'>" +
                "Confirmar Cuenta</a></p>" +
                "</body></html>";

            emailService.SendPasswordResetEmail(user.Email, htmlBody);

            return true;
        }

        public async Task<bool> ResetPasswordAsync(
            ResetPasswordRequestDto dto)
        {
            var user = await userRepo.GetUsuarioByEmail(dto.Email);

            if (user == null)
                return false;

            var providedToken = Uri.UnescapeDataString(
                dto.Token ?? string.Empty);

            var storedToken = user.PasswordResedToken ?? string.Empty;

            if (!storedToken.Equals(
                providedToken,
                StringComparison.Ordinal))
                return false;

            if (!user.PasswordResetTokenExpirityTime.HasValue ||
                user.PasswordResetTokenExpirityTime.Value < DateTime.UtcNow)
                return false;

            user.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(
                dto.NewPassword);

            user.PasswordResedToken = null;
            user.PasswordResetTokenExpirityTime = null;

            await userRepo.SaveAsync();

            return true;
        }

        public async Task<bool> SendResetPasswordLinkAsync(string email)
        {
            var user = await userRepo.GetUsuarioByEmail(email);

            if (user == null)
                return false;

            var token = tokenService.GeneratePasswordResetToken();
            var encodedToken = Uri.EscapeDataString(token);

            user.PasswordResedToken = token;
            user.PasswordResetTokenExpirityTime =
                DateTime.UtcNow.AddHours(1);

            await userRepo.SaveAsync();

            string resetLink =
                $"{BaseUrl}/Auth/ResetPassword?email={Uri.EscapeDataString(user.Email)}&token={encodedToken}";

            string htmlBody =
                "<!DOCTYPE html><html><body>" +
                "<p>Hola,</p>" +
                "<p>Haz clic en el siguiente botón para restablecer tu contraseña:</p>" +
                "<p><a href=\"" + resetLink +
                "\" style='background:#007BBFF;color:white;padding:10px 20px;border-radius:5px;text-decoration:none;'>" +
                "Restablecer contraseña</a></p>" +
                "</body></html>";

            emailService.SendPasswordResetEmail(user.Email, htmlBody);

            return true;
        }
    }
}