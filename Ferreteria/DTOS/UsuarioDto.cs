using Ferreteria.Models.Segurudad;

namespace Ferreteria.DTOS
{
    public class UsuarioDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool EmailConfirmed { get; set; }   
        public int RolId { get; set; }
    }
}
