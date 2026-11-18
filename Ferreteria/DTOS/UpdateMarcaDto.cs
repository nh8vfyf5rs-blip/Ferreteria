using System.ComponentModel.DataAnnotations;

namespace Ferreteria.DTOS
{
    public class UpdateMarcaDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
    }
}
