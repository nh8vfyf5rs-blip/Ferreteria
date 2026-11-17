using Ferreteria.Models;

namespace Ferreteria.Repositorios
{
    public interface ISucursalRepository
    {
        Task<IReadOnlyList<Sucursal>> GetAllAsync();
        Task<Sucursal?> GetByIdAsync(int id);
        Task<Sucursal?> CreateAsync(Sucursal sucursal);
        Task<bool> UpdateAsync(Sucursal sucursal);
        Task<bool> DeleteAsync(int id);
    }
}
