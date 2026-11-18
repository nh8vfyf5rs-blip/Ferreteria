using System.ComponentModel.DataAnnotations;

namespace Ferreteria.DTOS
{
    public class CreateMovimientoInventarioDto
    {
        [Required]
        public string TipoMovimiento { get; set; } = string.Empty;
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor que cero.")]
        public int Cantidad { get; set; }
        public string? Observacion { get; set; }
        [Required]
        public int InventarioId { get; set; }
    }
}
