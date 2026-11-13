using Ferreteria.Models;

namespace Ferreteria.DTOS
{
    public class UpdateDetallePagoDto
    {
        public decimal Monto { get; set; }
        public DateTime FechaPago { get; set; } 
        public int MetodoPagoId { get; set; }
    }
}
