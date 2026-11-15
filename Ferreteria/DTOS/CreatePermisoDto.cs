using System.ComponentModel.DataAnnotations;

namespace Ferreteria.DTOS
{
    public class CreatePermisoDto
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [MaxLength(150)]
        [RegularExpression(@"^[a-z][az0-9._]*$", ErrorMessage = "Use el formato recurso accion: ")]
        public string Name { get; set; } = string.Empty;
        [MaxLength(200)]
        public string? Description { get; set; }
    }
}
