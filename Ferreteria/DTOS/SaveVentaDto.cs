using System.ComponentModel.DataAnnotations;

namespace Ferreteria.DTOS
{
    public class SaveVentaDto
    {
        public int? Id { get; set; }

        [Required]
        public int ClienteId { get; set; }

        [Required]
        public int EmpleadoId { get; set; }

        [Required]
        public int AlmacenId { get; set; }

        public DateTime FechaVenta { get; set; } = DateTime.Now;

        public decimal Total { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "Debe agregar al menos un producto.")]
        public List<DetalleVentaDto> DetalleVentaDtos { get; set; } = new();
    }
}