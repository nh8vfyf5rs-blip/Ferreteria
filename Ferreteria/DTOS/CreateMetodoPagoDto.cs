using System.ComponentModel.DataAnnotations;

namespace Ferreteria.DTOS
{
    public class CreateMetodoPagoDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
    }
}
