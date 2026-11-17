using Ferreteria.Data;
using Ferreteria.Models;
using Microsoft.EntityFrameworkCore;

namespace Ferreteria.Repositorios
{
    public class ProveedorRepository(AppDbContext context) : IProveedorRepository
    {
        public async Task<Proveedor?> CreateAsync(Proveedor proveedor)
        {
            var entry = await context.Proveedores.AddAsync(proveedor);
            await context.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var proveedor = await context.Proveedores.FindAsync(id);

            if (proveedor == null)
                return false;

            context.Proveedores.Remove(proveedor);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<IReadOnlyList<Proveedor>> GetAllAsync()
        {
            return await context.Proveedores
                .AsNoTracking()
                .Include(p => p.Compras)
                .OrderBy(p => p.NombreEmpresa)
                .ToListAsync();
        }

        public async Task<Proveedor?> GetByIdAsync(int id)
        {
            return await context.Proveedores
                .Include(p => p.Compras)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<bool> UpdateAsync(Proveedor proveedor)
        {
            context.Proveedores.Update(proveedor);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
