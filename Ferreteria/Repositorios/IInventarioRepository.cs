using Ferreteria.Models;

namespace Ferreteria.Repositorios
{
    public interface IInventarioRepository
    {
        Task<IReadOnlyList<Inventario>> GetAllAsync();
        Task<Inventario?> GetByIdAsync(int id);
        Task<Inventario?> CreateAsync(Inventario inventario);
        Task<bool> UpdateAsync(Inventario inventario);
        Task<bool> DeleteAsync(int id);
    }
}
