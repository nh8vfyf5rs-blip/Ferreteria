using Ferreteria.Models.Segurudad;

namespace Ferreteria.Repositorios
{
    public interface IRolRepository
    {
        Task<IReadOnlyList<Rol>> GetAllAsync();
        Task<IReadOnlyList<Rol>> GetAllWithPermissionsAsync();
        Task<Rol?> GetByIdAsync(int rolId);
        Task<Rol?> GetByIdWithPermissionAsync(int id);
        Task<Rol?> GetByNameAsync(string name);
        Task<Rol> CreateAsync(string name);
        Task<Rol?> CreateAsync(Rol rol);
        Task<bool> AssingPermissionToRoleAsync(int rolId, int permisoId);
        Task<bool> RemovePermissionFromRolAsync(int rolId, int permisoId);
    }
}