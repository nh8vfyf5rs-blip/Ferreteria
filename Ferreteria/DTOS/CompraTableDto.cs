using Ferreteria.Models;

namespace Ferreteria.DTOS
{
    public class CompraTableDto
    {
        public int Id { get; set; }
        public DateTime FechaCompra { get; set; }
        public decimal Total { get; set; }
        public int ProveedorId { get; set; }
        public string ProveedorNombre { get; set; } = string.Empty;
        public int EmpleadoId { get; set; }
        public string EmpleadoNombre { get; set; } = string.Empty;
        public string AlmacenNombre { get; set; } = string.Empty;
        public string SucursalNombre { get; set; } = string.Empty;
    }
}
