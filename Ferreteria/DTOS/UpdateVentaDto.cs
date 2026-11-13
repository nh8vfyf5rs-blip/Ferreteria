namespace Ferreteria.DTOS
{
    public class UpdateVentaDto
    {
        public DateTime FechaVenta { get; set; }
        public decimal Total { get; set; }
        public int ClienteId { get; set; }
        public int EmpleadoId { get; set; }
    }
}
