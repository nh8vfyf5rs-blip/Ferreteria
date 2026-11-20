using Ferreteria.DTOS;
using Ferreteria.Helpers;
using Ferreteria.Models.Segurudad;
using Ferreteria.Repositorios;
using Ferreteria.Service;

namespace Ferreteria.Services
{
    public class RolePermissionService(
        IRolRepository roleRepo,
        IPermisoRepository permissionRepo,
        IUsuarioRepository userRepo) : IRolPermisoService
    {
        public async Task<(bool Success, string? ErrorMessage)> AssignPermissionToRoleAsync(int roleId, int permissionId)
        {
            var role = await roleRepo.GetByIdWithPermissionAsync(roleId);
            if (role == null)
                return (false, "Rol no encontrado");

            var permission = await permissionRepo.GetByIdAsync(permissionId);
            if (permission == null)
                return (false, "Permiso no encontrado");

            var assigned = await roleRepo.AssingPermissionToRoleAsync(roleId, permissionId);
            if (!assigned)
                return (false, "El rol ya tiene asignado ese permiso");

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> AssignPermissionToUserAsync(int userId, AssingUserPermisoDto dto)
        {
            var user = await userRepo.GetUsuarioByIdWhitPermissionAsync(userId);
            if (user == null)
                return (false, "Usuario no encontrado");

            var permission = await permissionRepo.GetByIdAsync(dto.PermisoId);
            if (permission == null)
                return (false, "Permiso no encontrado");

            await userRepo.AssingPermissionToUsuarioAsync(userId, dto.PermisoId, dto.IsGranted);
            return (true, null);
        }

        public async Task<(bool Success, PermisoDto? Result, string? ErrorMessage)> CreatePermissionAsync(SavePermisoDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return (false, null, "El nombre del permiso es obligatorio");

            var name = dto.Name.Trim().ToLowerInvariant();
            var existing = await permissionRepo.GetByNameAsync(name);
            if (existing != null)
                return (false, null, "Ya existe un permiso con ese nombre");

            var permission = await permissionRepo.CreateAsync(name, dto.Descripcion?.Trim());
            return (true, new PermisoDto
            {
                Id = permission.Id,
                Name = permission.Name,
                Description = permission.Descripcion
            }, null);
        }

        public async Task<(bool Success, RoleWithPermissionDto? Result, string? ErrorMessage)> CreateRoleAsync(CreateRolDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return (false, null, "El nombre del rol es obligatorio");

            var nombreRol = dto.Name.Trim();
            var existing = await roleRepo.GetByNameAsync(nombreRol);
            if (existing != null)
                return (false, null, "Ya existe un rol con ese nombre");

            var role = await roleRepo.CreateAsync(nombreRol);
            return (true, new RoleWithPermissionDto
            {
                Id = role.Id,
                Name = role.Name,
                Permisos = []
            }, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> DeletePermissionAsync(int id)
        {
            var permission = await permissionRepo.GetByIdAsync(id);
            if (permission == null)
                return (false, "Permiso no encontrado");

            if (await permissionRepo.IsAssingedToUsuarioAsync(id))
                return (false, "No se puede eliminar un permiso asignado a uno o más usuarios");

            await permissionRepo.DeleteAsync(id);
            return (true, null);
        }

        public async Task<IReadOnlyList<PermisoDto>> GetAllPermissionsAsync()
        {
            var permissions = await permissionRepo.GetAllAsync();
            return permissions.Select(p => new PermisoDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Descripcion
            }).ToList();
        }

        public async Task<IReadOnlyList<RoleWithPermissionDto>> GetAllRolesAsync()
        {
            var roles = await roleRepo.GetAllWithPermissionsAsync();
            return roles.Select(MapRole).ToList();
        }

        public async Task<IReadOnlyList<UserListItemDto>> GetAllUsersAsync()
        {
            var users = await userRepo.GetAllUsuarioAsync();
            return users.Select(u => new UserListItemDto
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                RolName = u.Rol?.Name ?? string.Empty
            }).ToList();
        }

        public async Task<PermisoDto?> GetPermissionByIdAsync(int id)
        {
            var permission = await permissionRepo.GetByIdAsync(id);
            if (permission == null)
                return null;

            return new PermisoDto
            {
                Id = permission.Id,
                Name = permission.Name,
                Description = permission.Descripcion
            };
        }

        public async Task<DataTableResponseDto<PermisoDto>> GetPermissionsPagedAsync(DataTablesRequestDto request)
        {
            return await permissionRepo.GetPagedAsync(request);
        }

        public async Task<RoleWithPermissionDto?> GetRoleByIdAsync(int roleId)
        {
            var role = await roleRepo.GetByIdWithPermissionAsync(roleId);
            return role == null ? null : MapRole(role);
        }

        public async Task<UserPermissionDto?> GetUserPermissionsAsync(int userId)
        {
            var user = await userRepo.GetUsuarioByIdWhitPermissionAsync(userId);
            if (user == null)
                return null;

            return new UserPermissionDto
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                RoleId = user.RolId,
                RolName = user.Rol?.Name ?? string.Empty,
                EffectivePermission = EffectivePermission.FromUser(user).ToList(),
                DirectPermission = user.UsuarioPermisos.Select(up => new UserPermisoOverrideDto
                {
                    PermissionId = up.PermisoId,
                    PermissionName = up.Permiso?.Name ?? string.Empty,
                    IsGranted = up.IsGranted
                }).ToList()
            };
        }

        public async Task<(bool Success, string? ErrorMessage)> RemovePermissionFromRoleAsync(int roleId, int permissionId)
        {
            var removed = await roleRepo.RemovePermissionFromRolAsync(roleId, permissionId);
            if (!removed)
                return (false, "El rol no tiene asignado este permiso");

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> RemoveUserPermissionAsync(int userId, int permissionId)
        {
            var removed = await userRepo.RemovePermissionFromeUsuarioAsync(userId, permissionId);
            if (!removed)
                return (false, "El usuario no tiene una asignación directa para este permiso");

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdatePermissionAsync(SavePermisoDto dto)
        {
            if (!dto.Id.HasValue || dto.Id.Value <= 0)
                return (false, "Id de permiso inválido");

            if (string.IsNullOrWhiteSpace(dto.Name))
                return (false, "El nombre del permiso es obligatorio");

            var permission = await permissionRepo.GetByIdAsync(dto.Id.Value);
            if (permission == null)
                return (false, "Permiso no encontrado");

            var name = dto.Name.Trim().ToLowerInvariant();
            var duplicate = await permissionRepo.GetByNameAsync(name);
            if (duplicate != null && duplicate.Id != permission.Id)
                return (false, "Ya existe un permiso con ese nombre");

            permission.Name = name;
            permission.Descripcion = dto.Descripcion?.Trim();
            await permissionRepo.UpdateAsync(permission);
            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateUserRoleAsync(int userId, int roleId)
        {
            var updated = await userRepo.UpdateUsuarioRoleAsync(userId, roleId);
            if (!updated)
                return (false, "Usuario o rol no encontrado");

            return (true, null);
        }

        private static RoleWithPermissionDto MapRole(Rol role) => new()
        {
            Id = role.Id,
            Name = role.Name,
            Permisos = role.RolPermiso.Select(rp => new PermisoDto
            {
                Id = rp.PermisoId,
                Name = rp.Permiso.Name,
                Description = rp.Permiso.Descripcion
            }).OrderBy(p => p.Name).ToList()
        };
    }
}