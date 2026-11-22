using Ferreteria.Models;

namespace Ferreteria.Repositorios
{
    public interface IDetalleInventarioRepository
    {
        Task<IReadOnlyList<DetalleInventario>> GetAllAsync();
        Task<DetalleInventario?> GetByIdAsync(int id);
        Task<DetalleInventario?> CreateAsync(DetalleInventario detalle);
        Task<bool> UpdateAsync(DetalleInventario detalle);
        Task<bool> DeleteAsync(int id);
    }
}
