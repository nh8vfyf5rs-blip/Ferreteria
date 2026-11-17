using Ferreteria.Data;
using Ferreteria.Models;
using Microsoft.EntityFrameworkCore;

namespace Ferreteria.Repositorios
{
    public class DevolucionesRepository(AppDbContext context) : IDevolucionesRepository
    {
        public async Task<Devoluciones?> CreateAsync(Devoluciones devolucion)
        {
            var entry = await context.Devoluciones.AddAsync(devolucion);
            await context.SaveChangesAsync();

            return entry.Entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var devolucion = await context.Devoluciones.FindAsync(id);
            if (devolucion == null) return false;
        
            context.Devoluciones.Remove(devolucion);
            await context.SaveChangesAsync();

            return true;
        }

        public async Task<IReadOnlyList<Devoluciones>> GetAllAsync()
        {
            return await context.Devoluciones
                .AsNoTracking()
                .Include(d => d.Venta)
                    .ThenInclude(v => v.Cliente)
                .OrderByDescending(d => d.FechaDevolucion)
                .ToListAsync();
        }

        public async Task<Devoluciones?> GetByIdAsync(int id)
        {
            return await context.Devoluciones
                .Include(d => d.Venta)
                    .ThenInclude(v => v.Cliente)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<bool> UpdateAsync(Devoluciones devolucion)
        {
            context.Devoluciones.Update(devolucion);
            await context.SaveChangesAsync();

            return true;
        }
    }
}
