using System.ComponentModel.DataAnnotations;

namespace Ferreteria.DTOS
{
    public class CreateEmpleadoDto
    {
        [Required]
        public string Nombres { get; set; } = string.Empty;
        [Required]
        public string Apellidos { get; set; } = string.Empty;
        [Required]
        public string Cedula { get; set; } = string.Empty;
        [Required]
        public string Telefono { get; set; } = string.Empty;
        [Required]
        public string Correo { get; set; } = string.Empty;
        [Required]
        public string Direccion { get; set; } = string.Empty;
        public DateTime FechaContratacion { get; set; } = DateTime.Now;
        public decimal Salario { get; set; }
        [Required]
        public int CargoId { get; set; }
        [Required]
        public int SucursalId { get; set; }
    }
}
