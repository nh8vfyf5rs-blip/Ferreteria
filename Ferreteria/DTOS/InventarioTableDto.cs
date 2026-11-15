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
        public string ProductoName { get; set; }= string.Empty;
    }
}
