using Ferreteria.Models;

namespace Ferreteria.Repositorios
{
    public interface IMovimientoInventarioRepository
    {
        Task<IReadOnlyList<MovimientoInventario>> GetAllAsyc();
        Task<MovimientoInventario?> GetByIdAsync(int id);
        Task<MovimientoInventario?> CreateAsync(MovimientoInventario movimiento);
        Task<bool> UpdateAsync(MovimientoInventario movimiento);
        Task<bool> DeleteAsync(int id);
    }
}
