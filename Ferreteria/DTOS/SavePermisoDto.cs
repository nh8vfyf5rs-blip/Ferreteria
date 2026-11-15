using System.ComponentModel.DataAnnotations;

namespace Ferreteria.DTOS
{
    public class SavePermisoDto
    {
        public int? Id { get; set; }

        [Required]
        [MaxLength(100)]
        [RegularExpression(@"^[a-z][a-z0-9._]*$", ErrorMessage = "Use formato: recurso.accion (ej: estudiante.crear)")]
        public string Name { get; set; }

        [MaxLength(250)]
        public string? Descripcion { get; set; }
    }
}
