namespace Ferreteria.Models
{
    public class MovimientoInventario
    {
        public int Id { get; set; }
        public string TipoMovimiento { get; set; }=string.Empty;
        public int Cantidad { get; set; }
        public DateTime Fecha { get; set; }=DateTime.Now;
        public string? Observacion { get; set; }
        public int InventarioId { get; set; }
        public virtual Inventario Inventario { get; set; } = null!;
    }
}
