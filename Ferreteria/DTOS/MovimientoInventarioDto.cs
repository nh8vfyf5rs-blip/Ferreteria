namespace Ferreteria.DTOS
{
    public class MovimientoInventarioDto
    {
        public int Id { get; set; }
        public string TipoMovimiento { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public DateTime Fecha { get; set; }
        public string? Observacion { get; set; }
        public int InventarioId { get; set; }
    }
}
