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

        public DateTime FechaVenta { get; set; }= DateTime.Now;

        [Required]
        [MinLength(1, ErrorMessage = "Debe agregar almenos un producto.")]
        public List<DetalleVentaDto> detalleVentaDtos { get; set; } = new();
    }
}
