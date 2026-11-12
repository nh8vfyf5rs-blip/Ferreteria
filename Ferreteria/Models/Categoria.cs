namespace Ferreteria.Models
{
    public class Categoria
    {
        public int Id { get; set; }
        public string Name { get; set; }=string.Empty;
        public virtual ICollection<Producto> Productos { get; set; } = [];
    }
}
