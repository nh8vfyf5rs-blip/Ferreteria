using BCrypt.Net;
using Ferreteria.Data;
using Ferreteria.Models.Segurudad;
using Microsoft.EntityFrameworkCore;

namespace Ferreteria.Repositorios
{
    public class UsuarioRepository(AppDbContext context) : IUsuarioRepository
    {
        private IQueryable<Usuario> UserWithPermission =>
            context.Usuarios
                .Include(u => u.Rol)
                    .ThenInclude(r => r.RolPermiso)
                        .ThenInclude(rp => rp.Permiso)
                .Include(u => u.UsuarioPermisos)
                    .ThenInclude(up => up.Permiso);

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
                .OrderBy(u => u.UserName)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Usuario>> GetAllUsuarioAsync()
        {
            return await GetAllAsync();
        }

        public async Task<Usuario?> GetByIdAsync(int id)
        {
            return await UserWithPermission
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<Usuario?> GetUsuarioByIdWhitPermissionAsync(int userId)
        {
            return await UserWithPermission
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<Usuario?> GetUsuarioByEmail(string email)
        {
            var normalized = email.Trim().ToLower();
            return await UserWithPermission
                .FirstOrDefaultAsync(u => u.Email.ToLower() == normalized);
        }

        public async Task<Usuario?> GetUsuarioByUserName(string userName)
        {
            var normalized = userName.Trim().ToLower();
            return await UserWithPermission
                .FirstOrDefaultAsync(u => u.UserName.ToLower() == normalized);
        }

        public async Task<Usuario?> GetUsuarioByRefreshTokenAsync(string refreshToken)
        {
            return await UserWithPermission
                .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
        }

        public async Task<bool> UpdateAsync(Usuario usuario)
        {
            var entry = context.Entry(usuario);
            if (entry.State == EntityState.Detached)
            {
                context.Usuarios.Attach(usuario);
                entry.State = EntityState.Modified;
            }

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateUsuarioRoleAsync(int usuarioId, int rolId)
        {
            var user = await context.Usuarios.FindAsync(usuarioId);
            if (user == null)
                return false;

            var roleExists = await context.Roles.AnyAsync(r => r.Id == rolId);
            if (!roleExists)
                return false;

            user.RolId = rolId;
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AssingPermissionToUsuarioAsync(
            int usuarioId,
            int permisoId,
            bool isGranted)
        {
            var userPermission = await context.UsuariosPermisos
                .FirstOrDefaultAsync(up =>
                    up.UsuarioId == usuarioId &&
                    up.PermisoId == permisoId);

            if (userPermission != null)
            {
                userPermission.IsGranted = isGranted;
            }
            else
            {
                await context.UsuariosPermisos.AddAsync(new UsuarioPermiso
                {
                    UsuarioId = usuarioId,
                    PermisoId = permisoId,
                    IsGranted = isGranted
                });
            }

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemovePermissionFromeUsuarioAsync(
            int usuarioId,
            int permisoId)
        {
            var userPermission = await context.UsuariosPermisos
                .FirstOrDefaultAsync(up =>
                    up.UsuarioId == usuarioId &&
                    up.PermisoId == permisoId);

            if (userPermission == null)
                return false;

            context.UsuariosPermisos.Remove(userPermission);
            await context.SaveChangesAsync();
            return true;
        }

        public bool ValidatePassword(Usuario usuario, string password)
        {
            if (string.IsNullOrWhiteSpace(usuario.Password) || string.IsNullOrWhiteSpace(password))
                return false;

            try
            {
                return BCrypt.Net.BCrypt.EnhancedVerify(password, usuario.Password);
            }
            catch
            {
                return false;
            }
        }

        public async Task SaveAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}