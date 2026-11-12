namespace Ferreteria.Models
{
    public class Proveedor
    {
        public int Id { get; set; }
        public string NombreEmpresa { get; set; } = string.Empty;
        public string Contacto { get; set; } = string.Empty;
        public string Correo {  get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; }

        public virtual ICollection<Compra> Compras { get; set; } = [];
    }
}
