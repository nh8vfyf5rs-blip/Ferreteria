using Ferreteria.Models;
using System.ComponentModel.DataAnnotations;

namespace Ferreteria.DTOS
{
    public class DetallePagoDto
    {
        [Required]
        [Range(typeof(decimal),"0.01","999999999")]
        public decimal Monto { get; set; }

        [Required]
        public int MetodoPagoId { get; set; }
    }
}
