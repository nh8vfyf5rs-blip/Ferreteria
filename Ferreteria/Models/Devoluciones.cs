namespace Ferreteria.Models
{
    public class Devoluciones
    {
        public int Id { get; set; }
        public DateTime FechaDevolucion { get; set; }= DateTime.Now;
        public string Motivo { get; set; }=string.Empty;
        public decimal Total {  get; set; }
        public int VentaId { get; set; }
        public virtual Venta Venta { get; set; } = null!;
    }
}
