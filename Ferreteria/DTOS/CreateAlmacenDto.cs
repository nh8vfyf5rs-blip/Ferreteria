using System.ComponentModel.DataAnnotations;

namespace Ferreteria.DTOS
{
    public class CreateAlmacenDto
    {
        [Required(ErrorMessage = "El nombre del almacén es obligatorio.")]
        public string Name { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una sucursal válida.")]
        public int SucursalId { get; set; }
    }
}
