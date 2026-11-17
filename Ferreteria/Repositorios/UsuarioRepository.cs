using Ferreteria.Data;
using Ferreteria.Models;
using Microsoft.EntityFrameworkCore;

namespace Ferreteria.Repositorios
{
    public class UsuarioRepository(AppDbContext context) : IUsuarioRepository
    {
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

        public async Task<bool> UpdateAsync(Usuario usuario)
        {
            context.Usuarios.Update(usuario);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
