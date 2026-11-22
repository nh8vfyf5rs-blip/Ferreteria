using System.ComponentModel.DataAnnotations;

namespace Ferreteria.DTOS
{
    public class DetalleVentaDto
    {
        [Required]
        public int ProductoId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Cantidad { get; set; }

        [Required]
        [Range(0.01, 999999999d)]
        public decimal PrecioUnitario { get; set; }

        public decimal Subtotal { get; set; }
    }
}