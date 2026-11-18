using System.ComponentModel.DataAnnotations;

namespace Ferreteria.DTOS
{
    public class SaveCargoDto
    {
        public int? Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
    }
}
