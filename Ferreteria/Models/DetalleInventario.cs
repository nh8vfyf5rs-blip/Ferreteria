namespace Ferreteria.Models
{
    public class DetalleInventario
    {
        public int Id { get; set; }
        public int SucursalId { get; set; }
        public int StockAnterior { get; set; }
        public int StockNuevo { get; set; }
        public DateTime FechaRegistro { get; set; }= DateTime.Now;
        public string Observacion { get; set; }=string.Empty;
        public int InventarioId { get; set; }
        public virtual Inventario Inventario { get; set; }=null!;   
    }
}