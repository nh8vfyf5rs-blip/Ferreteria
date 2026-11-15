using Ferreteria.Models;

namespace Ferreteria.DTOS
{
    public class DetalleCompraTableDto
    {
    
            public int Id { get; set; }
            public int CompraId { get; set; }
            public int ProductoId { get; set; }
             public string ProductoNombre { get; set; }=string.Empty;
            public int CantidadRecibida { get; set; }
            public decimal CostoUnitario { get; set; }
            public decimal Subtotal { get; set; }
        }
    }
