namespace Ferreteria.DTOS
{
    public class InventarioDto
    {
        public int Id { get; set; }
        public int SucursalId { get; set; }
        public int Stock { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public int AlmacenId { get; set; }
        public int ProductoId { get; set; }
        public List<DetalleInventarioDTO> DetalleInventarioDTO { get; set; } = new();
    }
}