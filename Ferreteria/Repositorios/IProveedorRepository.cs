using Ferreteria.Models;

namespace Ferreteria.Repositorios
{
    public interface IProveedorRepository
    {
        Task<IEnumerable<Proveedor>> GetAllAsync();
        Task<Proveedor?> GetByIdAsync(int id);
        Task AddAsync(Proveedor proveedor);
        Task Update (Proveedor proveedor);
        Task Delete(Proveedor proveedor);
        Task SaveAsync();
    }

}
