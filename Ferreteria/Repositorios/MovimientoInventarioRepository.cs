using Ferreteria.Data;
using Ferreteria.Models;
using Microsoft.EntityFrameworkCore;

namespace Ferreteria.Repositorios
{
    public class MovimientoInventarioRepository(AppDbContext context) : IMovimientoInventarioRepository
    {
        public async Task<MovimientoInventario?> CreateAsync(MovimientoInventario movimiento)
        {
            var entry = await context.MovimientoInventarios.AddAsync(movimiento);
            await context.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var movimiento = await context.MovimientoInventarios.FindAsync(id);

            if (movimiento == null)
                return false;

            context.MovimientoInventarios.Remove(movimiento);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<IReadOnlyList<MovimientoInventario>> GetAllAsyc()
        {
            return await context.MovimientoInventarios
               .AsNoTracking()
               .Include(m => m.Inventario)
                   .ThenInclude(i => i.Producto)
               .Include(m => m.Inventario)
                   .ThenInclude(i => i.Almacen)
               .OrderByDescending(m => m.Fecha)
               .ToListAsync();
        }

        public async  Task<MovimientoInventario?> GetByIdAsync(int id)
        {
            return await context.MovimientoInventarios
                .Include(m => m.Inventario)
                    .ThenInclude(i => i.Producto)
                .Include(m => m.Inventario)
                    .ThenInclude(i => i.Almacen)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<bool> UpdateAsync(MovimientoInventario movimiento)
        {
            context.MovimientoInventarios.Update(movimiento);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
