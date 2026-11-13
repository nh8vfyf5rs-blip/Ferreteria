namespace Ferreteria.DTOS
{
    public class UpdateCompraDto
    {
        public DateTime FechaCompra { get; set; } = DateTime.Now;
        public decimal Total { get; set; }
        public int ProveedorId { get; set; }
        public int EmpleadoId { get; set; }

    }
}
