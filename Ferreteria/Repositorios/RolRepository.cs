using Ferreteria.Data;
using Ferreteria.Models.Segurudad;
using Microsoft.EntityFrameworkCore;

namespace Ferreteria.Repositorios
{
    public class RolRepository(AppDbContext context) : IRolRepository
    {
        public async  Task<Rol?> CreateAsync(Rol rol)
        {
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
                .Include(r => r.RolPermisos)
                    .ThenInclude(rp => rp.Permiso)
                .Include(r => r.Usuarios)
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        public async Task<Rol?> GetByIdAsync(int id)
        {
            return await context.Roles
                .Include(r => r.RolPermisos)
                    .ThenInclude(rp => rp.Permiso)
                .Include(r => r.Usuarios)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async  Task<bool> UpdateAsync(Rol rol)
        {
            context.Roles.Update(rol);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
