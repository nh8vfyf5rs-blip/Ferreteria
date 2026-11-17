using Ferreteria.Models;

namespace Ferreteria.Repositorios
{
    public interface IProveedorRepository
    {
        Task<IReadOnlyList<Proveedor>> GetAllAsync();
        Task<Proveedor?> GetByIdAsync(int id);
        Task<Proveedor?> CreateAsync(Proveedor proveedor);
        Task<bool> UpdateAsync(Proveedor proveedor);
        Task<bool> DeleteAsync(int id);
    }

}
