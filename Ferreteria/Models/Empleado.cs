namespace Ferreteria.Models
{
    public class Empleado
    {
        public int Id { get; set; }
        public string   Nombres { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Cedula { get; set; }=string.Empty;
        public DateTime FechaContratacion {  get; set;  }=DateTime.Now;
        public int CargoId { get; set; }
        public virtual Cargo Cargo { get; set; } = null!;
        public int SucursalId { get; set; }
        public virtual Sucursal Sucursal { get; set; } = null!;
    }
}
