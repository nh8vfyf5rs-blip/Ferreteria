namespace Ferreteria.DTOS
{
    public class DevolucionTableDto
    {
        public int Id { get; set; }
        public DateTime FechaDevolucion { get; set; }
        public string Motivo { get; set; } = string.Empty;
        public decimal Total { get; set; }

        public string ClienteNombre { get; set; } = string.Empty;
        public string EmpleadoNombre { get; set; } = string.Empty;
        public string VentaId { get; set; } = string.Empty;
    }
}

