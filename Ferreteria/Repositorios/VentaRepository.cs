using Ferreteria.Data;
using Ferreteria.Models;
using Microsoft.EntityFrameworkCore;

namespace Ferreteria.Repositorios
{
    public class VentaRepository(AppDbContext context) : IVentaRepository
    {
        public async Task<Venta?> CreateAsync(Venta venta)
        {
            var entry = await context.Ventas.AddAsync(venta);
            await context.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var venta = await context.Ventas.FindAsync(id);
            if (venta == null)
                return false;

            context.Ventas.Remove(venta);
            await context.SaveChangesAsync();
            return true;
        }

        public async  Task<IReadOnlyList<Venta>> GetAllAsync()
        {
            return await context.Ventas
                .AsNoTracking()
                .Include(v => v.Cliente)
                .Include(v => v.DetalleVentas)
                    .ThenInclude(d => d.Producto)
                .Include(v => v.DetallePagos)
                    .ThenInclude(p => p.MetodoPago)
                .OrderByDescending(v => v.FechaVenta)
                .ToListAsync();
        }

        public async Task<Venta?> GetByIdAsync(int id)
        {
            return await context.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.DetalleVentas)
                    .ThenInclude(d => d.Producto)
                .Include(v => v.DetallePagos)
                    .ThenInclude(p => p.MetodoPago)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async  Task<bool> UpdateAsync(Venta venta)
        {
            context.Ventas.Update(venta);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
