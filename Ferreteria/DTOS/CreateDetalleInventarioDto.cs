using System.ComponentModel.DataAnnotations;

namespace Ferreteria.DTOS
{
    public class CreateDetalleInventarioDto
    {
        [Required(ErrorMessage = "El inventario es obligatorio.")]
        public int InventarioId { get; set; }

        [Required(ErrorMessage = "La sucursal es obligatoria.")]
        public int SucursalId { get; set; }

        [Required(ErrorMessage = "El stock anterior es obligatorio.")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock anterior no puede ser negativo.")]
        public int StockAnterior { get; set; }

        [Required(ErrorMessage = "El stock nuevo es obligatorio.")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock nuevo no puede ser negativo.")]
        public int StockNuevo { get; set; }

        [StringLength(250, ErrorMessage = "La observación no puede exceder los 250 caracteres.")]
        public string? Observacion { get; set; }
    }
}