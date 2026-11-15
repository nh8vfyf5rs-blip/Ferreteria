namespace Ferreteria.DTOS
{
    public class ProductoTableDto
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public decimal PrecioCompra { get; set; }
        public decimal? PrecioVenta { get; set; }
        public string CategoriaNombre { get; set; }=string.Empty;
        public string MarcaNombre { get; set; }  = string.Empty;
    }
}
