using System.ComponentModel.DataAnnotations;

namespace Ferreteria.DTOS
{
    public class UpdateMarcaDto
    {
        [Required(ErrorMessage = "El nombre de la marca es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        public string Name { get; set; } = string.Empty;
    }
}