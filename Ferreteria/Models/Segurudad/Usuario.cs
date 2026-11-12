using Ferreteria.Models.Segurudad;

namespace Ferreteria.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string UserName { get; set; }=string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool EmailConfirmed { get; set; }
        public string? EmailConfirmToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public string? PasswordResedToken { get; set; }
        public DateTime? PasswordResetTokenExpirityTime { get; set; }
        public int RolId { get; set; }
        public virtual Rol Rol { get; set; } = null!;
        public virtual ICollection<UsuarioPermiso> UsuarioPermisos { get; set; } = [];
   }
}
