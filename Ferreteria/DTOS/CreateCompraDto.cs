using System.ComponentModel.DataAnnotations;

namespace Ferreteria.DTOS
{
    public class CreateCompraDto
    {
        [Required] 
        public int ProveedorId { get; set; }
      
        [Required]
        public int EmpleadoId { get; set; }

        [Required]
        public int AlmacenId { get; set; }
        public DateTime FechaCompra { get; set; }= DateTime.Now;

        [Required]
        [MinLength(1, ErrorMessage ="Debe agregar al menos un producto.")]
        public List<DetalleCompraDto> DetalleCompraDtos { get; set; }=new();

    }
}
