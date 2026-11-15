using System.ComponentModel.DataAnnotations;

namespace Ferreteria.DTOS
{
    public class UpdateVentaDto
    {
        [Required]
        public int ClienteId { get; set; }
        [Required]
        public int EmpleadoId { get; set; }

        [Required]
        public int AlmacenId { get; set; }
    }
}
