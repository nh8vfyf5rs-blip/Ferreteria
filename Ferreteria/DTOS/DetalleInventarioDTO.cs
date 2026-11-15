using Ferreteria.Models;

namespace Ferreteria.DTOS
{
    public class DetalleInventarioDTO
    {
        public int Id { get; set; }
        public int SucursalId { get; set; }
        public int StockAnterior { get; set; }
        public int StockNuevo { get; set; }
        public DateTime FechaRegistro { get; set; } 
        public string Observacion { get; set; } = string.Empty;
        public int InventarioId { get; set; }
    }
}
