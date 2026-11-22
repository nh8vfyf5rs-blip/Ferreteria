using System.ComponentModel.DataAnnotations;

namespace Ferreteria.DTOS
{
    public class SaveCargoDto
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "El nombre del cargo es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        public string Name { get; set; } = string.Empty;
    }
}