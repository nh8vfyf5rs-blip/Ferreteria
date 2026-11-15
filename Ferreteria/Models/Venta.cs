namespace Ferreteria.Models
{
    public class Venta
    {
        public int Id { get; set; }
        public DateTime FechaVenta { get; set; }= DateTime.Now;
        public decimal Total { get; set; }
        public int ClienteId { get; set; }
        public virtual Cliente Cliente { get; set; } = null!;
        public int EmpleadoId { get; set; }
        public virtual Empleado Empleado { get; set; }= null!;
        public int AlmacenId { get; set; }
        public virtual Almacen Almacen { get; set; }= null!;
        public virtual ICollection<DetalleVenta> DetalleVentas { get; set; } = [];
        public virtual ICollection<DetallePago> DetallePagos { get; set; } = [];
    }
}
