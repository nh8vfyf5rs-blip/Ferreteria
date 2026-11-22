using Ferreteria.Data;
using Ferreteria.Models;
using Microsoft.EntityFrameworkCore;

namespace Ferreteria.Repositorios
{
    public class DetalleInventarioRepository(AppDbContext context) : IDetalleInventarioRepository
    {
        public async Task<DetalleInventario?> CreateAsync(DetalleInventario detalle)
        {
            var entry = await context.DetalleInventarios.AddAsync(detalle);
            await context.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var detalle = await context.DetalleInventarios.FindAsync(id);

            if (detalle == null)
                return false;

            context.DetalleInventarios.Remove(detalle);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<IReadOnlyList<DetalleInventario>> GetAllAsync()
        {
            return await context.DetalleInventarios
                .AsNoTracking()
                .Include(d => d.Sucursal)
                .Include(d => d.Inventario)
                    .ThenInclude(i => i.Producto)
                .Include(d => d.Inventario)
                    .ThenInclude(i => i.Almacen)
                        .ThenInclude(a => a.Sucursal)
                .OrderByDescending(d => d.FechaRegistro)
                .ToListAsync();
        }

        public async Task<DetalleInventario?> GetByIdAsync(int id)
        {
            return await context.DetalleInventarios
                .Include(d => d.Sucursal)
                .Include(d => d.Inventario)
                    .ThenInclude(i => i.Producto)
                .Include(d => d.Inventario)
                    .ThenInclude(i => i.Almacen)
                        .ThenInclude(a => a.Sucursal)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<bool> UpdateAsync(DetalleInventario detalle)
        {
            var existing = await context.DetalleInventarios.FindAsync(detalle.Id);

            if (existing == null)
                return false;

            context.Entry(existing).CurrentValues.SetValues(detalle);
            await context.SaveChangesAsync();
            return true;
        }
    }
}