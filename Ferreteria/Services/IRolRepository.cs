using Ferreteria.Models.Segurudad;

namespace Ferreteria.Services
{
    public interface IRolRepository
    {
        Task<IReadOnlyList<Rol>> GetAllWithPermissionsAsync();
        Task<Rol?> GetByIdWithPermissionAsync(int Id);
        Task<Rol?> GetByNameAsync(string name);
        Task<Rol> CreateAsync(string name);
        Task<bool> AssingPermissionToRoleAsync(int rolId, int permisoId);
        Task<bool> RemovePermissionFromRolAsync(int rolId, int permisoId);
    }
}

