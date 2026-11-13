using System.ComponentModel.DataAnnotations;

namespace Ferreteria.DTOS
{
    public class CreateRolDto
    {
        [Required(ErrorMessage = "El nombre del rol es requerido")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
    }
}
