using System.ComponentModel.DataAnnotations;

namespace Ferreteria.DTOS
{
    public class CreateMarcaDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
    }
}
