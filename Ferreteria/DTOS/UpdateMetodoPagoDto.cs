using System.ComponentModel.DataAnnotations;

namespace Ferreteria.DTOS
{
    public class UpdateMetodoPagoDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
    }
}
