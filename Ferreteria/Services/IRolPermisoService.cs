using Ferreteria.DTOS;

namespace Ferreteria.Service
{
    public interface IRolPermisoService
    {
        Task<IReadOnlyList<PermisoDto>> GetAllPermissionsAsync();
        Task<IReadOnlyList<RoleWithPermissionDto>> GetAllRolesAsync();
        Task<RoleWithPermissionDto?> GetRoleByIdAsync(int roleId);
        Task<(bool Success, string? ErrorMessage)> AssignPermissionToRoleAsync(int roleId, int permissionId);
        Task<(bool Success, string? ErrorMessage)> RemovePermissionFromRoleAsync(int roleId, int permissionId);
        Task<UserPermissionDto?> GetUserPermissionsAsync(int userId);
        Task<(bool Success, string? ErrorMessage)> AssignPermissionToUserAsync(int userId, AssingUserPermisoDto dto);
        Task<(bool Success, string? ErrorMessage)> RemoveUserPermissionAsync(int userId, int permissionId);
        Task<(bool Success, string? ErrorMessage)> UpdateUserRoleAsync(int userId, int roleId);
        Task<DataTableResponseDto<PermisoDto>> GetPermissionsPagedAsync(DataTablesRequestDto request);
        Task<PermisoDto?> GetPermissionByIdAsync(int id);
        Task<(bool Success, PermisoDto? Result, string? ErrorMessage)> CreatePermissionAsync(SavePermisoDto dto);
        Task<(bool Success, string? ErrorMessage)> UpdatePermissionAsync(SavePermisoDto dto);
        Task<(bool Success, string? ErrorMessage)> DeletePermissionAsync(int id);
        Task<(bool Success, RoleWithPermissionDto? Result, string? ErrorMessage)> CreateRoleAsync(CreateRolDto dto);
        Task<IReadOnlyList<UserListItemDto>> GetAllUsersAsync();
    }
}