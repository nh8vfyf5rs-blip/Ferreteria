namespace Ferreteria.Models
{
    public class DetallePago
    {
        public int Id { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaPago { get; set; }= DateTime.Now;
        public int MetodoPagoId { get; set; }
        public virtual MetodoPago MetodoPago { get; set; } = null!;
        public int VentaId { get; set; }
        public virtual Venta Venta { get; set; } = null!;
    }
}
