namespace Ferreteria.DTOS
{
    public class DevolucionDto
    {
        public int Id { get; set; }
        public DateTime FechaDevolucion { get; set; }
        public string Motivo { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public int VentaId { get; set; }
    }
}
