using System.ComponentModel.DataAnnotations;

namespace Ferreteria.DTOS
{
    public class UpdateInventarioDTO
    {
        [Required]
        public int AlmacenId { get; set; }

        [Required]
        public int ProductoId { get; set; }

        [Range(0, int.MaxValue)]
        public int Stock {  get; set; }
    }
}
