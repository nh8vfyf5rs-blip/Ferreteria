using Ferreteria.Models;

namespace Ferreteria.DTOS
{
    public class AlmacenTableDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int SucursalId { get; set; }
        public string SucursalName { get; set; }= string.Empty;
    }
}
