using System.ComponentModel.DataAnnotations;

namespace Ferreteria.DTOS
{
    public class UpdateDevolucionDto
    {
        [Required]
        public int VentaId { get; set; }

        [Required]
        public string Motivo { get; set; } = string.Empty;
    }
}
