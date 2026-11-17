using Ferreteria.Data;
using Ferreteria.Models;
using Microsoft.EntityFrameworkCore;

namespace Ferreteria.Repositorios
{
    public class CompraRepository(AppDbContext context) : ICompraRepository
    {
        public async Task<Compra?> CreateAsync(Compra compra)
        {
            var entry = await context.Compras.AddAsync(compra);
            await context.SaveChangesAsync();

            return entry.Entity;

        }

        public async Task<bool> DeleteAsync(int id)
        {
            var compra = await context.Compras.FindAsync(id);

            if (compra == null) return false;

            context.Compras.Remove(compra);
            await context.SaveChangesAsync();

            return true;
        }

        public async Task<IReadOnlyList<Compra>> GetAllAsync()
        {
            return await context.Compras
                .AsNoTracking()
                .Include(c => c.Proveedor)
                .Include(c => c.Empleado)
                .Include(c => c.DetalleCompras)
                    .ThenInclude(d => d.Producto)
                .OrderByDescending(c => c.FechaCompra)
                .ToListAsync();
        }

        public async Task<Compra?> GetByIdAsync(int id)
        {
            return await context.Compras

                .Include(c => c.Proveedor)
                .Include(c => c.Empleado)
                .Include(c => c.DetalleCompras)
                    .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<bool> UpdateAsync(Compra compra)
        {
            context.Compras.Update(compra);
            await context.SaveChangesAsync();

            return true;
        }
    }
}
