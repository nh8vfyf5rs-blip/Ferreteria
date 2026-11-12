namespace Ferreteria.Models
{
    public class Cargo
    {
        public int Id { get; set; }
        public string Name { get; set; }=string.Empty;

        public virtual ICollection<Empleado> Empleados { get; set; } = [];
    }
}
