using Ferreteria.Models;

namespace Ferreteria.Repositorios
{
    public interface IVentaRepository
    {
        Task<IReadOnlyList<Venta>> GetAllAsync();
        Task<Venta?> GetByIdAsync(int id);
        Task<Venta?> CreateAsync(Venta venta);
        Task<bool> UpdateAsync(Venta venta);
        Task<bool> DeleteAsync(int id);
    }
}
