namespace Ferreteria.DTOS
{
    public class VentaTableDto
    {
        public int Id { get; set; }

        public DateTime FechaVenta { get; set; }

        public decimal Total { get; set; }

        public int ClienteId { get; set; }

        public string Cliente { get; set; } = string.Empty;

        public int CantidadProductos { get; set; }
    }
}
