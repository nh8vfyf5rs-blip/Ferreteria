namespace Ferreteria.DTOS
{
    public class ProductoDto
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; } = string.Empty;

        // Representa el precio de venta.
        public decimal Precio { get; set; }
        public decimal PrecioCompra { get; set; }
        public decimal? PrecioVenta { get; set; }

        // Existencias correspondientes al almacén seleccionado.
        public int Stock { get; set; }

        public int CategoriaId { get; set; }
        public int MarcaId { get; set; }
        public int AlmacenId { get; set; }

        // Se conserva por compatibilidad.
        // El proveedor se relaciona con el producto mediante Compra.
        public int ProveedorId { get; set; }
    }
}
