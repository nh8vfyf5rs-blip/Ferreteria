namespace Ferreteria.Models
{
    public class Almacen
    {
        public int Id { get; set; }
        public string Name { get; set; }=string.Empty;
        public int SucursalId { get; set; }
        public virtual Sucursal Sucursal { get; set; } = null!;
        public virtual ICollection<Inventario> Inventarios { get; set; } = [];
    }
}
