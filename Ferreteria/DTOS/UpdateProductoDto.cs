using System.ComponentModel.DataAnnotations;

namespace Ferreteria.DTOS
{
    public class UpdateProductoDto
    {
        [Required]
        public string Codigo { get; set; } = string.Empty;

        [Required]
        public string Nombre { get; set; } = string.Empty;

        public string Descripcion { get; set; } = string.Empty;

        // Precio se conserva por compatibilidad con PrecioVenta.
        [Range(0.01, double.MaxValue,
            ErrorMessage = "El precio debe ser mayor que cero.")]
        public decimal Precio { get; set; }

        [Range(0, double.MaxValue,
            ErrorMessage = "El precio de compra no puede ser negativo.")]
        public decimal PrecioCompra { get; set; }

        [Range(0.01, double.MaxValue,
            ErrorMessage = "El precio de venta debe ser mayor que cero.")]
        public decimal? PrecioVenta { get; set; }

        [Range(0, int.MaxValue,
            ErrorMessage = "El stock no puede ser negativo.")]
        public int Stock { get; set; }

        [Required]
        public int CategoriaId { get; set; }

        [Required]
        public int MarcaId { get; set; }

        [Required]
        public int AlmacenId { get; set; }

        // Se conserva por compatibilidad.
        // No representa una relación directa con Producto.
        public int ProveedorId { get; set; }
    }
}
