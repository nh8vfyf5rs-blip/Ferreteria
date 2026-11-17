using Ferreteria.Data;
using Ferreteria.Models;
using Microsoft.EntityFrameworkCore;

namespace Ferreteria.Repositorios
{
    public class ProductoRepository(AppDbContext context) : IProductoRepository
    {
        public async Task<Producto?> CreateAsync(Producto producto)
        {
            var entry = await context.Productos.AddAsync(producto);
            await context.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var producto = await context.Productos.FindAsync(id);

            if (producto == null)
                return false;

            context.Productos.Remove(producto);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<IReadOnlyList<Producto>> GetAllAsync()
        {
            return await context.Productos
                .AsNoTracking()
                .Include(p => p.Categoria)
                .Include(p => p.Marca)
                .OrderBy(p => p.Nombre)
                .ToListAsync();
        }

        public async Task<Producto?> GetByIdAsync(int id)
        {
            return await context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Marca)
                .Include(p => p.DetalleVentas)
                .Include(p => p.DetalleCompras)
                .Include(p => p.DetalleInventarios)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async  Task<bool> UpdateAsync(Producto producto)
        {
            context.Productos.Update(producto);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
