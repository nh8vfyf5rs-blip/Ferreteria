using Ferreteria.DTOS;
using Ferreteria.Models;
using Ferreteria.Repositorios;

namespace Ferreteria.Services
{
    public class RolePermissionService(
        IRolRepository roleRepo,
        IPermisoRepository permissionRepo,
        IUsuarioRepository userRepo) : IRolePermissionService

    {
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

        public async Task<IReadOnlyList<RolDto>> GetAllRolesAsync()
        {
            var roles = await roleRepo.GetAllAsync();

            return roles.Select(r => new RolDto
            {
                Id = r.Id,
                Name = r.Name
            }).ToList();
        }

        public async Task<RolDto?> GetRoleByIdAsync(int roleId)
        {
            var role = await roleRepo.GetByIdAsync(roleId);

            if (role == null)
                return null;

            return new RolDto
            {
                Id = role.Id,
                Name = role.Name
            };
        }

        public async Task<IReadOnlyList<PermisoDto>> GetRolePermissionsAsync(
            int roleId)
        {
            var role = await roleRepo.GetByIdAsync(roleId);

            if (role == null)
                return [];

            return role.RolPermisos
                .Where(rp => rp.IsGranted)
                .Select(rp => new PermisoDto
                {
                    Id = rp.PermisoId,
                    Name = rp.Permiso.Name,
                    Description = rp.Permiso.Descripcion
                })
                .OrderBy(p => p.Name)
                .ToList();
        }

        public async Task<(bool Success, string? ErrorMessage)>
            AssignPermissionToRoleAsync(int roleId, int permissionId)
        {
            var role = await roleRepo.GetByIdAsync(roleId);

            if (role == null)
                return (false, "Rol no encontrado.");

            var permission = await permissionRepo.GetByIdAsync(permissionId);

            if (permission == null)
                return (false, "Permiso no encontrado.");

            var existing = role.RolPermisos.FirstOrDefault(
                rp => rp.PermisoId == permissionId);

            if (existing != null)
            {
                if (existing.IsGranted)
                    return (false, "El rol ya tiene asignado ese permiso.");

                existing.IsGranted = true;
            }
            else
            {
                role.RolPermisos.Add(new RolPermiso
                {
                    RolId = roleId,
                    PermisoId = permissionId,
                    IsGranted = true
                });
            }

            var updated = await roleRepo.UpdateAsync(role);

            if (!updated)
                return (false, "No fue posible asignar el permiso al rol.");

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)>
            RemovePermissionFromRoleAsync(int roleId, int permissionId)
        {
            var role = await roleRepo.GetByIdAsync(roleId);

            if (role == null)
                return (false, "Rol no encontrado.");

            var existing = role.RolPermisos.FirstOrDefault(
                rp => rp.PermisoId == permissionId && rp.IsGranted);

            if (existing == null)
                return (false, "El rol no tiene asignado ese permiso.");

            role.RolPermisos.Remove(existing);

            var updated = await roleRepo.UpdateAsync(role);

            if (!updated)
                return (false, "No fue posible remover el permiso del rol.");

            return (true, null);
        }

        public async Task<UserPermissionsDto?> GetUserPermissionsAsync(int userId)
        {
            var user = await userRepo.GetUsuarioByIdWhitPermissionAsync(userId);

            if (user == null)
                return null;

            var rolePermissions = user.Rol.RolPermisos
                .Where(rp => rp.IsGranted)
                .Select(rp => rp.Permiso.Name)
                .ToHashSet();

            foreach (var overridePermission in user.UsuarioPermisos)
            {
                if (overridePermission.IsGranted)
                    rolePermissions.Add(overridePermission.permiso.Name);
                else
                    rolePermissions.Remove(overridePermission.permiso.Name);
            }

            return new UserPermissionsDto
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                RoleId = user.RolId,
                RoleName = user.Rol.Name,
                EffectivePermission = rolePermissions
                    .OrderBy(p => p)
                    .ToList(),
                DirectPermiso = user.UsuarioPermisos.Select(up =>
                    new UserPermisoOverrideDto
                    {
                        PermissionId = up.PermisoId,
                        PermissionName = up.permiso.Name,
                        IsGranted = up.IsGranted
                    }).ToList()
            };
        }

        public async Task<(bool Success, string? ErrorMessage)>
            AssignPermissionToUserAsync(int userId, AssingUserPermisoDto dto)
        {
            var user = await userRepo.GetUsuarioByIdWhitPermissionAsync(userId);

            if (user == null)
                return (false, "Usuario no encontrado.");

            var permission = await permissionRepo.GetByIdAsync(dto.PermisoId);

            if (permission == null)
                return (false, "Permiso no encontrado.");

            await userRepo.AssingPermissionToUsuarioAsync(
                userId,
                dto.PermisoId,
                dto.IsGranted);

            await userRepo.SaveAsync();

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)>
            RemoveUserPermissionAsync(int userId, int permissionId)
        {
            var user = await userRepo.GetUsuarioByIdWhitPermissionAsync(userId);

            if (user == null)
                return (false, "Usuario no encontrado.");

            var removed = await userRepo.RemovePermissionFromeUsuarioAsync(
                userId,
                permissionId);

            if (!removed)
                return (false, "El usuario no tiene una asignación directa para este permiso.");

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)>
            UpdateUserRoleAsync(int userId, int roleId)
        {
            var role = await roleRepo.GetByIdAsync(roleId);

            if (role == null)
                return (false, "Rol no encontrado.");

            var user = await userRepo.GetByIdAsync(userId);

            if (user == null)
                return (false, "Usuario no encontrado.");

            var updated = await userRepo.UpdateUsuarioRoleAsync(userId, roleId);

            if (!updated)
                return (false, "No fue posible actualizar el rol del usuario.");

            await userRepo.SaveAsync();

            return (true, null);
        }
    }
}
