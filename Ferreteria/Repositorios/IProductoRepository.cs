using Ferreteria.Models;

namespace Ferreteria.Repositorios
{
    public interface IProductoRepository
    {
        Task<IReadOnlyList<Producto>> GetAllAsync();
        Task<Producto?> GetByIdAsync(int id);
        Task<Producto?> CreateAsync(Producto producto);
        Task<bool> UpdateAsync(Producto producto);
        Task<bool> DeleteAsync(int id);

    }
}
