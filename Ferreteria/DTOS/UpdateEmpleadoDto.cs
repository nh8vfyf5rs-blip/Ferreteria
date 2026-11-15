namespace Ferreteria.DTOS
{
    public class UpdateEmpleadoDto
    {
        public string Nombres { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Cedula { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public DateTime FechaContratacion { get; set; } = DateTime.Now;
        public decimal Salario { get; set; }
    }
}
