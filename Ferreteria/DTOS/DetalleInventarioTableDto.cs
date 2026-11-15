using Ferreteria.Models;

namespace Ferreteria.DTOS
{
    public class DetalleInventarioTableDto
    {
        public int Id { get; set; }

        public string SucursalNombre { get; set; }=string.Empty;
        public string AlmacenNombre { get; set; } = string.Empty;
        public string ProductoNombre { get; set; }=string.Empty;
        public int StockAnterior { get; set; }
        public int StockNuevo { get; set; }
        public DateTime FechaRegistro { get; set; } 
        public string Observacion { get; set; } = string.Empty;
     
    }
}
