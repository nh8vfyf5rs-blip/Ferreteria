using System.ComponentModel.DataAnnotations;

namespace Ferreteria.DTOS
{
    public class CreateCategoriaDto
    {
        [Required(ErrorMessage = "El nombre de la categoría es obligatorio.")]
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
    }
}
