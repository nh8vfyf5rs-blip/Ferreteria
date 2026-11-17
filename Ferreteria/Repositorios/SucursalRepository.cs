using Ferreteria.Data;
using Ferreteria.Models;
using Microsoft.EntityFrameworkCore;

namespace Ferreteria.Repositorios
{
    public class SucursalRepository(AppDbContext context) : ISucursalRepository
    {
        public async Task<Sucursal?> CreateAsync(Sucursal sucursal)
        {
            var entry = await context.Sucursales.AddAsync(sucursal);
            await context.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var sucursal = await context.Sucursales.FindAsync(id);

            if (sucursal == null)
                return false;

            context.Sucursales.Remove(sucursal);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<IReadOnlyList<Sucursal>> GetAllAsync()
        {
            return await context.Sucursales
                .AsNoTracking()
                .Include(s => s.Almacenes)
                .Include(s => s.Empleados)
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        public async Task<Sucursal?> GetByIdAsync(int id)
        {
            return await context.Sucursales
                .Include(s => s.Almacenes)
                .Include(s => s.Empleados)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async  Task<bool> UpdateAsync(Sucursal sucursal)
        {
            context.Sucursales.Update(sucursal);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
