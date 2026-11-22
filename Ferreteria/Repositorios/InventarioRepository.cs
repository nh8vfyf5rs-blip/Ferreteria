using Ferreteria.Data;
using Ferreteria.Models;
using Microsoft.EntityFrameworkCore;

namespace Ferreteria.Repositorios
{
    public class InventarioRepository(AppDbContext context) : IInventarioRepository
    {
        public async Task<Inventario?> CreateAsync(Inventario inventario)
        {
            var entry = await context.Inventarios.AddAsync(inventario);
            await context.SaveChangesAsync();

            return entry.Entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var inventario = await context.Inventarios.FindAsync(id);
            if (inventario == null) return false;

            context.Inventarios.Remove(inventario);
            await context.SaveChangesAsync();

            return true;
        }

        public async Task<IReadOnlyList<Inventario>> GetAllAsync()
        {
            return await context.Inventarios
                .AsNoTracking()
                .Include(i => i.Almacen)
                .Include(i => i.Producto)
                .Include(i => i.DetalleInventarios)
                .OrderBy(i => i.Id)
                .ToListAsync();
        }

        public async Task<Inventario?> GetByIdAsync(int id)
        {
            return await context.Inventarios
                .Include(i => i.Almacen)
                .Include(i => i.Producto)
                .Include(i => i.DetalleInventarios)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<bool> UpdateAsync(Inventario inventario)
        {
            var existente = await context.Inventarios.FindAsync(inventario.Id);
            if (existente == null)
                return false;

            existente.Stock = inventario.Stock;
            existente.FechaActualizacion = inventario.FechaActualizacion;
            existente.SucursalId = inventario.SucursalId;
            existente.AlmacenId = inventario.AlmacenId;
            existente.ProductoId = inventario.ProductoId;

            await context.SaveChangesAsync();
            return true;
        }
    }
}