using Ferreteria.Data;
using Ferreteria.Models;
using Ferreteria.Models.Segurudad;
using Microsoft.EntityFrameworkCore;

namespace Ferreteria.Repositorios
{
    public class UsuarioRepository(AppDbContext context) : IUsuarioRepository
    {
        private IQueryable<Usuario> UserWithPermission =>
            context.Usuarios
                .Include(u => u.Rol)
                    .ThenInclude(r => r.RolPermisos)
                        .ThenInclude(rp => rp.Permiso)
                .Include(u => u.UsuarioPermisos)
                    .ThenInclude(up => up.permiso);

        public async Task<Usuario?> CreateAsync(Usuario usuario)
        {
            var entry = await context.Usuarios.AddAsync(usuario);
            await context.SaveChangesAsync();

            return entry.Entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var usuario = await context.Usuarios.FindAsync(id);

            if (usuario == null)
                return false;

            context.Usuarios.Remove(usuario);
            await context.SaveChangesAsync();

            return true;
        }

        public void Delete(Usuario usuario)
        {
            context.Usuarios.Remove(usuario);
        }

        public async Task<IReadOnlyList<Usuario>> GetAllAsync()
        {
            return await context.Usuarios
                .AsNoTracking()
                .Include(u => u.Rol)
                .Include(u => u.UsuarioPermisos)
                    .ThenInclude(up => up.permiso)
                .OrderBy(u => u.UserName)
                .ToListAsync();
        }

        public async Task<Usuario?> GetByIdAsync(int id)
        {
            return await context.Usuarios
                .Include(u => u.Rol)
                .Include(u => u.UsuarioPermisos)
                    .ThenInclude(up => up.permiso)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<Usuario?> GetUsuarioByUserName(string Username)
        {
            return await UserWithPermission
                .FirstOrDefaultAsync(u => u.UserName == Username);
        }

        public async Task<Usuario?> GetUsuarioByEmail(string Email)
        {
            return await UserWithPermission
                .FirstOrDefaultAsync(u => u.Email == Email);
        }

        public async Task<Usuario?> GetUsuarioByIdWhitPermissionAsync(int Id)
        {
            return await UserWithPermission
                .FirstOrDefaultAsync(u => u.Id == Id);
        }

        public async Task<Usuario?> GetUsuarioByRefreshTokenAsync(string refreshToken)
        {
            return await UserWithPermission
                .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
        }

        public async Task<IReadOnlyList<Usuario>> GetAllUsuariosWhitRolesAsync()
        {
            return await context.Usuarios
                .Include(u => u.Rol)
                .OrderBy(u => u.UserName)
                .ToListAsync();
        }

        public async Task<bool> AssingPermissionToUsuarioAsync(
            int usuarioId, int permissionId, bool IsGranted)
        {
            var userPermission = await context.UsuariosPermisos
                .FirstOrDefaultAsync(up =>
                    up.UsuarioId == usuarioId &&
                    up.PermisoId == permissionId);

            if (userPermission != null)
            {
                userPermission.IsGranted = IsGranted;
            }
            else
            {
                context.UsuariosPermisos.Add(new UsuarioPermiso
                {
                    UsuarioId = usuarioId,
                    PermisoId = permissionId,
                    IsGranted = IsGranted
                });
            }

            return true;
        }

        public async Task<bool> RemovePermissionFromeUsuarioAsync(
            int usuarioId, int permissionId)
        {
            var userPermission = await context.UsuariosPermisos
                .FirstOrDefaultAsync(up =>
                    up.UsuarioId == usuarioId &&
                    up.PermisoId == permissionId);

            if (userPermission == null)
                return false;

            context.UsuariosPermisos.Remove(userPermission);
            await context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateUsuarioRoleAsync(
            int UsuarioId, int RoleId)
        {
            var usuario = await context.Usuarios.FindAsync(UsuarioId);

            if (usuario == null)
                return false;

            var roleExists = await context.Roles
                .AnyAsync(r => r.Id == RoleId);

            if (!roleExists)
                return false;

            usuario.RolId = RoleId;

            return true;
        }

        public async Task<bool> UpdateAsync(Usuario usuario)
        {
            context.Usuarios.Update(usuario);
            await context.SaveChangesAsync();

            return true;
        }

        public async Task SaveAsync()
        {
            await context.SaveChangesAsync();
        }

        public bool ValidatePassword(Usuario usuario, string password)
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(
                password,
                usuario.Password);
        }
    }
}