namespace Ferreteria.Models
{
    public class Sucursal
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;

        public virtual ICollection<Almacen> Almacenes { get; set; } = [];
        public virtual ICollection<Empleado> Empleados { get; set; } = [];
    }
}
