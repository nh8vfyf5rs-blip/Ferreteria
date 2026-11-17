using Ferreteria.Data;
using Ferreteria.Models;
using Microsoft.EntityFrameworkCore;

namespace Ferreteria.Repositorios
{
    public class PermisoRepository(AppDbContext context) : IPermisoRepository
    {
        public async Task<Permiso?> CreateAsync(Permiso permiso)
        {
            var entry = await context.Permisos.AddAsync(permiso);
            await context.SaveChangesAsync();
            return entry.Entity;
        }

        public async  Task<bool> DeleteAsync(int id)
        {
            var permiso = await context.Permisos.FindAsync(id);

            if (permiso == null)
                return false;

            context.Permisos.Remove(permiso);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<IReadOnlyList<Permiso>> GetAllAsync()
        {
            return await context.Permisos
                .AsNoTracking()
                .Include(p => p.RolPermissions)
                    .ThenInclude(rp => rp.Rol)
                .Include(p => p.UsuarioPermisos)
                    .ThenInclude(up => up.Usuario)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<Permiso?> GetByIdAsync(int id)
        {
            return await context.Permisos
               .Include(p => p.RolPermissions)
                   .ThenInclude(rp => rp.Rol)
               .Include(p => p.UsuarioPermisos)
                   .ThenInclude(up => up.Usuario)
               .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<bool> UpdateAsync(Permiso permiso)
        {
            context.Permisos.Update(permiso);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
