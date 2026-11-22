using System.ComponentModel.DataAnnotations;

namespace Ferreteria.DTOS
{
    public class CreateVentaDto
    {
        [Required]
        public int ClienteId { get; set; }

        [Required]
        public int EmpleadoId { get; set; }

        [Required]
        public int AlmacenId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un método de pago.")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un método de pago válido.")]
        public int MetodoPagoId { get; set; }

        public DateTime FechaVenta { get; set; } = DateTime.Now;

        [Required]
        [MinLength(1, ErrorMessage = "Debe agregar al menos un producto.")]
        public List<DetalleVentaDto> DetalleVentaDtos { get; set; } = new();
    }
}