using Ferreteria.DTOS;

namespace Ferreteria.Services
{
    public interface IRolePermissionService
    {
        Task<IReadOnlyList<PermisoDto>> GetAllPermissionsAsync();

        Task<IReadOnlyList<RolDto>> GetAllRolesAsync();

        Task<RolDto?> GetRoleByIdAsync(int roleId);

        Task<IReadOnlyList<PermisoDto>> GetRolePermissionsAsync(int roleId);

        Task<(bool Success, string? ErrorMessage)> AssignPermissionToRoleAsync(
            int roleId, int permissionId);

        Task<(bool Success, string? ErrorMessage)> RemovePermissionFromRoleAsync(
            int roleId, int permissionId);

        Task<UserPermissionsDto?> GetUserPermissionsAsync(int userId);

        Task<(bool Success, string? ErrorMessage)> AssignPermissionToUserAsync(
            int userId, AssingUserPermisoDto dto);

        Task<(bool Success, string? ErrorMessage)> RemoveUserPermissionAsync(
            int userId, int permissionId);

        Task<(bool Success, string? ErrorMessage)> UpdateUserRoleAsync(
            int userId, int roleId);
    }
}
