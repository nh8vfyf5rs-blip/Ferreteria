using System.Net;
using System.Net.Mail;
using Ferreteria.Models.Setting;
using Microsoft.Extensions.Options;

namespace Ferreteria.Services
{
    public class EmailService(IOptions<SmtpSetting> smtpSettings) : IEmailService
    {
        private readonly SmtpSetting _smtp = smtpSettings.Value;

        public void SendPasswordResetEmail(string toEmail, string body)
        {
            if (string.IsNullOrWhiteSpace(_smtp.Host))
                throw new InvalidOperationException(
                    "Falta la configuración Smtp:Host.");

            if (string.IsNullOrWhiteSpace(_smtp.User))
                throw new InvalidOperationException(
                    "Falta la configuración Smtp:User.");

            if (string.IsNullOrWhiteSpace(_smtp.Password))
                throw new InvalidOperationException(
                    "Falta la configuración Smtp:Password.");

            if (string.IsNullOrWhiteSpace(toEmail))
                throw new ArgumentException(
                    "El correo destinatario es obligatorio.",
                    nameof(toEmail));

            using var client = new SmtpClient(_smtp.Host, _smtp.Port)
            {
                Credentials = new NetworkCredential(
                    _smtp.User,
                    _smtp.Password),

                EnableSsl = _smtp.EnableSsl
            };

            var mail = new MailMessage
            {
                From = new MailAddress(
                    _smtp.User,
                    "Soporte de Autenticacion"),

                Subject = "Token de confirmacion / recuperacion",
                Body = body,
                IsBodyHtml = true
            };

            mail.To.Add(toEmail);

            client.Send(mail);
        }
    }
}
