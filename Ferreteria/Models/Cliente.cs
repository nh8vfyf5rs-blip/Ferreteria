namespace Ferreteria.Models
{
    public class Cliente
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Direccion {  get; set; }=string.Empty;
        public DateTime FechaRegistro { get; set; } 
        public virtual ICollection<Venta> Ventas { get; set; } = []; 
    }
}
