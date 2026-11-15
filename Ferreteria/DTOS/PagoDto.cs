namespace Ferreteria.DTOS
{
    public class PagoDto
    {
        public int Id { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaPago { get; set; }
        public int MetodoPagoId { get; set; }
        public string MetodoPago {  get; set; }=string.Empty;
        public int VentaId { get; set; }
    }
}
