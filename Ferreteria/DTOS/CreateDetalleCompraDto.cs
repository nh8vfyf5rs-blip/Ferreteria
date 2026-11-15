namespace Ferreteria.DTOS
{
    public class CreateDetalleCompraDto
    {
        public int CompraId { get; set; }
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
}
