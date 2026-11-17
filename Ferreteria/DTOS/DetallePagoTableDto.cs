using Ferreteria.Models;

namespace Ferreteria.DTOS
{
    public class DetallePagoTableDto
    {
        public int Id { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaPago { get; set; }
        public int VentaId { get; set; }
        public string MetodoPagoNombre { get; set; }=string.Empty;
         
    }
}
