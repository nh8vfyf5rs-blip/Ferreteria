using System.ComponentModel.DataAnnotations;

namespace Ferreteria.DTOS
{
    public class UpdatePagoDto
    {
        [Required]
        [Range(typeof(decimal),"0.01","99999999")]
        public decimal Monto { get; set; }

        [Required]
        public  int MetodoPagoId { get; set; }
    }
}
