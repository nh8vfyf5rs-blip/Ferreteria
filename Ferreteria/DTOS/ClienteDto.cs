namespace Ferreteria.DTOS
{
    public class ClienteDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Apellido { get; set; }= string.Empty;
        public string Cedula { get; set; } = string.Empty;

        public string Telefono { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
    }
}
