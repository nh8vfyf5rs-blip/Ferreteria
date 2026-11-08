namespace Ferreteria.DTOS
{
    public class VentaDtos
    {
        public int ClienteId { get; set; }
        public int MetodoPagoId { get; set; }
        public List<ItemVentaDto> Items { get; set; } = [];
    }

    public class ItemVentaDto
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
    }
}
