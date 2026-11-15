using System.ComponentModel.DataAnnotations;

namespace Ferreteria.DTOS
{
    public class UpdateCompraDto
    {
        [Required]
        public int ProveedorId { get; set; }
       
        [Required]
        public int EmpleadoId { get; set; }

        [Required]
        public int AlmacenId { get; set; }

    }
}
