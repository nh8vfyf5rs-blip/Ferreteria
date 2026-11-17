using System.ComponentModel.DataAnnotations;

namespace Ferreteria.DTOS
{
    public class CreateDevolucionDto
    {
        [Required]
        public int VentaId { get; set; }

        [Required]
        public string Motivo { get; set; } = string.Empty;
    }
}
