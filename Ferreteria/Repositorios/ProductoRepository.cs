using Ferreteria.Data;
using Ferreteria.Models;
using Microsoft.EntityFrameworkCore;

namespace Ferreteria.Repositorios
{
    public class ProductoRepository : IProductoRepository
    {
        public readonly AppDbContext _context;
        public ProductoRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Producto Producto)
        {
            await _context.Productos.AddAsync(Producto);
        }

        public async Task Delete(Producto Producto)
        {
            _context.Productos.Remove(Producto);
        }

        public  async  Task<IEnumerable<Producto>> GetAllAsync()
        {
            return await _context.Productos
             .AsNoTracking()
             .ToListAsync();
        }

        public async Task<Producto?> GetByIdAsync(int id)
        {
            return await _context.Productos.FindAsync(id);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task Update(Producto Producto)
        {
            _context.Productos.Update(Producto);
        }
    }
}
