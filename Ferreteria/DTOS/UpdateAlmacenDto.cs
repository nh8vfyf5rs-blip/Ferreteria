using System.ComponentModel.DataAnnotations;

namespace Ferreteria.DTOS
{
    public class UpdateAlmacenDto
    {
        [Required(ErrorMessage = "El identificador es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El identificador no es válido.")]
        public int? Id { get; set; }

        [Required(ErrorMessage = "El nombre del almacén es obligatorio.")]
        public string Name { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una sucursal válida.")]
        public int SucursalId { get; set; }
    }
}
