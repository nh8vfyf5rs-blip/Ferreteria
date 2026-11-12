namespace Ferreteria.Models
{
    public class Inventario
    {
        public int Id { get; set; }
        public int Stock { get; set; }
        public DateTime FechaActualizacion {  get; set; }= DateTime.Now;
        public int AlmacenId { get; set; }
        public virtual Almacen Almacen { get; set; } = null!;
        public int ProductoId { get; set; }
        public virtual Producto Producto { get; set; }=null!;
        public virtual ICollection<DetalleInventario> DetalleInventarios { get; set; } = [];

    }
}
