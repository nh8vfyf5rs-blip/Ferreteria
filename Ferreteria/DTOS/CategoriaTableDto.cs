namespace Ferreteria.DTOS
{
    public class CategoriaTableDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int TotalProductos { get; set; }
    }
}