using System.ComponentModel.DataAnnotations;

namespace Ferreteria.DTOS
{
    public class CreateSucursalDto
    {
        [Required(ErrorMessage = "El nombre de la sucursal es requerido")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "La dirección es requerida")]
        public string Direccion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es requerido")]
        public string Telefono { get; set; } = string.Empty;
    }
}
