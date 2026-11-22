using Ferreteria.Models;

namespace Ferreteria.DTOS
{
    public class InventarioTableDto
    {
        public int Id { get; set; }
        public int Stock { get; set; }
        public DateTime FechaActualizacion { get; set; } 
        public int AlmacenId { get; set; }
        public string AlmacenNombre { get; set; }=string.Empty;
        public int ProductoId { get; set; }
        public string ProductoNombre { get; set; }= string.Empty;
        public string SucursalNombre { get; internal set; }
        public int SucursalId { get; internal set; }
    }
}
