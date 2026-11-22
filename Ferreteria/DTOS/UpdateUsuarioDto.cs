using System.ComponentModel.DataAnnotations;

namespace Ferreteria.DTOS
{
    public class UpdateUsuarioDto
    {
        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        public string Email { get; set; } = string.Empty;

        public int RolId { get; set; }
    }
}