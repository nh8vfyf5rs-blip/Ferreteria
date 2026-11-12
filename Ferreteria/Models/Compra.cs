namespace Ferreteria.Models
{
    public class Compra
    {
        public int Id { get; set; }
        public DateTime FechaCompra {  get; set; }=DateTime.Now;
        public decimal Total {  get; set; }

        public int ProveedorId { get; set; }
        public virtual Proveedor Proveedor { get; set; } = null!;
        public int EmpleadoId { get; set; }
        public virtual Empleado Empleado { get; set; } = null!;
        public virtual ICollection<DetalleCompra> DetalleCompras { get; set; } = [];

    }
}
