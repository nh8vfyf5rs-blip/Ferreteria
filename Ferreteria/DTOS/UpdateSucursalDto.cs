using System.ComponentModel.DataAnnotations;

namespace Ferreteria.DTOS
{
    public class UpdateSucursalDto
    {
        [Required(ErrorMessage = "El nombre de la sucursal es obligatorio.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "La dirección es obligatoria.")]
        public string Direccion { get; set; } = string.Empty;

        public string Telefono { get; set; } = string.Empty;
    }
}