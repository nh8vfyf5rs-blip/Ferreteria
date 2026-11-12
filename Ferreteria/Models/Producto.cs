namespace Ferreteria.Models
{
    public class Producto
    {
        public int Id { get; set; }
        public string Codigo { get; set; } =string.Empty;
        public string Nombre { get; set; }=string.Empty;
        public string? Descripcion { get; set; }
        public decimal PrecioCompra {  get; set; }
        public decimal? PrecioVenta { get; set; }
        public int CategoriaId { get; set; }
        public virtual Categoria Categoria { get; set; } = null!;
        public int MarcaId { get; set; }
        public virtual Marca Marca { get; set; }=null!;

        public virtual ICollection<DetalleVenta> DetalleVentas { get; set; } = [];
        public virtual ICollection<DetalleCompra> DetalleCompras { get; } = [];
        public virtual ICollection<DetalleInventario> DetalleInventarios { get; set; } = [];
    }
}
