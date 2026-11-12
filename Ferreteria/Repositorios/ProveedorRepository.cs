using Ferreteria.Data;
using Ferreteria.Models;
using Microsoft.EntityFrameworkCore;

namespace Ferreteria.Repositorios
{
    public class ProveedorRepository : IProveedorRepository
    {
        public readonly AppDbContext _context;
        public ProveedorRepository(AppDbContext context)
        {
            _context = context;
        }

            public async Task AddAsync(Proveedor proveedor)
        {
           await _context.Proveedores.AddAsync(proveedor);
        }

        public async Task Delete(Proveedor proveedor)
        {
            _context.Proveedores.Remove(proveedor);
        }

        public async Task<IEnumerable<Proveedor>> GetAllAsync()
        {
            return await _context.Proveedores
              .AsNoTracking()
              .ToListAsync();
        }

        public async Task<Proveedor?> GetByIdAsync(int id)
        {
            return await _context.Proveedores.FindAsync(id);
        }

        public async Task SaveAsync()
        {
           await _context.SaveChangesAsync();
        }

        public async Task Update(Proveedor proveedor)
        {
           _context.Proveedores.Update(proveedor);
        }
    }
}
