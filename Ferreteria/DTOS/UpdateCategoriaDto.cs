using System.ComponentModel.DataAnnotations;

namespace Ferreteria.DTOS
{
    public class UpdateCategoriaDto
    {
        [Required(ErrorMessage = "El identificador de la categoría es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El identificador de la categoría no es válido.")]
        public int? Id { get; set; }

        [Required(ErrorMessage = "El nombre de la categoría es obligatorio.")]
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
    }
}
