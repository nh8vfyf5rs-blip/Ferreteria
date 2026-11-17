using Ferreteria.Data;
using Ferreteria.Models;
using Microsoft.EntityFrameworkCore;

namespace Ferreteria.Repositorios
{
    public class MetodoPagoRepository(AppDbContext context) : IMetodoPagoRepository
    {
        public async Task<MetodoPago?> CreateAsync(MetodoPago metodoPago)
        {
            var entry = await context.MetodosPago.AddAsync(metodoPago);
            await context.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var metodoPago = await context.MetodosPago.FindAsync(id);

            if (metodoPago == null)
                return false;

            context.MetodosPago.Remove(metodoPago);
            await context.SaveChangesAsync();
            return true;
        }

        public async  Task<IReadOnlyList<MetodoPago>> GetAllAsync()
        {
            return await context.MetodosPago
               .AsNoTracking()
               .Include(m => m.DetallePagos)
               .OrderBy(m => m.Name)
               .ToListAsync();
        }

        public async Task<MetodoPago?> GetByIdAsync(int id)
        {
            return await context.MetodosPago
                .Include(m => m.DetallePagos)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<bool> UpdateAsync(MetodoPago metodoPago)
        {
            context.MetodosPago.Update(metodoPago);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
