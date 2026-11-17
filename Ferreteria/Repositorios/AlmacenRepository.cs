using Ferreteria.Data;
using Ferreteria.Models;
using Microsoft.EntityFrameworkCore;

namespace Ferreteria.Repositorios
{
    public class AlmacenRepository(AppDbContext context) : IAlmacenRepository
    {
        public async Task<Almacen?> CreateAsync(Almacen almacen)
        {
            var entry = await context.Almacenes.AddAsync(almacen);
            await context.SaveChangesAsync();

            return entry.Entity;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var almacen = await context.Almacenes.FindAsync(id);
            if (almacen == null) return false;
            
            context.Almacenes.Remove(almacen);
            await context.SaveChangesAsync();
            return true;
        }
        
        public async Task<IReadOnlyList<Almacen>> GetAllAsync()
        {
            return await context.Almacenes
                .Include(a => a.Sucursal)
                .OrderBy(a => a.Name)
                .ToListAsync();
        }
        public async Task<Almacen?> GetByIdAsync(int id)
        {
            return await context.Almacenes
                .Include(a => a.Sucursal)
                .FirstOrDefaultAsync(a => a.Id == id);
        }
        public async Task<bool> UpdateAsync(Almacen almacen)
        {
            context.Almacenes.Update(almacen);
            await context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> HasInventariosAsync(int almacenId)
        {
            return await context.Inventarios
                .AnyAsync(i => i.AlmacenId == almacenId);
        }

    }
}
