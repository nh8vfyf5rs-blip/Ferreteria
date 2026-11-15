using Ferreteria.Models;
using System.ComponentModel.DataAnnotations;

namespace Ferreteria.DTOS
{
    public class DetalleCompraDto
    {
        [Required]
        public int ProductoId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Cantidad { get; set; }

        [Required]
        [Range (typeof(decimal),"0.01", "999999999")]
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }
}
