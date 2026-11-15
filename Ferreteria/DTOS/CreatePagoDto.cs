using System.ComponentModel.DataAnnotations;

namespace Ferreteria.DTOS
{
    public class CreatePagoDto
    {
        [Required]
        public int VentaId { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "Debe registrar almenos un pago.")]
        public List<DetallePagoDto> detallePagoDtos { get; set; } = new();
    }
}
