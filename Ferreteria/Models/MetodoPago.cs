namespace Ferreteria.Models
{
    public class MetodoPago
    {
        public int Id { get; set; }
        public string Name { get; set; }=string.Empty;
        public virtual ICollection<DetallePago> DetallePagos { get; set; } = [];
    }
}
