namespace Ferreteria.DTOS
{
    public class CompraDto
    {
   
        public int Id { get; set; }
        public DateTime FechaCompra { get; set; } = DateTime.Now;
        public decimal Total { get; set; }
        public int ProveedorId { get; set; }
        public int EmpleadoId { get; set; }
        public int AlmacenId { get; set; }
        public List<DetalleCompraDto> DetalleCompraDtos { get; set; } = new();
    }
}