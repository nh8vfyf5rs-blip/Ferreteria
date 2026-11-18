using System.ComponentModel.DataAnnotations;

namespace Ferreteria.DTOS
{
    public class CreateClienteDto
    {
        [Required(ErrorMessage = "El nombre del cliente es obligatorio.")]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "El apellido del cliente es obligatorio.")]
        public string Apellido { get; set; } = string.Empty;

        [Required(ErrorMessage = "La cédula del cliente es obligatoria.")]
        public string Cedula { get; set; } = string.Empty;

        public string Telefono { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
    }
}
//