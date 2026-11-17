using Ferreteria.Models.Segurudad;

namespace Ferreteria.Repositorios
{
    public interface IRolRepository
    {
        Task<IReadOnlyList<Rol>> GetAllAsync();
        Task<Rol?> GetByIdAsync(int id);
        Task<Rol?> CreateAsync(Rol rol);
        Task<bool> UpdateAsync(Rol rol);
        Task<bool> DeleteAsync(int id);
    }
}
