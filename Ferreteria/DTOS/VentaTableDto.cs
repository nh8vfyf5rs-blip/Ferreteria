namespace Ferreteria.DTOS
{
    public class VentaTableDto
    {
        public int Id { get; set; }
        public DateTime FechaVenta { get; set; }
        public decimal Total { get; set; }
        public int ClienteId { get; set; }
        public string ClienteNombre { get; set; } = string.Empty;
        public string EmpleadoNombre { get; set; } = string.Empty;
        public string AlmacenNombre { get; set; } = string.Empty;
        public string SucursalNombre { get; set; } = string.Empty;
        public int CantidadProductos { get; set; }
    }
}