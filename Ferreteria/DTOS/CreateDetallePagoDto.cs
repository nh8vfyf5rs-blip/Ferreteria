using Ferreteria.Models;

namespace Ferreteria.DTOS
{
    public class CreateDetallePagoDto
    {
        public decimal Monto { get; set; }
        public DateTime FechaPago { get; set; } 
        public int MetodoPagoId { get; set; }
        public int VentaId { get; set; }
    }
}
