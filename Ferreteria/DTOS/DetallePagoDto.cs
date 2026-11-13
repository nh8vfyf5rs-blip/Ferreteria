using Ferreteria.Models;

namespace Ferreteria.DTOS
{
    public class DetallePagoDto
    {
        public int Id { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaPago { get; set; } 
        public int MetodoPagoId { get; set; }
        public int VentaId { get; set; }
    }
}
