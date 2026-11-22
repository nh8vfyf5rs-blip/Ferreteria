namespace Ferreteria.DTOS
{
    public class MetodoPagoTableDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int CantidadPagos { get; set; }
        public decimal TotalPagos { get; set; }
    }
}