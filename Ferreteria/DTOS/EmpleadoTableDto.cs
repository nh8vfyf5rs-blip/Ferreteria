using Ferreteria.Models;

namespace Ferreteria.DTOS
{
    public class EmpleadoTableDto
    {
    
            public int Id { get; set; }
            public string Nombres { get; set; } = string.Empty;
            public string Apellidos { get; set; } = string.Empty;
            public string Cedula { get; set; } = string.Empty;
            public string Telefono { get; set; } = string.Empty;
            public string Correo { get; set; } = string.Empty;
            public string Direccion { get; set; } = string.Empty;
            public DateTime FechaContratacion { get; set; } 
            public decimal Salario { get; set; }
            public string CargoNombre { get; set; }=string.Empty;       
            public string SucursalNombre { get; set; }= string.Empty;

}
}
