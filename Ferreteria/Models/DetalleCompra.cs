namespace Ferreteria.Models
{
    public class DetalleCompra
    {
        public int Id { get; set; }
        public int CompraId { get; set; }
        public virtual Compra Compra { get; set; } = null!;
        public int ProductoId { get; set; }
        public virtual Producto Producto { get; set; } = null!;
        public int CantidadRecibida { get; set; }
        public decimal CostoUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }
}
