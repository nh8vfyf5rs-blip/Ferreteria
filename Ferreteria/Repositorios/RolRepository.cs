using Ferreteria.Data;
using Ferreteria.Models.Segurudad;
using Ferreteria.Services;
using Microsoft.EntityFrameworkCore;

namespace Ferreteria.Repositorios
{
    public class RolRepository(AppDbContext context) : IRolRepository
    {
        public async Task<Rol?> CreateAsync(Rol rol)
        {
            var entry = await context.Roles.AddAsync(rol);
            await context.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<Rol> CreateAsync(string name)
        {
            var rol = new Rol { Name = name.Trim() };
            var entry = await context.Roles.AddAsync(rol);
            await context.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var rol = await context.Roles.FindAsync(id);
            if (rol == null)
                return false;

            context.Roles.Remove(rol);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<IReadOnlyList<Rol>> GetAllAsync()
        {
            return await context.Roles
                .AsNoTracking()
                .Include(r => r.RolPermiso)
                    .ThenInclude(rp => rp.Permiso)
                .Include(r => r.Usuarios)
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Rol>> GetAllWithPermissionsAsync()
        {
            return await context.Roles
                .AsNoTracking()
                .Include(r => r.RolPermiso)
                    .ThenInclude(rp => rp.Permiso)
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        public async Task<Rol?> GetByIdAsync(int id)
        {
            return await context.Roles
                .Include(r => r.RolPermiso)
                    .ThenInclude(rp => rp.Permiso)
                .Include(r => r.Usuarios)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Rol?> GetByIdWithPermissionAsync(int id)
        {
            return await context.Roles
                .Include(r => r.RolPermiso)
                    .ThenInclude(rp => rp.Permiso)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Rol?> GetByNameAsync(string name)
        {
            return await context.Roles
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Name == name);
        }

        public async Task<bool> AssingPermissionToRoleAsync(int rolId, int permisoId)
        {
            var rol = await context.Roles
                .Include(r => r.RolPermiso)
                .FirstOrDefaultAsync(r => r.Id == rolId);

            if (rol == null)
                return false;

            var existing = rol.RolPermiso.FirstOrDefault(rp => rp.PermisoId == permisoId);
            if (existing != null)
            {
                existing.IsGranted = true;
            }
            else
            {
                rol.RolPermiso.Add(new RolPermiso
                {
                    RolId = rolId,
                    PermisoId = permisoId,
                    IsGranted = true
                });
            }

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemovePermissionFromRolAsync(int rolId, int permisoId)
        {
            var rol = await context.Roles
                .Include(r => r.RolPermiso)
                .FirstOrDefaultAsync(r => r.Id == rolId);

            if (rol == null)
                return false;

            var existing = rol.RolPermiso.FirstOrDefault(rp => rp.PermisoId == permisoId);
            if (existing == null)
                return false;

            rol.RolPermiso.Remove(existing);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(Rol rol)
        {
            context.Roles.Update(rol);
            await context.SaveChangesAsync();
            return true;
        }
    }
}