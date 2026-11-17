using Ferreteria.Models;

namespace Ferreteria.Repositorios
{
    public interface IPermisoRepository
    {
        Task<IReadOnlyList<Permiso>> GetAllAsync();
        Task<Permiso?> GetByIdAsync(int id);
        Task<Permiso?> CreateAsync(Permiso permiso);
        Task<bool> UpdateAsync(Permiso permiso);
        Task<bool> DeleteAsync(int id);
    }
}
